namespace DeepSeal.ProceduralGeneration
{
    /// <summary>
    /// Provides prototype terrain presets as pure C# data until asset-based authoring is justified.
    /// </summary>
    public static class MineTerrainPresetLibrary
    {
        private static readonly MineTerrainPreset[] PrototypePresets =
        {
            CreateCollapsedPillar(),
            CreateCrackedWallVein(),
            CreateSmallVaultShape()
        };

        public static MineTerrainPreset[] CreatePrototypePresets()
        {
            var copy = new MineTerrainPreset[PrototypePresets.Length];

            for (int i = 0; i < PrototypePresets.Length; i++)
            {
                copy[i] = PrototypePresets[i];
            }

            return copy;
        }

        private static MineTerrainPreset CreateCollapsedPillar()
        {
            const MineTerrainPresetCell I = MineTerrainPresetCell.Ignore;
            const MineTerrainPresetCell F = MineTerrainPresetCell.Floor;
            const MineTerrainPresetCell M = MineTerrainPresetCell.MineableWall;
            const MineTerrainPresetCell U = MineTerrainPresetCell.UnmineableWall;

            return new MineTerrainPreset(
                "collapsed-pillar",
                5,
                5,
                new[]
                {
                    I, F, F, F, I,
                    F, F, M, F, F,
                    F, M, U, M, F,
                    F, F, M, F, F,
                    I, F, F, F, I
                });
        }

        private static MineTerrainPreset CreateSmallVaultShape()
        {
            const MineTerrainPresetCell I = MineTerrainPresetCell.Ignore;
            const MineTerrainPresetCell F = MineTerrainPresetCell.Floor;
            const MineTerrainPresetCell M = MineTerrainPresetCell.MineableWall;
            const MineTerrainPresetCell U = MineTerrainPresetCell.UnmineableWall;

            return new MineTerrainPreset(
                "small-vault-shape",
                5,
                5,
                new[]
                {
                    I, M, M, M, I,
                    M, F, F, F, M,
                    M, F, U, F, M,
                    M, F, F, F, M,
                    I, M, M, M, I
                });
        }

        private static MineTerrainPreset CreateCrackedWallVein()
        {
            const MineTerrainPresetCell I = MineTerrainPresetCell.Ignore;
            const MineTerrainPresetCell F = MineTerrainPresetCell.Floor;
            const MineTerrainPresetCell M = MineTerrainPresetCell.MineableWall;
            const MineTerrainPresetCell U = MineTerrainPresetCell.UnmineableWall;

            return new MineTerrainPreset(
                "cracked-wall-vein",
                5,
                5,
                new[]
                {
            I, I, M, I, I,
            I, F, M, F, I,
            F, F, U, F, F,
            I, F, M, F, I,
            I, I, F, I, I
                });
        }
    }
}