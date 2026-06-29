namespace DeepSeal.ProceduralGeneration
{
    /// <summary>
    /// Describes one authored terrain cell inside a procedural terrain preset.
    /// </summary>
    public enum MineTerrainPresetCell
    {
        Ignore = 0,
        Floor = 1,
        MineableWall = 2,
        UnmineableWall = 3
    }
}
