using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Expedition;
using DeepSeal.Mining;
using DeepSeal.UnityAdapters.Grid;
using DeepSeal.UnityAdapters.Prototype;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Extraction
{
    /// <summary>
    /// Prototype-only Unity adapter that spawns one visible extraction marker near the generated mine start position.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypeExtractionMarkerSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PrototypeMineGridBootstrap mineGridBootstrap;
        [SerializeField] private PrototypeExtractionMarkerView markerPrefab;
        [SerializeField] private Transform spawnParent;

        [Header("Spawn")]
        [SerializeField] private bool spawnOnStart = true;
        [SerializeField] private bool generateGridIfMissing = true;
        [SerializeField] private int markerId;
        [SerializeField] private int offsetX = -1;
        [SerializeField] private int offsetY = -1;

        [Header("Fallback Spawn Rules")]
        [SerializeField] private bool useFallbackSpawnRules = true;
        [SerializeField] private int fallbackSpawnMinDistanceFromStart = 1;
        [SerializeField] private int fallbackSpawnMaxDistanceFromStart = 6;
        [SerializeField] private int fallbackSpawnRandomSeed = 1801;

        [Header("Debug")]
        [SerializeField] private bool logSkippedSpawn;

        private PrototypeExtractionMarkerView markerView;
        private bool hasSpawned;

        public PrototypeExtractionMarkerView CurrentMarkerView => markerView;

        private void Start()
        {
            if (spawnOnStart)
            {
                TrySpawnMarker();
            }
        }

        [ContextMenu("Spawn Extraction Marker")]
        public void SpawnMarkerFromContextMenu()
        {
            _ = TrySpawnMarker();
        }

        public bool TrySpawnMarker()
        {
            if (hasSpawned)
            {
                Debug.LogWarning("Prototype extraction marker has already been spawned by this spawner.", this);
                return markerView != null;
            }

            if (!TryResolveGrid(out MineGrid grid))
            {
                return false;
            }

            if (markerPrefab == null)
            {
                Debug.LogError("Cannot spawn prototype extraction marker because Marker Prefab is not assigned.", this);
                return false;
            }

            if (!mineGridBootstrap.TryGetStartPosition(out GridPosition startPosition))
            {
                Debug.LogError("Cannot spawn prototype extraction marker because mine start position is unavailable.", this);
                return false;
            }

            GridPosition spawnPosition = startPosition.Offset(offsetX, offsetY);
            var occupiedPositions = new List<GridPosition> { startPosition };

            if (!ExpeditionSpawnRules.CanSpawnAt(grid, spawnPosition, occupiedPositions))
            {
                if (!useFallbackSpawnRules
                    || !TryFindFallbackSpawnPosition(
                        grid,
                        startPosition,
                        occupiedPositions,
                        out spawnPosition))
                {
                    if (logSkippedSpawn)
                    {
                        Debug.LogWarning(
                            $"Skipped prototype extraction marker spawn. Requested={startPosition.Offset(offsetX, offsetY)}. No valid passable fallback was found.",
                            this);
                    }

                    return false;
                }
            }

            Transform parent = spawnParent != null ? spawnParent : transform;
            Vector3 worldPosition = GridCoordinateConverter.GridToWorldCenter(spawnPosition);

            markerView = Instantiate(
                markerPrefab,
                worldPosition,
                Quaternion.identity,
                parent);

            markerView.name = $"PrototypeExtractionMarker_{markerId}";
            markerView.Initialize(markerId, spawnPosition);

            hasSpawned = true;
            return true;
        }

        public bool TryGetCurrentMarkerView(out PrototypeExtractionMarkerView view)
        {
            view = markerView;
            return markerView != null && markerView.isActiveAndEnabled;
        }

        private bool TryResolveGrid(out MineGrid grid)
        {
            grid = null;

            if (mineGridBootstrap == null)
            {
                Debug.LogError("Cannot spawn prototype extraction marker because Prototype Mine Grid Bootstrap is not assigned.", this);
                return false;
            }

            if (!mineGridBootstrap.TryGetCurrentGrid(out grid) && generateGridIfMissing)
            {
                mineGridBootstrap.TryGenerateAndRender();
            }

            if (!mineGridBootstrap.TryGetCurrentGrid(out grid))
            {
                Debug.LogWarning("Cannot spawn prototype extraction marker because no MineGrid has been generated yet.", this);
                return false;
            }

            return true;
        }

        private bool TryFindFallbackSpawnPosition(
            MineGrid grid,
            GridPosition startPosition,
            IReadOnlyCollection<GridPosition> occupiedPositions,
            out GridPosition spawnPosition)
        {
            var settings = new ExpeditionSpawnSettings(
                fallbackSpawnMinDistanceFromStart,
                fallbackSpawnMaxDistanceFromStart);

            return ExpeditionSpawnRules.TryFindSpawnPosition(
                grid,
                startPosition,
                occupiedPositions,
                settings,
                new System.Random(fallbackSpawnRandomSeed),
                out spawnPosition);
        }

        private void Reset()
        {
            spawnParent = transform;
            spawnOnStart = true;
            generateGridIfMissing = true;
            markerId = 0;
            offsetX = -1;
            offsetY = -1;
            useFallbackSpawnRules = true;
            fallbackSpawnMinDistanceFromStart = 1;
            fallbackSpawnMaxDistanceFromStart = 6;
            fallbackSpawnRandomSeed = 1801;
        }

        private void OnValidate()
        {
            markerId = Mathf.Max(0, markerId);
            fallbackSpawnMinDistanceFromStart = Mathf.Max(0, fallbackSpawnMinDistanceFromStart);
            fallbackSpawnMaxDistanceFromStart = Mathf.Max(
                fallbackSpawnMinDistanceFromStart,
                fallbackSpawnMaxDistanceFromStart);
            fallbackSpawnRandomSeed = Mathf.Max(0, fallbackSpawnRandomSeed);
        }
    }
}