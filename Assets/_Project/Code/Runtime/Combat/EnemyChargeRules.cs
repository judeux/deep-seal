using System;
using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Mining;

namespace DeepSeal.Combat
{
    /// <summary>
    /// 순수 C# 돌진 규칙. 돌진 방향은 ProjectileAttackRules와 같은 주축 판정을 따른다.
    /// 경로에는 통과 가능한 셀만 담고, 벽에 막히면 마지막 통과 셀에서 멈추고 기절한다.
    /// </summary>
    public static class EnemyChargeRules
    {
        public static EnemyChargeResult TraceCharge(
            MineGrid grid,
            GridPosition chargerPosition,
            GridPosition targetPosition,
            int maxChargeCells)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (maxChargeCells < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxChargeCells),
                    maxChargeCells,
                    "Charge cell budget must be zero or greater.");
            }

            GridDirection direction = ProjectileAttackRules.ResolveFireDirection(chargerPosition, targetPosition);

            if (direction == GridDirection.None)
            {
                return new EnemyChargeResult(
                    EnemyChargeStopReason.RangeEnd,
                    chargerPosition,
                    Array.Empty<GridPosition>(),
                    false);
            }

            GridPosition step = direction.ToOffset();
            GridPosition current = chargerPosition;
            var path = new List<GridPosition>(maxChargeCells);

            while (path.Count < maxChargeCells)
            {
                GridPosition next = current.Offset(step.X, step.Y);

                if (!grid.TryGetCell(next, out TerrainCell cell) || !cell.IsPassable)
                {
                    return new EnemyChargeResult(
                        EnemyChargeStopReason.WallHit,
                        current,
                        path.ToArray(),
                        true);
                }

                current = next;
                path.Add(current);

                if (current == targetPosition)
                {
                    return new EnemyChargeResult(
                        EnemyChargeStopReason.TargetReached,
                        current,
                        path.ToArray(),
                        false);
                }
            }

            return new EnemyChargeResult(
                EnemyChargeStopReason.RangeEnd,
                current,
                path.ToArray(),
                false);
        }
    }
}
