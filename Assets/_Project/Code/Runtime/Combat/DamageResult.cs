using System;

namespace DeepSeal.Combat
{
    /// <summary>
    /// Pure result value returned after applying combat damage.
    /// </summary>
    public readonly struct DamageResult
    {
        public DamageResult(
            HitPointState previous,
            HitPointState current,
            int damage)
        {
            if (!previous.IsInitialized)
            {
                throw new ArgumentException("Previous hit point state must be initialized.", nameof(previous));
            }

            if (!current.IsInitialized)
            {
                throw new ArgumentException("Current hit point state must be initialized.", nameof(current));
            }

            if (previous.MaxHitPoints != current.MaxHitPoints)
            {
                throw new ArgumentException(
                    "Current hit point state must use the same max hit points as the previous state.",
                    nameof(current));
            }

            if (damage <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(damage),
                    damage,
                    "Damage must be greater than zero.");
            }

            Previous = previous;
            Current = current;
            Damage = damage;
        }

        public HitPointState Previous { get; }

        public HitPointState Current { get; }

        public int Damage { get; }

        public bool Changed => Previous.CurrentHitPoints != Current.CurrentHitPoints;

        public bool TargetWasAlreadyDefeated => Previous.IsDefeated;

        public bool DefeatedThisHit => !Previous.IsDefeated && Current.IsDefeated;
    }
}