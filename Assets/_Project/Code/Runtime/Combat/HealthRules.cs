using System;

namespace DeepSeal.Combat
{
    /// <summary>
    /// Pure C# health and damage rules for prototype combat.
    /// </summary>
    public static class HealthRules
    {
        public static DamageResult ApplyDamage(HitPointState state, int damage)
        {
            if (!state.IsInitialized)
            {
                throw new ArgumentException("Hit point state must be initialized.", nameof(state));
            }

            if (damage <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(damage),
                    damage,
                    "Damage must be greater than zero.");
            }

            if (state.IsDefeated)
            {
                return new DamageResult(state, state, damage);
            }

            int nextHitPoints = Math.Max(0, state.CurrentHitPoints - damage);
            HitPointState nextState = state.WithCurrentHitPoints(nextHitPoints);

            return new DamageResult(state, nextState, damage);
        }
    }
}