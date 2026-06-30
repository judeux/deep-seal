using System;

namespace DeepSeal.Expedition
{
    /// <summary>
    /// Defines distance and reachability limits for prototype expedition object spawn selection.
    /// </summary>
    public readonly struct ExpeditionSpawnSettings
    {
        public ExpeditionSpawnSettings(
            int minimumDistanceFromOrigin,
            int maximumDistanceFromOrigin)
            : this(minimumDistanceFromOrigin, maximumDistanceFromOrigin, 512)
        {
        }

        public ExpeditionSpawnSettings(
            int minimumDistanceFromOrigin,
            int maximumDistanceFromOrigin,
            int maxPathVisitedCells)
        {
            ValidateArguments(
                minimumDistanceFromOrigin,
                maximumDistanceFromOrigin,
                maxPathVisitedCells);

            MinimumDistanceFromOrigin = minimumDistanceFromOrigin;
            MaximumDistanceFromOrigin = maximumDistanceFromOrigin;
            MaxPathVisitedCells = maxPathVisitedCells;
        }

        public int MinimumDistanceFromOrigin { get; }

        public int MaximumDistanceFromOrigin { get; }

        public int MaxPathVisitedCells { get; }

        public void Validate()
        {
            ValidateArguments(
                MinimumDistanceFromOrigin,
                MaximumDistanceFromOrigin,
                MaxPathVisitedCells);
        }

        public bool AllowsDistance(int distance)
        {
            return distance >= MinimumDistanceFromOrigin
                && distance <= MaximumDistanceFromOrigin;
        }

        private static void ValidateArguments(
            int minimumDistanceFromOrigin,
            int maximumDistanceFromOrigin,
            int maxPathVisitedCells)
        {
            if (minimumDistanceFromOrigin < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(minimumDistanceFromOrigin),
                    minimumDistanceFromOrigin,
                    "Minimum spawn distance must be zero or greater.");
            }

            if (maximumDistanceFromOrigin < minimumDistanceFromOrigin)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumDistanceFromOrigin),
                    maximumDistanceFromOrigin,
                    "Maximum spawn distance must be greater than or equal to minimum spawn distance.");
            }

            if (maxPathVisitedCells <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxPathVisitedCells),
                    maxPathVisitedCells,
                    "Max path visited cells must be greater than zero.");
            }
        }
    }
}