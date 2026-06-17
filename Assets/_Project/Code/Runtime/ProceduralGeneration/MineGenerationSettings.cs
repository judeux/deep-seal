using System;
using DeepSeal.Core;

namespace DeepSeal.ProceduralGeneration
{
    public readonly struct MineGenerationSettings
    {
        public MineGenerationSettings(
            int width,
            int height,
            int seed,
            GridPosition startPosition,
            int startClearRadius,
            int wallDurability,
            int randomFloorPercent)
        {
            ValidateArguments(
                width,
                height,
                startPosition,
                startClearRadius,
                wallDurability,
                randomFloorPercent);

            Width = width;
            Height = height;
            Seed = seed;
            StartPosition = startPosition;
            StartClearRadius = startClearRadius;
            WallDurability = wallDurability;
            RandomFloorPercent = randomFloorPercent;
        }

        public int Width { get; }
        public int Height { get; }
        public int Seed { get; }
        public GridPosition StartPosition { get; }
        public int StartClearRadius { get; }
        public int WallDurability { get; }
        public int RandomFloorPercent { get; }

        public void Validate()
        {
            ValidateArguments(
                Width,
                Height,
                StartPosition,
                StartClearRadius,
                WallDurability,
                RandomFloorPercent);
        }

        public bool IsInStartClearArea(GridPosition position)
        {
            return Math.Abs(position.X - StartPosition.X) <= StartClearRadius
                && Math.Abs(position.Y - StartPosition.Y) <= StartClearRadius;
        }

        private static void ValidateArguments(
            int width,
            int height,
            GridPosition startPosition,
            int startClearRadius,
            int wallDurability,
            int randomFloorPercent)
        {
            if (width < 3)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(width),
                    width,
                    "Mine width must be at least 3 so an outer wall can exist.");
            }

            if (height < 3)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(height),
                    height,
                    "Mine height must be at least 3 so an outer wall can exist.");
            }

            if (startClearRadius < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(startClearRadius),
                    startClearRadius,
                    "Start clear radius cannot be negative.");
            }

            if (wallDurability <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(wallDurability),
                    wallDurability,
                    "Wall durability must be greater than zero.");
            }

            if (randomFloorPercent < 0 || randomFloorPercent > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(randomFloorPercent),
                    randomFloorPercent,
                    "Random floor percent must be between 0 and 100.");
            }

            int minStartX = 1 + startClearRadius;
            int maxStartX = width - 2 - startClearRadius;
            int minStartY = 1 + startClearRadius;
            int maxStartY = height - 2 - startClearRadius;

            if (minStartX > maxStartX || minStartY > maxStartY)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(startClearRadius),
                    startClearRadius,
                    "Start clear radius is too large for this grid size.");
            }

            if (startPosition.X < minStartX
                || startPosition.X > maxStartX
                || startPosition.Y < minStartY
                || startPosition.Y > maxStartY)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(startPosition),
                    startPosition,
                    "Start position and clear radius must stay inside the outer boundary.");
            }
        }
    }
}
