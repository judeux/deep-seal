using System;
using DeepSeal.Core;
using DeepSeal.Mining;

namespace DeepSeal.Combat
{
    /// <summary>
    /// Pure C# movement rules for prototype enemies on a MineGrid.
    /// </summary>
    public static class EnemyMovementRules
    {
        public static EnemyMoveResult TryMoveToward(
            MineGrid grid,
            EnemyState enemy,
            GridPosition targetPosition)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (!grid.Contains(enemy.Position))
            {
                return EnemyMoveResult.EnemyOutOfBounds(enemy);
            }

            if (!grid.Contains(targetPosition))
            {
                return EnemyMoveResult.DestinationOutOfBounds(
                    enemy,
                    targetPosition,
                    GridDirection.None);
            }

            if (enemy.Position == targetPosition)
            {
                return EnemyMoveResult.AlreadyAtTarget(enemy);
            }

            GridDirection primaryDirection = ChoosePrimaryDirection(enemy.Position, targetPosition);
            GridDirection secondaryDirection = ChooseSecondaryDirection(enemy.Position, targetPosition);

            EnemyMoveResult primaryResult = TryMoveOneCell(grid, enemy, primaryDirection);

            if (primaryResult.Moved || secondaryDirection == GridDirection.None)
            {
                return primaryResult;
            }

            EnemyMoveResult secondaryResult = TryMoveOneCell(grid, enemy, secondaryDirection);

            return secondaryResult.Moved ? secondaryResult : primaryResult;
        }

        public static EnemyMoveResult TryMoveOneCell(
            MineGrid grid,
            EnemyState enemy,
            GridDirection direction)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (!direction.IsCardinal())
            {
                return EnemyMoveResult.InvalidDirection(enemy, enemy.Position, direction);
            }

            if (!grid.Contains(enemy.Position))
            {
                return EnemyMoveResult.EnemyOutOfBounds(enemy);
            }

            GridPosition attemptedPosition = enemy.Position.Offset(direction);

            if (!grid.Contains(attemptedPosition))
            {
                return EnemyMoveResult.DestinationOutOfBounds(
                    enemy,
                    attemptedPosition,
                    direction);
            }

            if (!grid.TryGetCell(attemptedPosition, out TerrainCell cell) || !cell.IsPassable)
            {
                return EnemyMoveResult.Blocked(enemy, attemptedPosition, direction);
            }

            EnemyState movedEnemy = enemy.WithPosition(attemptedPosition);

            return EnemyMoveResult.MovedTo(
                enemy,
                movedEnemy,
                attemptedPosition,
                direction);
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

            return deltaY > 0 ? GridDirection.Up : GridDirection.Down;
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