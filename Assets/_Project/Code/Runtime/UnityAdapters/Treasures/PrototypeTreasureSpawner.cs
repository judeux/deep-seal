using System;
using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Mining;
using DeepSeal.UnityAdapters.Grid;
using DeepSeal.UnityAdapters.Prototype;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Treasures
{
    /// <summary>
    /// Prototype-only Unity adapter that spawns visible treasure markers near the generated mine start position.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypeTreasureSpawner : MonoBehaviour
    {
        [Serializable]
        private struct TreasureSpawnPoint
        {
            [SerializeField] private int x;
            [SerializeField] private int y;
            [SerializeField] private int value;

            public TreasureSpawnPoint(int x, int y, int value)
            {
                this.x = x;
                this.y = y;
                this.value = value;
            }

            public int Value => value > 0 ? value : 1;

            public GridPosition ToPosition(GridPosition origin)
            {
                return origin.Offset(x, y);
            }

            public TreasureSpawnPoint Normalized()
            {
                return new TreasureSpawnPoint(x, y, Value);
            }

            public override string ToString()
            {
                return $"({x}, {y}), value={Value}";
            }
        }

        [Header("References")]
        [SerializeField] private PrototypeMineGridBootstrap mineGridBootstrap;
        [SerializeField] private PrototypeTreasureView treasurePrefab;
        [SerializeField] private Transform spawnParent;

        [Header("Spawn")]
        [SerializeField] private bool spawnOnStart = true;
        [SerializeField] private bool generateGridIfMissing = true;
        [SerializeField] private int firstTreasureId;
        [SerializeField]
        private TreasureSpawnPoint[] treasureSpawnPoints =
        {
            new TreasureSpawnPoint(1, 1, 1),
            new TreasureSpawnPoint(-1, 1, 1),
            new TreasureSpawnPoint(1, -1, 2)
        };

        [Header("Debug")]
        [SerializeField] private bool logSkippedSpawns;

        private readonly List<PrototypeTreasureView> spawnedTreasures = new List<PrototypeTreasureView>();
        private bool hasSpawned;

        public IReadOnlyList<PrototypeTreasureView> SpawnedTreasures => spawnedTreasures;

        private void Start()
        {
            if (spawnOnStart)
            {
                SpawnTreasures();
            }
        }

        [ContextMenu("Spawn Treasures")]
        public void SpawnTreasures()
        {
            if (hasSpawned)
            {
                Debug.LogWarning("Prototype treasures have already been spawned by this spawner.", this);
                return;
            }

            if (!TryResolveGrid(out MineGrid grid))
            {
                return;
            }

            if (treasurePrefab == null)
            {
                Debug.LogError("Cannot spawn prototype treasures because Treasure Prefab is not assigned.", this);
                return;
            }

            if (!mineGridBootstrap.TryGetStartPosition(out GridPosition startPosition))
            {
                Debug.LogError("Cannot spawn prototype treasures because mine start position is unavailable.", this);
                return;
            }

            if (treasureSpawnPoints == null || treasureSpawnPoints.Length == 0)
            {
                Debug.LogWarning("No prototype treasure spawn points are configured.", this);
                hasSpawned = true;
                return;
            }

            Transform parent = spawnParent != null ? spawnParent : transform;
            int spawnedCount = 0;

            for (int i = 0; i < treasureSpawnPoints.Length; i++)
            {
                TreasureSpawnPoint spawnPoint = treasureSpawnPoints[i].Normalized();
                GridPosition spawnPosition = spawnPoint.ToPosition(startPosition);

                if (!CanSpawnAt(grid, spawnPosition))
                {
                    if (logSkippedSpawns)
                    {
                        Debug.LogWarning(
                            $"Skipped prototype treasure spawn at {spawnPosition}. SpawnPoint={spawnPoint}. Cell is blocked or out of bounds.",
                            this);
                    }

                    continue;
                }

                int treasureId = firstTreasureId + spawnedCount;
                Vector3 worldPosition = GridCoordinateConverter.GridToWorldCenter(spawnPosition);

                PrototypeTreasureView treasureView = Instantiate(
                    treasurePrefab,
                    worldPosition,
                    Quaternion.identity,
                    parent);

                treasureView.name = $"PrototypeTreasure_{treasureId}";
                treasureView.Initialize(treasureId, spawnPosition, spawnPoint.Value);
                spawnedTreasures.Add(treasureView);

                spawnedCount++;
            }

            hasSpawned = true;
        }

        public void RemoveInactiveTreasureReferences()
        {
            for (int i = spawnedTreasures.Count - 1; i >= 0; i--)
            {
                PrototypeTreasureView treasure = spawnedTreasures[i];

                if (treasure == null || treasure.IsCollected || !treasure.gameObject.activeInHierarchy)
                {
                    spawnedTreasures.RemoveAt(i);
                }
            }
        }

        private bool TryResolveGrid(out MineGrid grid)
        {
            grid = null;

            if (mineGridBootstrap == null)
            {
                Debug.LogError("Cannot spawn prototype treasures because Prototype Mine Grid Bootstrap is not assigned.", this);
                return false;
            }

            if (!mineGridBootstrap.TryGetCurrentGrid(out grid) && generateGridIfMissing)
            {
                mineGridBootstrap.TryGenerateAndRender();
            }

            if (!mineGridBootstrap.TryGetCurrentGrid(out grid))
            {
                Debug.LogWarning("Cannot spawn prototype treasures because no MineGrid has been generated yet.", this);
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
            firstTreasureId = 0;
        }

        private void OnValidate()
        {
            firstTreasureId = Mathf.Max(0, firstTreasureId);

            if (treasureSpawnPoints == null)
            {
                return;
            }

            for (int i = 0; i < treasureSpawnPoints.Length; i++)
            {
                treasureSpawnPoints[i] = treasureSpawnPoints[i].Normalized();
            }
        }
    }
}