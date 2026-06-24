using System;
using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Mining;

namespace DeepSeal.Combat
{
    /// <summary>
    /// Pure C# rules for selecting prototype enemy spawn positions on a MineGrid.
    /// </summary>
    public static class EnemySpawnRules
    {
        public static bool TryFindSpawnPosition(
            MineGrid grid,
            GridPosition targetPosition,
            IReadOnlyCollection<GridPosition> occupiedPositions,
            EnemySpawnSettings settings,
            Random random,
            out GridPosition spawnPosition)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (occupiedPositions == null)
            {
                throw new ArgumentNullException(nameof(occupiedPositions));
            }

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            spawnPosition = default;

            if (!grid.Contains(targetPosition))
            {
                return false;
            }

            var candidates = new List<GridPosition>();

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    var candidate = new GridPosition(x, y);

                    if (!IsSpawnCandidate(
                            grid,
                            candidate,
                            targetPosition,
                            occupiedPositions,
                            settings))
                    {
                        continue;
                    }

                    candidates.Add(candidate);
                }
            }

            if (candidates.Count == 0)
            {
                return false;
            }

            spawnPosition = candidates[random.Next(candidates.Count)];
            return true;
        }

        private static bool IsSpawnCandidate(
            MineGrid grid,
            GridPosition candidate,
            GridPosition targetPosition,
            IReadOnlyCollection<GridPosition> occupiedPositions,
            EnemySpawnSettings settings)
        {
            if (!grid.TryGetCell(candidate, out TerrainCell cell) || !cell.IsPassable)
            {
                return false;
            }

            int distance = AttackTargetingRules.ManhattanDistance(candidate, targetPosition);

            if (!settings.AllowsDistance(distance))
            {
                return false;
            }

            foreach (GridPosition occupiedPosition in occupiedPositions)
            {
                if (candidate == occupiedPosition)
                {
                    return false;
                }
            }

            EnemyPathfindingResult pathfindingResult = EnemyPathfindingRules.FindNextStep(
                grid,
                candidate,
                targetPosition,
                settings.MaxPathVisitedCells);

            return pathfindingResult.Found;
        }
    }
}