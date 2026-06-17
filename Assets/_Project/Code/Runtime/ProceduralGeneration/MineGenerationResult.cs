using System;
using DeepSeal.Core;
using DeepSeal.Mining;

namespace DeepSeal.ProceduralGeneration
{
    public readonly struct MineGenerationResult
    {
        public MineGenerationResult(MineGrid grid, MineGenerationSettings settings)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            settings.Validate();

            if (grid.Width != settings.Width || grid.Height != settings.Height)
            {
                throw new ArgumentException(
                    "Grid size must match generation settings.",
                    nameof(grid));
            }

            Grid = grid;
            Settings = settings;
        }

        public MineGrid Grid { get; }

        public MineGenerationSettings Settings { get; }

        public GridPosition StartPosition => Settings.StartPosition;

        public int Seed => Settings.Seed;

    }
}
