using System;

namespace DeepSeal.Combat
{
    /// <summary>
    /// Pure hit point value used by prototype combat health rules.
    /// </summary>
    public readonly struct HitPointState : IEquatable<HitPointState>
    {
        public HitPointState(int maxHitPoints, int currentHitPoints)
        {
            if (maxHitPoints <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxHitPoints),
                    maxHitPoints,
                    "Max hit points must be greater than zero.");
            }

            if (currentHitPoints < 0 || currentHitPoints > maxHitPoints)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(currentHitPoints),
                    currentHitPoints,
                    "Current hit points must be between zero and max hit points.");
            }

            MaxHitPoints = maxHitPoints;
            CurrentHitPoints = currentHitPoints;
        }

        public int MaxHitPoints { get; }

        public int CurrentHitPoints { get; }

        public bool IsInitialized => MaxHitPoints > 0;

        public bool IsDefeated => IsInitialized && CurrentHitPoints <= 0;

        public static HitPointState Full(int maxHitPoints)
        {
            return new HitPointState(maxHitPoints, maxHitPoints);
        }

        public HitPointState WithCurrentHitPoints(int currentHitPoints)
        {
            return new HitPointState(MaxHitPoints, currentHitPoints);
        }

        public bool Equals(HitPointState other)
        {
            return MaxHitPoints == other.MaxHitPoints
                && CurrentHitPoints == other.CurrentHitPoints;
        }

        public override bool Equals(object obj)
        {
            return obj is HitPointState other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (MaxHitPoints * 397) ^ CurrentHitPoints;
            }
        }

        public override string ToString()
        {
            return $"{CurrentHitPoints}/{MaxHitPoints} HP";
        }

        public static bool operator ==(HitPointState left, HitPointState right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HitPointState left, HitPointState right)
        {
            return !left.Equals(right);
        }
    }
}
