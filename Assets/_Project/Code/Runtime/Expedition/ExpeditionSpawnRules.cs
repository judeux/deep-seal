using System;
using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Mining;

namespace DeepSeal.Expedition
{
    /// <summary>
    /// Pure C# rules for selecting prototype expedition object spawn positions on a MineGrid.
    /// </summary>
    public static class ExpeditionSpawnRules
    {
        private static readonly GridPosition[] CardinalOffsets =
        {
            new GridPosition(0, 1),
            new GridPosition(1, 0),
            new GridPosition(0, -1),
            new GridPosition(-1, 0)
        };

        public static bool CanSpawnAt(
            MineGrid grid,
            GridPosition position)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            return IsPassable(grid, position);
        }

        public static bool CanSpawnAt(
            MineGrid grid,
            GridPosition position,
            IReadOnlyCollection<GridPosition> occupiedPositions)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (occupiedPositions == null)
            {
                throw new ArgumentNullException(nameof(occupiedPositions));
            }

            return IsPassable(grid, position)
                && !IsOccupied(position, occupiedPositions);
        }

        public static bool TryFindSpawnPosition(
            MineGrid grid,
            GridPosition originPosition,
            IReadOnlyCollection<GridPosition> occupiedPositions,
            ExpeditionSpawnSettings settings,
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

            settings.Validate();
            spawnPosition = default;

            if (!IsPassable(grid, originPosition))
            {
                return false;
            }

            bool[,] reachable = BuildReachableMap(
                grid,
                originPosition,
                settings.MaxPathVisitedCells);

            var candidates = new List<GridPosition>();

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    var candidate = new GridPosition(x, y);

                    if (!IsSpawnCandidate(
                            grid,
                            candidate,
                            originPosition,
                            occupiedPositions,
                            settings,
                            reachable))
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

        public static int ManhattanDistance(GridPosition from, GridPosition to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        private static bool IsSpawnCandidate(
            MineGrid grid,
            GridPosition candidate,
            GridPosition originPosition,
            IReadOnlyCollection<GridPosition> occupiedPositions,
            ExpeditionSpawnSettings settings,
            bool[,] reachable)
        {
            if (!IsPassable(grid, candidate))
            {
                return false;
            }

            if (!reachable[candidate.X, candidate.Y])
            {
                return false;
            }

            if (IsOccupied(candidate, occupiedPositions))
            {
                return false;
            }

            int distance = ManhattanDistance(candidate, originPosition);
            return settings.AllowsDistance(distance);
        }

        private static bool[,] BuildReachableMap(
            MineGrid grid,
            GridPosition originPosition,
            int maxVisitedCells)
        {
            var reachable = new bool[grid.Width, grid.Height];

            if (!IsPassable(grid, originPosition))
            {
                return reachable;
            }

            var queue = new Queue<GridPosition>();
            reachable[originPosition.X, originPosition.Y] = true;
            queue.Enqueue(originPosition);

            int visitedCount = 0;

            while (queue.Count > 0 && visitedCount < maxVisitedCells)
            {
                GridPosition current = queue.Dequeue();
                visitedCount++;

                for (int i = 0; i < CardinalOffsets.Length; i++)
                {
                    GridPosition next = current + CardinalOffsets[i];

                    if (!grid.Contains(next)
                        || reachable[next.X, next.Y]
                        || !IsPassable(grid, next))
                    {
                        continue;
                    }

                    reachable[next.X, next.Y] = true;
                    queue.Enqueue(next);
                }
            }

            return reachable;
        }

        private static bool IsPassable(MineGrid grid, GridPosition position)
        {
            return grid.TryGetCell(position, out TerrainCell cell)
                && cell.IsPassable;
        }

        private static bool IsOccupied(
            GridPosition position,
            IReadOnlyCollection<GridPosition> occupiedPositions)
        {
            foreach (GridPosition occupiedPosition in occupiedPositions)
            {
                if (position == occupiedPosition)
                {
                    return true;
                }
            }

            return false;
        }
    }
}