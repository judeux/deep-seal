using DeepSeal.Core;
using DeepSeal.Mining;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DeepSeal.UnityAdapters.Tilemaps
{
    /// <summary>
    /// 순수 도메인 MineGrid를 Unity Tilemap에 표시한다.
    /// GridPosition(x, y)를 Vector3Int(x, y, 0)에 단순 대응한다.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MineGridTilemapRenderer : MonoBehaviour
    {
        [SerializeField] private Tilemap targetTilemap;
        [SerializeField] private TerrainTileSet tileSet;
        [SerializeField] private bool clearBeforeRender = true;

        public Tilemap TargetTilemap => targetTilemap;
        public TerrainTileSet TileSet => tileSet;

        public bool ClearBeforeRender
        {
            get => clearBeforeRender;
            set => clearBeforeRender = value;
        }

        public void Render(MineGrid grid)
        {
            if(!CanRender(grid))
            {
                return;
            }

            if(clearBeforeRender)
            {
                TargetTilemap.ClearAllTiles();
            }

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    var gridPosition = new GridPosition(x, y);

                    if (!grid.TryGetCell(gridPosition, out TerrainCell cell))
                    {
                        Debug.LogWarning(
                            $"MineGrid unexpectedly failed to return a cell at {gridPosition}.",
                            this);
                        continue;
                    }

                    if (!tileSet.TryGetTile(cell.Type, out TileBase tile))
                    {
                        Debug.LogWarning(
                            $"No tile is assigned for terrain cell type '{cell.Type}' at {gridPosition}.",
                            this);
                        continue;
                    }

                    targetTilemap.SetTile(ToTilemapPosition(gridPosition), tile);
                }
            }

            targetTilemap.RefreshAllTiles();
        }

        public void Clear()
        {
            if (targetTilemap == null)
            {
                Debug.LogWarning("Cannot clear Tilemap because Target Tilemap is not assigned.", this);
                return;
            }

            targetTilemap.ClearAllTiles();
        }

        private bool CanRender(MineGrid grid)
        {
            if (grid == null)
            {
                Debug.LogError("Cannot render MineGrid because the grid is null.", this);
                return false;
            }

            if (targetTilemap == null)
            {
                Debug.LogError("Cannot render MineGrid because Target Tilemap is not assigned.", this);
                return false;
            }

            if (tileSet == null)
            {
                Debug.LogError("Cannot render MineGrid because Terrain Tile Set is not assigned.", this);
                return false;
            }

            if (!tileSet.HasRequiredTiles)
            {
                Debug.LogError(
                    "Cannot render MineGrid because Terrain Tile Set must have Floor Tile and Wall Tile assigned.",
                    tileSet);
                return false;
            }

            return true;
        }

        private static Vector3Int ToTilemapPosition(GridPosition position)
        {
            return new Vector3Int(position.X, position.Y, 0);
        }

        private void Reset()
        {
            targetTilemap = GetComponent<Tilemap>();
        }

        private void OnValidate()
        {
            if (targetTilemap == null)
            {
                targetTilemap = GetComponent<Tilemap>();
            }
        }

    }
}
