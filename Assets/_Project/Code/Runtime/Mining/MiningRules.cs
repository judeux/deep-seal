using System;
using DeepSeal.Core;

namespace DeepSeal.Mining
{
    /// <summary>
    /// Applies pure mining rules to mine grid cells.
    /// 하나의 좌표에 채굴 피해를 적용하고, 벽 내구도가 0 이하가 되면 바닥으로 전환한다.
    /// </summary>
    public static class MiningRules
    {
        public static MiningResult ApplyMiningDamage(
            MineGrid grid,
            GridPosition position,
            int damage)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (damage <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(damage),
                    damage,
                    "Mining damage must be greater than zero.");
            }

            if (!grid.TryGetCell(position, out TerrainCell previousCell))
            {
                return MiningResult.OutOfBounds(position, damage);
            }

            if (!previousCell.IsMineable)
            {
                return MiningResult.NotMineable(position, previousCell, damage);
            }

            int remainingDurability = previousCell.Durability - damage;

            if (remainingDurability <= 0)
            {
                TerrainCell floor = TerrainCell.Floor;
                grid.TrySetCell(position, floor);
                return MiningResult.Destroyed(position, previousCell, floor, damage);
            }

            TerrainCell damagedWall = TerrainCell.Wall(remainingDurability);
            grid.TrySetCell(position, damagedWall);
            return MiningResult.Damaged(position, previousCell, damagedWall, damage);
        }

    }
}
