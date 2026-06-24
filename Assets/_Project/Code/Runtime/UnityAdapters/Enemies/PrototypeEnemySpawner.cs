using System;
using System.Collections.Generic;
using DeepSeal.Combat;
using DeepSeal.Core;
using DeepSeal.Mining;
using DeepSeal.UnityAdapters.Grid;
using DeepSeal.UnityAdapters.Prototype;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Enemies
{
    /// <summary>
    /// Prototype-only Unity adapter that spawns simple enemy views near the generated mine start position
    /// and can maintain runtime enemy pressure over time.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypeEnemySpawner : MonoBehaviour
    {
        [Serializable]
        private struct SpawnOffset
        {
            [SerializeField] private int x;
            [SerializeField] private int y;

            public SpawnOffset(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public GridPosition ToPosition(GridPosition origin)
            {
                return origin.Offset(x, y);
            }

            public override string ToString()
            {
                return $"({x}, {y})";
            }
        }

        [Header("References")]
        [SerializeField] private PrototypeMineGridBootstrap mineGridBootstrap;
        [SerializeField] private Transform target;
        [SerializeField] private PrototypeEnemyView enemyPrefab;
        [SerializeField] private Transform spawnParent;

        [Header("Initial Spawn")]
        [SerializeField] private bool spawnOnStart = true;
        [SerializeField] private bool generateGridIfMissing = true;
        [SerializeField] private int firstEnemyId;
        [SerializeField]
        private SpawnOffset[] spawnOffsets =
        {
            new SpawnOffset(1, 0),
            new SpawnOffset(0, 1),
            new SpawnOffset(-1, 0)
        };

        [Header("Runtime Spawn")]
        [SerializeField] private bool spawnOverTime = true;
        [SerializeField] private float spawnIntervalSeconds = 4f;
        [SerializeField] private int minimumActiveEnemies = 4;
        [SerializeField] private int maximumActiveEnemies = 10;
        [SerializeField] private int randomSpawnSeed = 1401;
        [SerializeField] private int minimumSpawnDistanceFromTarget = 5;
        [SerializeField] private int maximumSpawnDistanceFromTarget = 14;

        [Header("Enemy Variation")]
        [SerializeField] private int minimumEnemyHitPoints = 3;
        [SerializeField] private int maximumEnemyHitPoints = 5;
        [SerializeField] private float minimumMoveIntervalSeconds = 0.35f;
        [SerializeField] private float maximumMoveIntervalSeconds = 0.7f;

        [Header("Debug")]
        [SerializeField] private bool logSkippedSpawns;
        [SerializeField] private bool logRuntimeSpawns;

        private readonly List<PrototypeEnemyView> spawnedEnemies = new List<PrototypeEnemyView>();
        private readonly List<GridPosition> occupiedPositions = new List<GridPosition>(32);
        private System.Random spawnRandom;
        private bool hasSpawned;
        private bool hasInitializedRandom;
        private int nextEnemyId;
        private float nextRuntimeSpawnTime;

        public IReadOnlyList<PrototypeEnemyView> SpawnedEnemies => spawnedEnemies;

        public int ActiveEnemyCount
        {
            get
            {
                int count = 0;

                for (int i = 0; i < spawnedEnemies.Count; i++)
                {
                    PrototypeEnemyView enemy = spawnedEnemies[i];

                    if (enemy != null && !enemy.IsDefeated && enemy.isActiveAndEnabled)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        private void Start()
        {
            EnsureRandomInitialized();

            if (spawnOnStart)
            {
                SpawnEnemies();
            }

            ScheduleNextRuntimeSpawn();
        }

        private void Update()
        {
            if (!spawnOverTime)
            {
                return;
            }

            if (Time.time < nextRuntimeSpawnTime)
            {
                return;
            }

            ScheduleNextRuntimeSpawn();
            RemoveInactiveEnemyReferences();

            int activeCount = ActiveEnemyCount;

            if (activeCount >= maximumActiveEnemies || activeCount >= minimumActiveEnemies)
            {
                return;
            }

            int spawnCount = Mathf.Min(
                minimumActiveEnemies - activeCount,
                maximumActiveEnemies - activeCount);

            for (int i = 0; i < spawnCount; i++)
            {
                if (!TrySpawnRuntimeEnemy())
                {
                    break;
                }
            }
        }

        [ContextMenu("Spawn Enemies")]
        public void SpawnEnemies()
        {
            EnsureRandomInitialized();

            if (hasSpawned)
            {
                Debug.LogWarning("Prototype enemies have already been spawned by this spawner.", this);
                return;
            }

            if (!TryResolveGrid(out MineGrid grid))
            {
                return;
            }

            if (!TryValidateSpawnReferences())
            {
                return;
            }

            if (!mineGridBootstrap.TryGetStartPosition(out GridPosition startPosition))
            {
                Debug.LogError("Cannot spawn prototype enemies because mine start position is unavailable.", this);
                return;
            }

            if (spawnOffsets == null || spawnOffsets.Length == 0)
            {
                Debug.LogWarning("No prototype enemy spawn offsets are configured.", this);
                hasSpawned = true;
                return;
            }

            for (int i = 0; i < spawnOffsets.Length; i++)
            {
                GridPosition spawnPosition = spawnOffsets[i].ToPosition(startPosition);

                if (!CanSpawnAt(grid, spawnPosition))
                {
                    if (logSkippedSpawns)
                    {
                        Debug.LogWarning(
                            $"Skipped prototype enemy spawn at {spawnPosition}. Offset={spawnOffsets[i]}. Cell is blocked or out of bounds.",
                            this);
                    }

                    continue;
                }

                SpawnEnemyAt(spawnPosition);
            }

            hasSpawned = true;
        }

        public void RemoveInactiveEnemyReferences()
        {
            for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
            {
                PrototypeEnemyView enemy = spawnedEnemies[i];

                if (enemy == null || enemy.IsDefeated || !enemy.gameObject.activeInHierarchy)
                {
                    spawnedEnemies.RemoveAt(i);
                }
            }
        }

        private bool TrySpawnRuntimeEnemy()
        {
            EnsureRandomInitialized();

            if (!TryResolveGrid(out MineGrid grid))
            {
                return false;
            }

            if (!TryValidateSpawnReferences())
            {
                return false;
            }

            GridPosition targetPosition = GridCoordinateConverter.WorldToGridPosition(target.position);
            CollectOccupiedPositions(targetPosition);

            var settings = new EnemySpawnSettings(
                minimumSpawnDistanceFromTarget,
                maximumSpawnDistanceFromTarget);

            if (!EnemySpawnRules.TryFindSpawnPosition(
                    grid,
                    targetPosition,
                    occupiedPositions,
                    settings,
                    spawnRandom,
                    out GridPosition spawnPosition))
            {
                if (logSkippedSpawns)
                {
                    Debug.LogWarning(
                        "Runtime enemy spawn skipped because no valid passable spawn position was found.",
                        this);
                }

                return false;
            }

            PrototypeEnemyView enemy = SpawnEnemyAt(spawnPosition);

            if (logRuntimeSpawns && enemy != null)
            {
                Debug.Log(
                    $"Runtime enemy spawned. Id={enemy.CurrentEnemy.Id}, Position={spawnPosition}, Active={ActiveEnemyCount}.",
                    this);
            }

            return enemy != null;
        }

        private PrototypeEnemyView SpawnEnemyAt(GridPosition spawnPosition)
        {
            Transform parent = spawnParent != null ? spawnParent : transform;
            int enemyId = AllocateEnemyId();
            Vector3 worldPosition = GridCoordinateConverter.GridToWorldCenter(spawnPosition);
            int hitPoints = NextEnemyHitPoints();
            float moveInterval = NextMoveIntervalSeconds();

            PrototypeEnemyView enemyView = Instantiate(
                enemyPrefab,
                worldPosition,
                Quaternion.identity,
                parent);

            enemyView.name = $"PrototypeEnemy_{enemyId}";
            enemyView.Initialize(
                enemyId,
                spawnPosition,
                mineGridBootstrap,
                target,
                hitPoints,
                moveInterval);

            spawnedEnemies.Add(enemyView);
            return enemyView;
        }

        private bool TryResolveGrid(out MineGrid grid)
        {
            grid = null;

            if (mineGridBootstrap == null)
            {
                Debug.LogError("Cannot spawn prototype enemies because Prototype Mine Grid Bootstrap is not assigned.", this);
                return false;
            }

            if (!mineGridBootstrap.TryGetCurrentGrid(out grid) && generateGridIfMissing)
            {
                mineGridBootstrap.TryGenerateAndRender();
            }

            if (!mineGridBootstrap.TryGetCurrentGrid(out grid))
            {
                Debug.LogWarning("Cannot spawn prototype enemies because no MineGrid has been generated yet.", this);
                return false;
            }

            return true;
        }

        private bool TryValidateSpawnReferences()
        {
            if (enemyPrefab == null)
            {
                Debug.LogError("Cannot spawn prototype enemies because Enemy Prefab is not assigned.", this);
                return false;
            }

            if (target == null)
            {
                Debug.LogError("Cannot spawn prototype enemies because Target is not assigned.", this);
                return false;
            }

            return true;
        }

        private void CollectOccupiedPositions(GridPosition targetPosition)
        {
            occupiedPositions.Clear();
            occupiedPositions.Add(targetPosition);

            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                PrototypeEnemyView enemy = spawnedEnemies[i];

                if (enemy == null || enemy.IsDefeated || !enemy.isActiveAndEnabled)
                {
                    continue;
                }

                if (enemy.TryGetCurrentEnemy(out EnemyState enemyState))
                {
                    occupiedPositions.Add(enemyState.Position);
                }
            }
        }

        private int AllocateEnemyId()
        {
            int id = nextEnemyId;
            nextEnemyId++;
            return id;
        }

        private int NextEnemyHitPoints()
        {
            if (minimumEnemyHitPoints >= maximumEnemyHitPoints)
            {
                return minimumEnemyHitPoints;
            }

            return spawnRandom.Next(minimumEnemyHitPoints, maximumEnemyHitPoints + 1);
        }

        private float NextMoveIntervalSeconds()
        {
            if (minimumMoveIntervalSeconds >= maximumMoveIntervalSeconds)
            {
                return minimumMoveIntervalSeconds;
            }

            return Mathf.Lerp(
                minimumMoveIntervalSeconds,
                maximumMoveIntervalSeconds,
                (float)spawnRandom.NextDouble());
        }

        private void EnsureRandomInitialized()
        {
            if (hasInitializedRandom)
            {
                return;
            }

            spawnRandom = new System.Random(randomSpawnSeed);
            nextEnemyId = firstEnemyId;
            hasInitializedRandom = true;
        }

        private void ScheduleNextRuntimeSpawn()
        {
            nextRuntimeSpawnTime = Time.time + spawnIntervalSeconds;
        }

        private static bool CanSpawnAt(MineGrid grid, GridPosition position)
        {
            if (!grid.TryGetCell(position, out TerrainCell cell))
            {
                return false;
            }

            return cell.IsPassable;
        }

        private void Reset()
        {
            spawnParent = transform;
            spawnOnStart = true;
            generateGridIfMissing = true;
            firstEnemyId = 0;
            spawnOverTime = true;
            spawnIntervalSeconds = 4f;
            minimumActiveEnemies = 4;
            maximumActiveEnemies = 10;
            randomSpawnSeed = 1401;
            minimumSpawnDistanceFromTarget = 5;
            maximumSpawnDistanceFromTarget = 14;
            minimumEnemyHitPoints = 3;
            maximumEnemyHitPoints = 5;
            minimumMoveIntervalSeconds = 0.35f;
            maximumMoveIntervalSeconds = 0.7f;
        }

        private void OnValidate()
        {
            firstEnemyId = Mathf.Max(0, firstEnemyId);
            spawnIntervalSeconds = Mathf.Max(0.1f, spawnIntervalSeconds);
            minimumActiveEnemies = Mathf.Max(0, minimumActiveEnemies);
            maximumActiveEnemies = Mathf.Max(minimumActiveEnemies, maximumActiveEnemies);
            minimumSpawnDistanceFromTarget = Mathf.Max(0, minimumSpawnDistanceFromTarget);
            maximumSpawnDistanceFromTarget = Mathf.Max(minimumSpawnDistanceFromTarget, maximumSpawnDistanceFromTarget);
            minimumEnemyHitPoints = Mathf.Max(1, minimumEnemyHitPoints);
            maximumEnemyHitPoints = Mathf.Max(minimumEnemyHitPoints, maximumEnemyHitPoints);
            minimumMoveIntervalSeconds = Mathf.Max(0.05f, minimumMoveIntervalSeconds);
            maximumMoveIntervalSeconds = Mathf.Max(minimumMoveIntervalSeconds, maximumMoveIntervalSeconds);
        }
    }
}