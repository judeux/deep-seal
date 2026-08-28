using System;
using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Mining;

namespace DeepSeal.Combat
{
    /// <summary>
    /// Pure C# rules for straight cardinal projectile flights over the mine grid.
    /// The flight is decided at fire time: it stops at the first wall, the first enemy
    /// on the path, or the range limit, whichever comes first.
    /// </summary>
    public static class ProjectileAttackRules
    {
        public static GridDirection ResolveFireDirection(GridPosition origin, GridPosition target)
        {
            int deltaX = target.X - origin.X;
            int deltaY = target.Y - origin.Y;

            if (deltaX == 0 && deltaY == 0)
            {
                return GridDirection.None;
            }

            if (Math.Abs(deltaX) >= Math.Abs(deltaY))
            {
                return deltaX > 0 ? GridDirection.Right : GridDirection.Left;
            }

            return deltaY > 0 ? GridDirection.Up : GridDirection.Down;
        }

        public static ProjectileTraceResult Trace(
            MineGrid grid,
            GridPosition origin,
            GridPosition target,
            IReadOnlyList<EnemyState> enemies,
            int maxRangeCells)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (enemies == null)
            {
                throw new ArgumentNullException(nameof(enemies));
            }

            if (maxRangeCells < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxRangeCells),
                    maxRangeCells,
                    "Projectile range must be zero or greater.");
            }

            GridDirection direction = ResolveFireDirection(origin, target);

            if (direction == GridDirection.None)
            {
                return ProjectileTraceResult.NoShot();
            }

            GridPosition step = direction.ToOffset();
            GridPosition current = origin;
            int traveledCells = 0;

            while (traveledCells < maxRangeCells)
            {
                GridPosition next = current.Offset(step.X, step.Y);

                if (!grid.TryGetCell(next, out TerrainCell cell) || !cell.IsPassable)
                {
                    return new ProjectileTraceResult(true, next, traveledCells + 1, true, -1);
                }

                traveledCells++;
                current = next;

                if (TryFindEnemyAt(enemies, current, out int enemyId))
                {
                    return new ProjectileTraceResult(true, current, traveledCells, false, enemyId);
                }
            }

            return new ProjectileTraceResult(traveledCells > 0, current, traveledCells, false, -1);
        }

        private static bool TryFindEnemyAt(IReadOnlyList<EnemyState> enemies, GridPosition position, out int enemyId)
        {
            enemyId = -1;
            int bestId = int.MaxValue;
            bool found = false;

            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyState candidate = enemies[i];

                if (candidate.Position == position && candidate.Id < bestId)
                {
                    bestId = candidate.Id;
                    found = true;
                }
            }

            if (found)
            {
                enemyId = bestId;
            }

            return found;
        }
    }
}