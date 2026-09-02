using System;
using DeepSeal.Core;
using DeepSeal.Mining;

namespace DeepSeal.Combat
{
    /// <summary>
    /// 순수 C# 원거리 적 규칙. 주축 시야 판정과 거리 밴드 이동 방향을 제공한다.
    /// 사이가 벽으로 막히거나 어긋나 있으면 발사할 수 없어 지형이 카운터플레이가 된다.
    /// </summary>
    public static class EnemyRangedRules
    {
        public static bool HasClearCardinalLine(MineGrid grid, GridPosition from, GridPosition to)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            GridDirection direction = ProjectileAttackRules.ResolveFireDirection(from, to);

            if (direction == GridDirection.None)
            {
                return false;
            }

            GridPosition step = direction.ToOffset();
            GridPosition current = from;

            while (true)
            {
                if (current == to)
                {
                    return true;
                }

                GridPosition next = current.Offset(step.X, step.Y);

                if (!grid.TryGetCell(next, out TerrainCell cell) || !cell.IsPassable)
                {
                    return false;
                }

                bool overshootsHorizontally = step.X > 0 && next.X > to.X || step.X < 0 && next.X < to.X;
                bool overshootsVertically = step.Y > 0 && next.Y > to.Y || step.Y < 0 && next.Y < to.Y;

                if (overshootsHorizontally || overshootsVertically)
                {
                    return false;
                }

                current = next;
            }
        }

        public static GridDirection ResolveBandStepDirection(
            GridPosition enemyPosition,
            GridPosition targetPosition,
            int minimumRangeCells,
            int maximumRangeCells)
        {
            if (minimumRangeCells < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(minimumRangeCells),
                    minimumRangeCells,
                    "Minimum range cannot be negative.");
            }

            if (maximumRangeCells < minimumRangeCells)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumRangeCells),
                    maximumRangeCells,
                    "Maximum range must be greater than or equal to the minimum range.");
            }

            int distance = AttackTargetingRules.ManhattanDistance(enemyPosition, targetPosition);

            if (distance > maximumRangeCells)
            {
                return ProjectileAttackRules.ResolveFireDirection(enemyPosition, targetPosition);
            }

            if (distance < minimumRangeCells)
            {
                return ProjectileAttackRules.ResolveFireDirection(enemyPosition, targetPosition).Opposite();
            }

            return GridDirection.None;
        }
    }
}
