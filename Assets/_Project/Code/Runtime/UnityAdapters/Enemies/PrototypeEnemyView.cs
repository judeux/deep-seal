using System;
using DeepSeal.Combat;
using DeepSeal.Core;
using DeepSeal.Mining;
using DeepSeal.UnityAdapters.Grid;
using DeepSeal.UnityAdapters.Player;
using DeepSeal.UnityAdapters.Prototype;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Enemies
{
    /// <summary>
    /// Unity adapter that displays and advances one prototype enemy using pure Combat domain rules.
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
        [SerializeField] private bool usePathfinding = true;
        [SerializeField] private int maxPathVisitedCells = 512;
        [SerializeField] private bool logMovementResults;

        [Header("Behavior")]
        [SerializeField] private EnemyBehaviorKind behaviorKind = EnemyBehaviorKind.Chaser;

        [Header("Charger Behavior")]
        [SerializeField] private float chargeCooldownSeconds = 4f;
        [SerializeField] private float chargeWindupSeconds = 0.6f;
        [SerializeField] private float chargeSpeedCellsPerSecond = 9f;
        [SerializeField] private int chargeMaxCells = 6;
        [SerializeField] private int chargeOffAxisToleranceCells = 1;
        [SerializeField] private float chargeStunSeconds = 1.5f;
        [SerializeField] private Color chargeWindupTint = new Color(1f, 0.35f, 0.25f, 1f);

        [Header("Ranged Behavior")]
        [SerializeField] private int rangedMinimumRangeCells = 3;
        [SerializeField] private int rangedMaximumRangeCells = 7;
        [SerializeField] private float rangedFireCooldownSeconds = 2.5f;
        [SerializeField] private int rangedDamage = 1;
        [SerializeField] private Color rangedProjectileTint = new Color(1f, 0.4f, 0.35f, 1f);

        private EnemyState enemyState;
        private bool hasEnemyState;
        private HitPointState hitPoints;
        private bool isDefeated;
        private float nextMoveTime;
        private bool warnedMissingBootstrap;
        private bool warnedMissingGrid;
        private bool warnedMissingTarget;

        private enum ChargePhase
        {
            Cruise = 0,
            Windup = 1,
            Charging = 2,
            Stunned = 3
        }

        private ChargePhase chargePhase;
        private float nextChargeTime;
        private float phaseEndTime;
        private GridPosition[] chargePath;
        private int chargePathIndex;
        private float nextChargeCellTime;
        private bool chargeEndsStunned;
        private Color defaultSpriteTint = Color.white;
        private bool hasDefaultSpriteTint;
        private string displayName = "";
        private int defeatRewardValue = 1;
        private float nextRangedFireTime;
        private PrototypePlayerHealth cachedPlayerHealth;
        private bool warnedMissingPlayerHealth;

        public bool HasEnemyState => hasEnemyState;

        public bool IsDefeated => isDefeated;

        public int CurrentHitPoints => hitPoints.IsInitialized ? hitPoints.CurrentHitPoints : maxHitPoints;

        public int MaxHitPoints => hitPoints.IsInitialized ? hitPoints.MaxHitPoints : maxHitPoints;

        public EnemyState CurrentEnemy => enemyState;

        public string DisplayName => displayName;

        public int DefeatRewardValue => defeatRewardValue;

        public EnemyBehaviorKind BehaviorKind => behaviorKind;

        private void Start()
        {
            EnsureControlledTransform();

            if (!hasEnemyState)
            {
                var initialPosition = new GridPosition(initialGridPosition.x, initialGridPosition.y);
                SetEnemyState(new EnemyState(enemyId, initialPosition), placeAtInitialPositionOnStart);
            }

            EnsurePrototypeHealthInitialized();
            ScheduleNextMove();
            ScheduleNextChargeTime();
        }

        private void Update()
        {
            if (!hasEnemyState || isDefeated)
            {
                return;
            }

            if (behaviorKind == EnemyBehaviorKind.Charger)
            {
                UpdateCharger();
                return;
            }

            if (behaviorKind == EnemyBehaviorKind.Ranged)
            {
                UpdateRanged();
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
            Initialize(id, position, bootstrap, moveTarget, maxHitPoints, moveIntervalSeconds);
        }

        public void Initialize(
            int id,
            GridPosition position,
            PrototypeMineGridBootstrap bootstrap,
            Transform moveTarget,
            int configuredMaxHitPoints,
            float configuredMoveIntervalSeconds)
        {
            enemyId = Mathf.Max(0, id);
            initialGridPosition = new Vector2Int(position.X, position.Y);
            mineGridBootstrap = bootstrap;
            target = moveTarget;
            maxHitPoints = Mathf.Max(1, configuredMaxHitPoints);
            moveIntervalSeconds = Mathf.Max(0.05f, configuredMoveIntervalSeconds);

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

            EnsurePrototypeHealthInitialized();

            DamageResult result;

            try
            {
                result = HealthRules.ApplyDamage(hitPoints, damage);
            }
            catch (ArgumentException exception)
            {
                Debug.LogWarning(
                    $"Ignored prototype enemy damage because damage settings are invalid. Damage={damage}. {exception.Message}",
                    this);
                return false;
            }

            hitPoints = result.Current;

            if (!result.DefeatedThisHit)
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
            EnemyMoveResult result = usePathfinding
                ? EnemyMovementRules.TryMoveTowardWithPathfinding(
                    grid,
                    enemyState,
                    targetPosition,
                    maxPathVisitedCells)
                : EnemyMovementRules.TryMoveToward(grid, enemyState, targetPosition);

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

        private void ScheduleNextChargeTime()
        {
            nextChargeTime = Time.time + chargeCooldownSeconds;
        }

        private void UpdateCharger()
        {
            switch (chargePhase)
            {
                case ChargePhase.Cruise:
                    UpdateChargeCruise();
                    break;
                case ChargePhase.Windup:
                    UpdateChargeWindup();
                    break;
                case ChargePhase.Charging:
                    UpdateChargeMovement();
                    break;
                case ChargePhase.Stunned:
                    UpdateChargeStun();
                    break;
            }
        }

        private void UpdateChargeCruise()
        {
            if (Time.time >= nextChargeTime && TryBeginChargeWindup())
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

        private bool TryBeginChargeWindup()
        {
            if (target == null || !TryResolveGrid(out MineGrid grid))
            {
                return false;
            }

            GridPosition targetPosition = GridCoordinateConverter.WorldToGridPosition(target.position);

            if (!IsWithinChargeAlignment(enemyState.Position, targetPosition))
            {
                return false;
            }

            chargePhase = ChargePhase.Windup;
            phaseEndTime = Time.time + chargeWindupSeconds;
            ApplyWindupTint();
            return true;
        }

        private bool IsWithinChargeAlignment(GridPosition from, GridPosition to)
        {
            int deltaX = Mathf.Abs(to.X - from.X);
            int deltaY = Mathf.Abs(to.Y - from.Y);
            int offAxisDistance = deltaX >= deltaY ? deltaY : deltaX;

            return offAxisDistance <= chargeOffAxisToleranceCells;
        }

        private void UpdateChargeWindup()
        {
            if (Time.time < phaseEndTime)
            {
                return;
            }

            if (target == null || !TryResolveGrid(out MineGrid grid))
            {
                EndCharge(false);
                return;
            }

            GridPosition targetPosition = GridCoordinateConverter.WorldToGridPosition(target.position);
            EnemyChargeResult chargeResult = EnemyChargeRules.TraceCharge(
                grid,
                enemyState.Position,
                targetPosition,
                chargeMaxCells);

            if (chargeResult.PathCellCount == 0)
            {
                EndCharge(chargeResult.Stunned);
                return;
            }

            chargePath = chargeResult.PathCells;
            chargePathIndex = 0;
            chargeEndsStunned = chargeResult.Stunned;
            nextChargeCellTime = Time.time;
            chargePhase = ChargePhase.Charging;
        }

        private void UpdateChargeMovement()
        {
            if (chargePath == null)
            {
                EndCharge(false);
                return;
            }

            while (chargePathIndex < chargePath.Length && Time.time >= nextChargeCellTime)
            {
                SetEnemyState(new EnemyState(enemyState.Id, chargePath[chargePathIndex]), true);
                chargePathIndex++;
                nextChargeCellTime += 1f / Mathf.Max(0.1f, chargeSpeedCellsPerSecond);
            }

            if (chargePathIndex >= chargePath.Length)
            {
                EndCharge(chargeEndsStunned);
            }
        }

        private void UpdateChargeStun()
        {
            if (Time.time < phaseEndTime)
            {
                return;
            }

            EndCharge(false);
        }

        private void EndCharge(bool stunned)
        {
            chargePath = null;
            chargePathIndex = 0;
            RestoreDefaultTint();
            ScheduleNextChargeTime();

            if (stunned)
            {
                chargePhase = ChargePhase.Stunned;
                phaseEndTime = Time.time + chargeStunSeconds;
                return;
            }

            chargePhase = ChargePhase.Cruise;
            ScheduleNextMove();
        }

        private void ApplyWindupTint()
        {
            if (!TryGetSpriteRenderer(out SpriteRenderer spriteRenderer))
            {
                return;
            }

            if (!hasDefaultSpriteTint)
            {
                defaultSpriteTint = spriteRenderer.color;
                hasDefaultSpriteTint = true;
            }

            spriteRenderer.color = chargeWindupTint;
        }

        private void RestoreDefaultTint()
        {
            if (TryGetSpriteRenderer(out SpriteRenderer spriteRenderer) && hasDefaultSpriteTint)
            {
                spriteRenderer.color = defaultSpriteTint;
            }
        }

        private bool TryGetSpriteRenderer(out SpriteRenderer spriteRenderer)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            return spriteRenderer != null;
        }

        /// <summary>
        /// 명명 엘리트 구성을 적용한다. 행동, 표시 이름, 격파 보상, 체력, 이동 간격, 크기, 색조를 지정한다.
        /// </summary>
        public void ConfigureElite(
            EnemyBehaviorKind kind,
            string eliteDisplayName,
            int eliteDefeatRewardValue,
            int eliteMaxHitPoints,
            float eliteMoveIntervalSeconds,
            float scaleMultiplier,
            Color tint)
        {
            behaviorKind = kind;
            displayName = eliteDisplayName ?? "";
            defeatRewardValue = Mathf.Max(1, eliteDefeatRewardValue);
            maxHitPoints = Mathf.Max(1, eliteMaxHitPoints);
            moveIntervalSeconds = Mathf.Max(0.05f, eliteMoveIntervalSeconds);

            ResetPrototypeHealth();
            ScheduleNextMove();
            ScheduleNextChargeTime();
            EnsureControlledTransform();

            if (controlledTransform != null)
            {
                float multiplier = Mathf.Max(0.1f, scaleMultiplier);
                controlledTransform.localScale = new Vector3(
                    controlledTransform.localScale.x * multiplier,
                    controlledTransform.localScale.y * multiplier,
                    controlledTransform.localScale.z);
            }

            if (TryGetSpriteRenderer(out SpriteRenderer spriteRenderer))
            {
                if (!hasDefaultSpriteTint)
                {
                    defaultSpriteTint = spriteRenderer.color;
                    hasDefaultSpriteTint = true;
                }

                spriteRenderer.color = tint;
            }
            if (!string.IsNullOrEmpty(displayName))
            {
                PrototypeEnemyNameplate.Create(controlledTransform, displayName);
            }
        }

                /// <summary>
        /// 이 적의 행동 유형만 교체한다. 원거리 등 행동 세부값은 프리팹의 인스펙터 값을 따른다.
        /// </summary>
        public void ConfigureBehavior(EnemyBehaviorKind kind)
        {
            behaviorKind = kind;
        }

        private void UpdateRanged()
        {
            if (target == null || !TryResolveGrid(out MineGrid grid))
            {
                return;
            }

            GridPosition targetPosition = GridCoordinateConverter.WorldToGridPosition(target.position);

            if (Time.time >= nextRangedFireTime && TryFireRangedShot(grid, targetPosition))
            {
                return;
            }

            if (Time.time < nextMoveTime)
            {
                return;
            }

            ScheduleNextMove();
            TryRangedBandStep(grid, targetPosition);
        }

        private bool TryFireRangedShot(MineGrid grid, GridPosition targetPosition)
        {
            int distance = AttackTargetingRules.ManhattanDistance(enemyState.Position, targetPosition);

            if (distance < rangedMinimumRangeCells || distance > rangedMaximumRangeCells)
            {
                return false;
            }

            if (!EnemyRangedRules.HasClearCardinalLine(grid, enemyState.Position, targetPosition))
            {
                return false;
            }

            if (!TryResolvePlayerHealth(out PrototypePlayerHealth health))
            {
                return false;
            }

            nextRangedFireTime = Time.time + rangedFireCooldownSeconds;

            PrototypeProjectileView projectile = PrototypeProjectileView.Create(transform.position, transform);
            projectile.SetTint(rangedProjectileTint);

            Vector3 impactWorldPosition = GridCoordinateConverter.GridToWorldCenter(targetPosition);
            projectile.Begin(impactWorldPosition, arrived => OnRangedProjectileArrived(health, targetPosition));
            return true;
        }

        private void OnRangedProjectileArrived(PrototypePlayerHealth health, GridPosition impactPosition)
        {
            if (target == null)
            {
                return;
            }

            // 발사 시점 셀에 플레이어가 아직 남아 있을 때만 피해를 입힌다. 이동하면 회피된다.
            GridPosition playerPosition = GridCoordinateConverter.WorldToGridPosition(target.position);

            if (playerPosition == impactPosition)
            {
                health.TryApplyDamage(rangedDamage);
            }
        }

        private void TryRangedBandStep(MineGrid grid, GridPosition targetPosition)
        {
            GridDirection stepDirection = EnemyRangedRules.ResolveBandStepDirection(
                enemyState.Position,
                targetPosition,
                rangedMinimumRangeCells,
                rangedMaximumRangeCells);

            if (stepDirection == GridDirection.None)
            {
                return;
            }

            GridPosition offset = stepDirection.ToOffset();
            GridPosition next = enemyState.Position.Offset(offset.X, offset.Y);

            if (grid.TryGetCell(next, out TerrainCell cell) && cell.IsPassable)
            {
                SetEnemyState(enemyState.WithPosition(next), true);
            }
        }

        private bool TryResolvePlayerHealth(out PrototypePlayerHealth health)
        {
            health = cachedPlayerHealth;

            if (health == null && target != null)
            {
                health = target.GetComponent<PrototypePlayerHealth>();
                cachedPlayerHealth = health;
            }

            if (health == null)
            {
                if (!warnedMissingPlayerHealth)
                {
                    Debug.LogWarning("Ranged enemy requires a Prototype Player Health component on its target.", this);
                    warnedMissingPlayerHealth = true;
                }

                return false;
            }

            return true;
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

        private void EnsurePrototypeHealthInitialized()
        {
            if (!hitPoints.IsInitialized)
            {
                ResetPrototypeHealth();
            }
        }

        private void ResetPrototypeHealth()
        {
            hitPoints = HitPointState.Full(maxHitPoints);
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
            usePathfinding = true;
            maxPathVisitedCells = 512;
            placeAtInitialPositionOnStart = true;
            behaviorKind = EnemyBehaviorKind.Chaser;
        }

        private void OnValidate()
        {
            enemyId = Mathf.Max(0, enemyId);
            maxHitPoints = Mathf.Max(1, maxHitPoints);
            moveIntervalSeconds = Mathf.Max(0.05f, moveIntervalSeconds);
            maxPathVisitedCells = Mathf.Max(1, maxPathVisitedCells);
            chargeCooldownSeconds = Mathf.Max(0.5f, chargeCooldownSeconds);
            chargeWindupSeconds = Mathf.Max(0.05f, chargeWindupSeconds);
            chargeSpeedCellsPerSecond = Mathf.Max(0.5f, chargeSpeedCellsPerSecond);
            chargeMaxCells = Mathf.Max(0, chargeMaxCells);
            chargeOffAxisToleranceCells = Mathf.Max(0, chargeOffAxisToleranceCells);
            chargeStunSeconds = Mathf.Max(0f, chargeStunSeconds);
            rangedMinimumRangeCells = Mathf.Max(0, rangedMinimumRangeCells);
            rangedMaximumRangeCells = Mathf.Max(rangedMinimumRangeCells, rangedMaximumRangeCells);
            rangedFireCooldownSeconds = Mathf.Max(0.5f, rangedFireCooldownSeconds);
            rangedDamage = Mathf.Max(1, rangedDamage);
        }
    }
}
