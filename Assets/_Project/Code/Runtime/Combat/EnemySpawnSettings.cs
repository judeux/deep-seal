using System;

namespace DeepSeal.Combat
{
    /// <summary>
    /// Distance and reachability constraints for prototype enemy spawn selection.
    /// </summary>
    public readonly struct EnemySpawnSettings
    {
        public EnemySpawnSettings(
            int minimumDistanceFromTarget,
            int maximumDistanceFromTarget)
            : this(minimumDistanceFromTarget, maximumDistanceFromTarget, 512)
        {
        }

        public EnemySpawnSettings(
            int minimumDistanceFromTarget,
            int maximumDistanceFromTarget,
            int maxPathVisitedCells)
        {
            if (minimumDistanceFromTarget < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(minimumDistanceFromTarget),
                    minimumDistanceFromTarget,
                    "Minimum spawn distance must be zero or greater.");
            }

            if (maximumDistanceFromTarget < minimumDistanceFromTarget)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumDistanceFromTarget),
                    maximumDistanceFromTarget,
                    "Maximum spawn distance must be greater than or equal to minimum spawn distance.");
            }

            if (maxPathVisitedCells <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxPathVisitedCells),
                    maxPathVisitedCells,
                    "Max path visited cells must be greater than zero.");
            }

            MinimumDistanceFromTarget = minimumDistanceFromTarget;
            MaximumDistanceFromTarget = maximumDistanceFromTarget;
            MaxPathVisitedCells = maxPathVisitedCells;
        }

        public int MinimumDistanceFromTarget { get; }

        public int MaximumDistanceFromTarget { get; }

        public int MaxPathVisitedCells { get; }

        public bool AllowsDistance(int distance)
        {
            return distance >= MinimumDistanceFromTarget
                && distance <= MaximumDistanceFromTarget;
        }
    }
}