using DeepSeal.Core;
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

            if (!CanSpawnAt(grid, spawnPosition))
            {
                if (logSkippedSpawn)
                {
                    Debug.LogWarning(
                        $"Skipped prototype extraction marker spawn at {spawnPosition}. Cell is blocked or out of bounds.",
                        this);
                }

                return false;
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
            markerId = 0;
            offsetX = -1;
            offsetY = -1;
        }

        private void OnValidate()
        {
            markerId = Mathf.Max(0, markerId);
        }
    }
}