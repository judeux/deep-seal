using System;
using DeepSeal.Core;

namespace DeepSeal.Expedition
{
    /// <summary>
    /// 순수 C# 심도 티어 규칙. 시작점에서의 맨해튼 거리로 보상 티어를 계산한다.
    /// 깊이 갈수록 보상이 커지는 구배를 만들어 탐험 동기를 부여한다.
    /// </summary>
    public static class DepthTierRules
    {
        public static int ResolveDepthTier(
            GridPosition position,
            GridPosition startPosition,
            int tierDistanceCells,
            int maximumTier)
        {
            if (tierDistanceCells <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(tierDistanceCells),
                    tierDistanceCells,
                    "Tier distance must be greater than zero.");
            }

            if (maximumTier < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumTier),
                    maximumTier,
                    "Maximum tier cannot be negative.");
            }

            int distance = Math.Abs(position.X - startPosition.X) + Math.Abs(position.Y - startPosition.Y);
            return Math.Min(distance / tierDistanceCells, maximumTier);
        }

        public static int ResolveTieredValue(int baseValue, int depthTier, int bonusPerTier)
        {
            if (baseValue <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(baseValue),
                    baseValue,
                    "Base value must be greater than zero.");
            }

            if (bonusPerTier < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(bonusPerTier),
                    bonusPerTier,
                    "Bonus per tier cannot be negative.");
            }

            return baseValue + depthTier * bonusPerTier;
        }
    }
}
