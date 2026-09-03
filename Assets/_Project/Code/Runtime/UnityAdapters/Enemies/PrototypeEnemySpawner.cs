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
            new SpawnOffset(4, 0),
            new SpawnOffset(0, 4),
            new SpawnOffset(-4, 0),
            new SpawnOffset(0, -4)
        };

        [Header("Runtime Spawn")]
        [SerializeField] private bool spawnOverTime = true;
        [SerializeField] private float spawnIntervalSeconds = 4f;
        [SerializeField] private float initialSpawnGraceSeconds = 20f;
        [SerializeField] private int minimumActiveEnemies = 3;
        [SerializeField] private int maximumActiveEnemies = 10;
        [SerializeField] private float spawnPressureRampSeconds = 90f;
        [SerializeField] private int randomSpawnSeed = 1401;
        [SerializeField] private int minimumSpawnDistanceFromTarget = 8;
        [SerializeField] private int maximumSpawnDistanceFromTarget = 30;

        [Header("Enemy Variation")]
        [SerializeField] private int minimumEnemyHitPoints = 3;
        [SerializeField] private int maximumEnemyHitPoints = 5;
        [SerializeField] private float minimumMoveIntervalSeconds = 0.7f;
        [SerializeField] private float maximumMoveIntervalSeconds = 1.4f;

        [Header("Elite Spawning")]
        [SerializeField] private bool spawnElitesOverTime = true;
        [SerializeField] private float eliteSpawnIntervalSeconds = 150f;
        [SerializeField] private string eliteDisplayName = "Vanguard Kolt";
        [SerializeField] private int eliteHitPoints = 9;
        [SerializeField] private float eliteMoveIntervalSeconds = 0.9f;
        [SerializeField] private int eliteDefeatRewardValue = 5;
        [SerializeField] private float eliteScaleMultiplier = 1.35f;
        [SerializeField] private Color eliteTint = new Color(1f, 0.6f, 0.2f, 1f);

        [Header("Ranged Spawning")]
        [Range(0, 100)]
        [SerializeField] private int rangedSpawnChancePercent = 25;

        [Header("Threat")]
        [SerializeField] private float threatSecondsPerLevel = 90f;
        [SerializeField] private int threatMaximumLevel = 5;
        [SerializeField] private int threatHitPointsBonusPerLevel = 1;
        [SerializeField] private int threatRewardValueBonusPerLevel = 1;

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
        private float nextEliteSpawnTime;
        private int lastLoggedThreatLevel;
        private PrototypeEnemyView activeEliteView;
        private float spawnPressureStartTime;

        /// <summary>
        /// 현재 위협 단계. 원정 경과 시간에 따라 결정된다.
        /// </summary>
        public int CurrentThreatLevel => ThreatRules.ResolveThreatLevel(
            Mathf.Max(0f, Time.time - spawnPressureStartTime),
            threatSecondsPerLevel,
            threatMaximumLevel);

        public int MaximumThreatLevel => threatMaximumLevel;

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

            spawnPressureStartTime = Time.time;
            ScheduleNextEliteSpawn();
            nextRuntimeSpawnTime = Time.time + Mathf.Max(spawnIntervalSeconds, initialSpawnGraceSeconds);
        }

        private void Update()
        {
            int threatLevel = CurrentThreatLevel;

            if (threatLevel != lastLoggedThreatLevel)
            {
                Debug.Log($"Threat level is now {threatLevel}.", this);
                lastLoggedThreatLevel = threatLevel;
            }

            if (spawnElitesOverTime && Time.time >= nextEliteSpawnTime)
            {
                ScheduleNextEliteSpawn();

                if (TrySpawnRuntimeEnemy(out PrototypeEnemyView eliteView))
                {
                    eliteView.ConfigureElite(
                        EnemyBehaviorKind.Charger,
                        eliteDisplayName,
                        eliteDefeatRewardValue,
                        eliteHitPoints,
                        eliteMoveIntervalSeconds,
                        eliteScaleMultiplier,
                        eliteTint);

                    activeEliteView = eliteView;

                    int eliteThreatLevel = CurrentThreatLevel;

                    if (eliteThreatLevel > 0)
                    {
                        eliteView.ConfigureThreat(
                            eliteThreatLevel * threatHitPointsBonusPerLevel,
                            eliteThreatLevel * threatRewardValueBonusPerLevel);
                    }

                    Debug.Log($"Elite spawned. Name={eliteDisplayName}.", this);
                }
            }

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
            int targetActiveEnemies = GetCurrentActiveEnemyTarget();

            if (activeCount >= targetActiveEnemies)
            {
                return;
            }

            int spawnCount = targetActiveEnemies - activeCount;

            for (int i = 0; i < spawnCount; i++)
            {
                if (!TrySpawnRuntimeEnemy(out _))
                {
                    break;
                }
            }

            if (spawnElitesOverTime && Time.time >= nextEliteSpawnTime)
            {
                ScheduleNextEliteSpawn();

                if (TrySpawnRuntimeEnemy(out PrototypeEnemyView eliteView))
                {
                    eliteView.ConfigureElite(
                        EnemyBehaviorKind.Charger,
                        eliteDisplayName,
                        eliteDefeatRewardValue,
                        eliteHitPoints,
                        eliteMoveIntervalSeconds,
                        eliteScaleMultiplier,
                        eliteTint);

                    if (logRuntimeSpawns)
                    {
                        Debug.Log($"Elite spawned. Name={eliteDisplayName}.", this);
                    }
                }
            }
        }

        private int GetCurrentActiveEnemyTarget()
        {
            float elapsedSeconds = Mathf.Max(0f, Time.time - spawnPressureStartTime);
            int rampSteps = Mathf.FloorToInt(elapsedSeconds / spawnPressureRampSeconds);
            return Mathf.Min(maximumActiveEnemies, minimumActiveEnemies + rampSteps);
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

        private bool TrySpawnRuntimeEnemy(out PrototypeEnemyView spawnedEnemy)
        {
            spawnedEnemy = null;

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

            spawnedEnemy = enemy;
            if (enemy != null
                && rangedSpawnChancePercent > 0
                && spawnRandom.Next(0, 100) < rangedSpawnChancePercent)
            {
                enemy.ConfigureBehavior(EnemyBehaviorKind.Ranged);
            }

            if (enemy != null)
            {
                int threatLevel = CurrentThreatLevel;

                if (threatLevel > 0)
                {
                    enemy.ConfigureThreat(
                        threatLevel * threatHitPointsBonusPerLevel,
                        threatLevel * threatRewardValueBonusPerLevel);
                }
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

        private void ScheduleNextEliteSpawn()
        {
            nextEliteSpawnTime = Time.time + eliteSpawnIntervalSeconds;
        }

        /// <summary>
        /// 현재 활성 상태의 엘리트 정보를 반환한다. 엘리트가 없으면 false를 반환한다.
        /// </summary>
        public bool TryGetActiveEliteInfo(out string eliteName, out int currentHitPoints, out int maxHitPoints)
        {
            eliteName = "";
            currentHitPoints = 0;
            maxHitPoints = 0;

            if (activeEliteView == null || activeEliteView.IsDefeated || !activeEliteView.isActiveAndEnabled)
            {
                activeEliteView = null;
                return false;
            }

            eliteName = activeEliteView.DisplayName;
            currentHitPoints = activeEliteView.CurrentHitPoints;
            maxHitPoints = activeEliteView.MaxHitPoints;
            return true;
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
            minimumActiveEnemies = 3;
            maximumActiveEnemies = 10;
            randomSpawnSeed = 1401;
            minimumSpawnDistanceFromTarget = 8;
            maximumSpawnDistanceFromTarget = 30;
            minimumEnemyHitPoints = 3;
            maximumEnemyHitPoints = 5;
            minimumMoveIntervalSeconds = 0.7f;
            maximumMoveIntervalSeconds = 1.4f;
            spawnPressureRampSeconds = 90f;
            initialSpawnGraceSeconds = Mathf.Max(0f, initialSpawnGraceSeconds);
            initialSpawnGraceSeconds = 20f;
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
            spawnPressureRampSeconds = Mathf.Max(1f, spawnPressureRampSeconds);
            rangedSpawnChancePercent = Mathf.Clamp(rangedSpawnChancePercent, 0, 100);
            threatSecondsPerLevel = Mathf.Max(1f, threatSecondsPerLevel);
            threatMaximumLevel = Mathf.Max(0, threatMaximumLevel);
            threatHitPointsBonusPerLevel = Mathf.Max(0, threatHitPointsBonusPerLevel);
            threatRewardValueBonusPerLevel = Mathf.Max(0, threatRewardValueBonusPerLevel);
            initialSpawnGraceSeconds = Mathf.Max(0f, initialSpawnGraceSeconds);
            initialSpawnGraceSeconds = 20f;
        }
    }
}
