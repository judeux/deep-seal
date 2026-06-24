using System;
using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Mining;

namespace DeepSeal.Combat
{
    /// <summary>
    /// Pure C# grid pathfinding rules for prototype enemy movement.
    /// </summary>
    public static class EnemyPathfindingRules
    {
        public static EnemyPathfindingResult FindNextStep(
            MineGrid grid,
            GridPosition startPosition,
            GridPosition targetPosition,
            int maxVisitedCells)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (maxVisitedCells <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxVisitedCells),
                    maxVisitedCells,
                    "Max visited cells must be greater than zero.");
            }

            if (!grid.Contains(startPosition) || !grid.Contains(targetPosition))
            {
                return EnemyPathfindingResult.NoPath(startPosition, targetPosition, 0);
            }

            if (startPosition == targetPosition)
            {
                return EnemyPathfindingResult.AtTarget(startPosition, targetPosition, 1);
            }

            if (!IsPassable(grid, startPosition) || !IsPassable(grid, targetPosition))
            {
                return EnemyPathfindingResult.NoPath(startPosition, targetPosition, 0);
            }

            var visited = new bool[grid.Width, grid.Height];
            var firstDirections = new GridDirection[grid.Width, grid.Height];
            var queue = new Queue<GridPosition>();
            var directionOrder = new GridDirection[4];

            visited[startPosition.X, startPosition.Y] = true;
            queue.Enqueue(startPosition);

            int visitedCellCount = 1;

            while (queue.Count > 0)
            {
                GridPosition current = queue.Dequeue();
                FillDirectionOrder(current, targetPosition, directionOrder);

                for (int i = 0; i < directionOrder.Length; i++)
                {
                    GridDirection direction = directionOrder[i];

                    if (!direction.IsCardinal())
                    {
                        continue;
                    }

                    if (visitedCellCount >= maxVisitedCells)
                    {
                        return EnemyPathfindingResult.NoPath(
                            startPosition,
                            targetPosition,
                            visitedCellCount);
                    }

                    GridPosition next = current.Offset(direction);

                    if (!grid.Contains(next)
                        || visited[next.X, next.Y]
                        || !IsPassable(grid, next))
                    {
                        continue;
                    }

                    GridDirection firstDirection = current == startPosition
                        ? direction
                        : firstDirections[current.X, current.Y];

                    if (!firstDirection.IsCardinal())
                    {
                        continue;
                    }

                    visited[next.X, next.Y] = true;
                    firstDirections[next.X, next.Y] = firstDirection;
                    visitedCellCount++;

                    if (next == targetPosition)
                    {
                        GridPosition nextPosition = startPosition.Offset(firstDirection);

                        return EnemyPathfindingResult.FoundPath(
                            startPosition,
                            targetPosition,
                            nextPosition,
                            firstDirection,
                            visitedCellCount);
                    }

                    queue.Enqueue(next);
                }
            }

            return EnemyPathfindingResult.NoPath(
                startPosition,
                targetPosition,
                visitedCellCount);
        }

        private static bool IsPassable(MineGrid grid, GridPosition position)
        {
            return grid.TryGetCell(position, out TerrainCell cell) && cell.IsPassable;
        }

        private static void FillDirectionOrder(
            GridPosition from,
            GridPosition to,
            GridDirection[] directions)
        {
            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = GridDirection.None;
            }

            int count = 0;
            AddUnique(ChoosePrimaryDirection(from, to), directions, ref count);
            AddUnique(ChooseSecondaryDirection(from, to), directions, ref count);
            AddUnique(GridDirection.Up, directions, ref count);
            AddUnique(GridDirection.Right, directions, ref count);
            AddUnique(GridDirection.Down, directions, ref count);
            AddUnique(GridDirection.Left, directions, ref count);
        }

        private static void AddUnique(
            GridDirection direction,
            GridDirection[] directions,
            ref int count)
        {
            if (!direction.IsCardinal())
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                if (directions[i] == direction)
                {
                    return;
                }
            }

            if (count < directions.Length)
            {
                directions[count] = direction;
                count++;
            }
        }

        private static GridDirection ChoosePrimaryDirection(
            GridPosition from,
            GridPosition to)
        {
            int deltaX = to.X - from.X;
            int deltaY = to.Y - from.Y;

            if (Math.Abs(deltaX) >= Math.Abs(deltaY) && deltaX != 0)
            {
                return deltaX > 0 ? GridDirection.Right : GridDirection.Left;
            }

            if (deltaY != 0)
            {
                return deltaY > 0 ? GridDirection.Up : GridDirection.Down;
            }

            return GridDirection.None;
        }

        private static GridDirection ChooseSecondaryDirection(
            GridPosition from,
            GridPosition to)
        {
            int deltaX = to.X - from.X;
            int deltaY = to.Y - from.Y;

            if (Math.Abs(deltaX) >= Math.Abs(deltaY))
            {
                if (deltaY == 0)
                {
                    return GridDirection.None;
                }

                return deltaY > 0 ? GridDirection.Up : GridDirection.Down;
            }

            if (deltaX == 0)
            {
                return GridDirection.None;
            }

            return deltaX > 0 ? GridDirection.Right : GridDirection.Left;
        }
    }
}