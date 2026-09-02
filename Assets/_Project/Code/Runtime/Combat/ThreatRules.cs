using System;

namespace DeepSeal.Combat
{
    /// <summary>
    /// 순수 C# 위협 단계 규칙. 원정 경과 시간을 기반으로 0부터 상한까지의 위협 단계를 계산한다.
    /// 같은 경과 시간은 항상 같은 단계를 반환해야 한다.
    /// </summary>
    public static class ThreatRules
    {
        public static int ResolveThreatLevel(float elapsedSeconds, float secondsPerLevel, int maximumLevel)
        {
            if (secondsPerLevel <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(secondsPerLevel),
                    secondsPerLevel,
                    "Seconds per threat level must be greater than zero.");
            }

            if (maximumLevel < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumLevel),
                    maximumLevel,
                    "Maximum threat level cannot be negative.");
            }

            if (elapsedSeconds <= 0f)
            {
                return 0;
            }

            int level = (int)Math.Floor(elapsedSeconds / secondsPerLevel);
            return Math.Min(level, maximumLevel);
        }
    }
}
