using System;
using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Mining;
using DeepSeal.UnityAdapters.Grid;
using DeepSeal.UnityAdapters.Prototype;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Enemies
{
    /// <summary>
    /// Prototype-only Unity adapter that spawns simple enemy views near the generated mine start position.
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

        [Header("Spawn")]
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

        [Header("Debug")]
        [SerializeField] private bool logSkippedSpawns;

        private readonly List<PrototypeEnemyView> spawnedEnemies = new List<PrototypeEnemyView>();
        private bool hasSpawned;

        public IReadOnlyList<PrototypeEnemyView> SpawnedEnemies => spawnedEnemies;

        private void Start()
        {
            if (spawnOnStart)
            {
                SpawnEnemies();
            }
        }

        [ContextMenu("Spawn Enemies")]
        public void SpawnEnemies()
        {
            if (hasSpawned)
            {
                Debug.LogWarning("Prototype enemies have already been spawned by this spawner.", this);
                return;
            }

            if (!TryResolveGrid(out MineGrid grid))
            {
                return;
            }

            if (enemyPrefab == null)
            {
                Debug.LogError("Cannot spawn prototype enemies because Enemy Prefab is not assigned.", this);
                return;
            }

            if (target == null)
            {
                Debug.LogError("Cannot spawn prototype enemies because Target is not assigned.", this);
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

            Transform parent = spawnParent != null ? spawnParent : transform;
            int spawnedCount = 0;

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

                int enemyId = firstEnemyId + spawnedCount;
                Vector3 worldPosition = GridCoordinateConverter.GridToWorldCenter(spawnPosition);

                PrototypeEnemyView enemyView = Instantiate(
                    enemyPrefab,
                    worldPosition,
                    Quaternion.identity,
                    parent);

                enemyView.name = $"PrototypeEnemy_{enemyId}";
                enemyView.Initialize(enemyId, spawnPosition, mineGridBootstrap, target);
                spawnedEnemies.Add(enemyView);

                spawnedCount++;
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
        }

        private void OnValidate()
        {
            firstEnemyId = Mathf.Max(0, firstEnemyId);
        }
    }
}