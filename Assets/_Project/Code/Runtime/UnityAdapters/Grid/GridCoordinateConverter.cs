using DeepSeal.Core;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Grid
{
    /// <summary>
    /// Converts between DeepSeal grid coordinates and Unity world coordinates.
    /// Prototype rule:
    /// GridPosition(x, y) maps to Tilemap cell (x, y, 0).
    /// The world-space center of that cell is (x + 0.5, y + 0.5, z).
    /// This assumes the prototype Grid/Tilemap transform is at origin and cell size is 1.
    /// </summary>
    public static class GridCoordinateConverter
    {
        public static Vector3 GridToWorldCenter(GridPosition position, float z = 0f)
        {
            return new Vector3(position.X + 0.5f, position.Y + 0.5f, z);
        }

        public static GridPosition WorldToGridPosition(Vector3 worldPosition)
        {
            return new GridPosition(
                Mathf.FloorToInt(worldPosition.x),
                Mathf.FloorToInt(worldPosition.y));
        }
    
    }
}
