namespace DeepSeal.Mining
{
    /// <summary>
    /// Defines the basic terrain category of a mine grid cell.
    /// </summary>
    public enum TerrainCellType
    {
        Floor = 0,

        // Compatibility alias for the first prototype wall type.
        Wall = 1,
        MineableWall = Wall,

        Void = 2,
        UnmineableWall = 3,
        BoundaryWall = 4
    }
}