using System;
using DeepSeal.Combat;
using NUnit.Framework;

namespace DeepSeal.Tests.Combat
{
    public sealed class HealthRulesTests
    {
        [Test]
        public void ApplyDamage_ReducesCurrentHitPoints()
        {
            var state = new HitPointState(10, 8);

            DamageResult result = HealthRules.ApplyDamage(state, 3);

            Assert.That(result.Previous, Is.EqualTo(state));
            Assert.That(result.Current.CurrentHitPoints, Is.EqualTo(5));
            Assert.That(result.Damage, Is.EqualTo(3));
            Assert.That(result.Changed, Is.True);
            Assert.That(result.DefeatedThisHit, Is.False);
        }

        [Test]
        public void ApplyDamage_FloorsCurrentHitPointsAtZero()
        {
            var state = new HitPointState(10, 2);

            DamageResult result = HealthRules.ApplyDamage(state, 5);

            Assert.That(result.Current.CurrentHitPoints, Is.EqualTo(0));
            Assert.That(result.Current.IsDefeated, Is.True);
        }

        [Test]
        public void ApplyDamage_WhenDamageReachesZero_MarksDefeatedThisHit()
        {
            var state = new HitPointState(10, 3);

            DamageResult result = HealthRules.ApplyDamage(state, 3);

            Assert.That(result.DefeatedThisHit, Is.True);
            Assert.That(result.TargetWasAlreadyDefeated, Is.False);
        }

        [Test]
        public void ApplyDamage_OnAlreadyDefeatedTarget_DoesNotChangeState()
        {
            var state = new HitPointState(10, 0);

            DamageResult result = HealthRules.ApplyDamage(state, 3);

            Assert.That(result.Current, Is.EqualTo(state));
            Assert.That(result.Changed, Is.False);
            Assert.That(result.DefeatedThisHit, Is.False);
            Assert.That(result.TargetWasAlreadyDefeated, Is.True);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void ApplyDamage_ThrowsForInvalidDamage(int damage)
        {
            var state = HitPointState.Full(10);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = HealthRules.ApplyDamage(state, damage);
            });
        }

        [Test]
        public void ApplyDamage_ThrowsForDefaultHitPointState()
        {
            HitPointState state = default;

            Assert.Throws<ArgumentException>(() =>
            {
                _ = HealthRules.ApplyDamage(state, 1);
            });
        }
    }
}