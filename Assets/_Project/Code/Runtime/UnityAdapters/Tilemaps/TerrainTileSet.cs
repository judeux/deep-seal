using DeepSeal.Mining;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DeepSeal.UnityAdapters.Tilemaps
{
    /// <summary>
    /// TerrainCellType을 Unity TileBase로 매핑하는 Inspector 설정 asset.
    /// Void는 의도적으로 Tilemap에 아무 타일도 표시하지 않는다.
    /// </summary>
    [CreateAssetMenu(
        fileName = "TerrainTileSet",
        menuName = "Deep Seal/Prototype/Terrain Tile Set")]
    public sealed class TerrainTileSet : ScriptableObject
    {
        [SerializeField] private TileBase floorTile;
        [SerializeField] private TileBase wallTile;

        public TileBase FloorTile => floorTile;

        public TileBase WallTile => wallTile;

        public bool HasRequiredTiles => floorTile != null && wallTile != null;

        public bool TryGetTile(TerrainCellType cellType, out TileBase tile)
        {
            switch (cellType)
            {
                case TerrainCellType.Floor:
                    tile = floorTile;
                    return tile != null;

                case TerrainCellType.Wall:
                    tile = wallTile;
                    return tile != null;

                case TerrainCellType.Void:
                    tile = null;
                    return true;

                default:
                    tile = null;
                    return false;
            }
        }
    }
}