using DeepSeal.Combat;
using DeepSeal.Core;
using DeepSeal.Mining;
using DeepSeal.UnityAdapters.Grid;
using DeepSeal.UnityAdapters.Prototype;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Enemies
{
    /// <summary>
    /// Unity adapter that displays and advances one prototype enemy using pure Combat domain movement rules.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PrototypeEnemyView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PrototypeMineGridBootstrap mineGridBootstrap;
        [SerializeField] private Transform target;
        [SerializeField] private Transform controlledTransform;

        [Header("Enemy")]
        [SerializeField] private int enemyId;
        [SerializeField] private Vector2Int initialGridPosition;
        [SerializeField] private bool placeAtInitialPositionOnStart = true;

        [Header("Prototype Health")]
        [SerializeField] private int maxHitPoints = 3;
        [SerializeField] private bool disableOnDefeat = true;

        [Header("Movement")]
        [SerializeField] private float moveIntervalSeconds = 0.5f;
        [SerializeField] private bool logMovementResults;

        private EnemyState enemyState;
        private bool hasEnemyState;
        private int currentHitPoints;
        private bool isDefeated;
        private float nextMoveTime;
        private bool warnedMissingBootstrap;
        private bool warnedMissingGrid;
        private bool warnedMissingTarget;

        public bool HasEnemyState => hasEnemyState;

        public bool IsDefeated => isDefeated;

        public int CurrentHitPoints => currentHitPoints;

        public int MaxHitPoints => maxHitPoints;

        public EnemyState CurrentEnemy => enemyState;

        private void Start()
        {
            EnsureControlledTransform();

            if (!hasEnemyState)
            {
                var initialPosition = new GridPosition(initialGridPosition.x, initialGridPosition.y);
                SetEnemyState(new EnemyState(enemyId, initialPosition), placeAtInitialPositionOnStart);
            }

            if (currentHitPoints <= 0)
            {
                ResetPrototypeHealth();
            }

            ScheduleNextMove();
        }

        private void Update()
        {
            if (!hasEnemyState || isDefeated)
            {
                return;
            }

            if (Time.time < nextMoveTime)
            {
                return;
            }

            ScheduleNextMove();
            TryMoveTowardTarget();
        }

        public void Initialize(
            int id,
            GridPosition position,
            PrototypeMineGridBootstrap bootstrap,
            Transform moveTarget)
        {
            enemyId = Mathf.Max(0, id);
            initialGridPosition = new Vector2Int(position.X, position.Y);
            mineGridBootstrap = bootstrap;
            target = moveTarget;

            EnsureControlledTransform();
            SetEnemyState(new EnemyState(enemyId, position), true);
            ResetPrototypeHealth();
            ScheduleNextMove();
        }

        public bool TryGetCurrentEnemy(out EnemyState enemy)
        {
            enemy = enemyState;
            return hasEnemyState && !isDefeated && isActiveAndEnabled;
        }

        public bool TryApplyPrototypeDamage(int damage)
        {
            if (isDefeated)
            {
                return false;
            }

            if (damage <= 0)
            {
                Debug.LogWarning(
                    $"Ignored prototype enemy damage because damage must be greater than zero. Damage={damage}.",
                    this);
                return false;
            }

            currentHitPoints = Mathf.Max(0, currentHitPoints - damage);

            if (currentHitPoints > 0)
            {
                return false;
            }

            Defeat();
            return true;
        }

        [ContextMenu("Move Toward Target Once")]
        public bool TryMoveTowardTarget()
        {
            if (isDefeated)
            {
                return false;
            }

            if (!TryResolveGrid(out MineGrid grid))
            {
                return false;
            }

            if (target == null)
            {
                if (!warnedMissingTarget)
                {
                    Debug.LogWarning("Prototype enemy cannot move because target is not assigned.", this);
                    warnedMissingTarget = true;
                }

                return false;
            }

            warnedMissingTarget = false;

            GridPosition targetPosition = GridCoordinateConverter.WorldToGridPosition(target.position);
            EnemyMoveResult result = EnemyMovementRules.TryMoveToward(grid, enemyState, targetPosition);

            if (result.Moved)
            {
                SetEnemyState(result.CurrentEnemy, true);
            }

            if (logMovementResults)
            {
                Debug.Log(
                    $"Enemy move result: Type={result.Type}, Direction={result.Direction}, Attempted={result.AttemptedPosition}, Current={result.CurrentEnemy.Position}.",
                    this);
            }

            return result.Moved;
        }

        private bool TryResolveGrid(out MineGrid grid)
        {
            grid = null;

            if (mineGridBootstrap == null)
            {
                if (!warnedMissingBootstrap)
                {
                    Debug.LogError("Prototype enemy requires a Prototype Mine Grid Bootstrap reference.", this);
                    warnedMissingBootstrap = true;
                }

                return false;
            }

            warnedMissingBootstrap = false;

            if (!mineGridBootstrap.TryGetCurrentGrid(out grid))
            {
                if (!warnedMissingGrid)
                {
                    Debug.LogWarning("Prototype enemy cannot move because no MineGrid has been generated yet.", this);
                    warnedMissingGrid = true;
                }

                return false;
            }

            warnedMissingGrid = false;
            return true;
        }

        private void SetEnemyState(EnemyState newEnemyState, bool updateTransform)
        {
            enemyState = newEnemyState;
            hasEnemyState = true;

            if (updateTransform)
            {
                SyncTransformToGridPosition(enemyState.Position);
            }
        }

        private void ResetPrototypeHealth()
        {
            currentHitPoints = maxHitPoints;
            isDefeated = false;
        }

        private void Defeat()
        {
            isDefeated = true;

            if (disableOnDefeat)
            {
                gameObject.SetActive(false);
            }
        }

        private void SyncTransformToGridPosition(GridPosition position)
        {
            EnsureControlledTransform();

            if (controlledTransform == null)
            {
                return;
            }

            float z = controlledTransform.position.z;
            controlledTransform.position = GridCoordinateConverter.GridToWorldCenter(position, z);
        }

        private void ScheduleNextMove()
        {
            nextMoveTime = Time.time + moveIntervalSeconds;
        }

        private void EnsureControlledTransform()
        {
            if (controlledTransform == null)
            {
                controlledTransform = transform;
            }
        }

        private void Reset()
        {
            controlledTransform = transform;
            maxHitPoints = 3;
            disableOnDefeat = true;
            moveIntervalSeconds = 0.5f;
            placeAtInitialPositionOnStart = true;
        }

        private void OnValidate()
        {
            enemyId = Mathf.Max(0, enemyId);
            maxHitPoints = Mathf.Max(1, maxHitPoints);
            moveIntervalSeconds = Mathf.Max(0.05f, moveIntervalSeconds);
        }
    }
}