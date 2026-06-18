using DeepSeal.Mining;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DeepSeal.UnityAdapters.Tilemaps
{
    /// <summary>
    /// TerrainCellType을 Unity TileBase로 매핑하는 Inspector 설정 asset.
    /// Floor/Wall 타일을 Unity Editor에서 할당할 수 있게 한다.
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

                default:
                    tile = null;
                    return false;
            }
        }
    
    }
}
