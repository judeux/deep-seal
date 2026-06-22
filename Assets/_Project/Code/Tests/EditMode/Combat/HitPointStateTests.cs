using System;
using DeepSeal.Combat;
using NUnit.Framework;

namespace DeepSeal.Tests.Combat
{
    public sealed class HitPointStateTests
    {
        [Test]
        public void Constructor_StoresValues()
        {
            var state = new HitPointState(10, 7);

            Assert.That(state.MaxHitPoints, Is.EqualTo(10));
            Assert.That(state.CurrentHitPoints, Is.EqualTo(7));
            Assert.That(state.IsInitialized, Is.True);
            Assert.That(state.IsDefeated, Is.False);
        }

        [Test]
        public void Full_CreatesFullHealthState()
        {
            HitPointState state = HitPointState.Full(8);

            Assert.That(state.MaxHitPoints, Is.EqualTo(8));
            Assert.That(state.CurrentHitPoints, Is.EqualTo(8));
        }

        [Test]
        public void IsDefeated_ReturnsTrueWhenCurrentHitPointsAreZero()
        {
            var state = new HitPointState(5, 0);

            Assert.That(state.IsDefeated, Is.True);
        }

        [Test]
        public void WithCurrentHitPoints_ReturnsStateWithSameMax()
        {
            var state = new HitPointState(10, 10);

            HitPointState damaged = state.WithCurrentHitPoints(4);

            Assert.That(damaged.MaxHitPoints, Is.EqualTo(10));
            Assert.That(damaged.CurrentHitPoints, Is.EqualTo(4));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_ThrowsForInvalidMaxHitPoints(int maxHitPoints)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new HitPointState(maxHitPoints, 0);
            });
        }

        [TestCase(-1)]
        [TestCase(11)]
        public void Constructor_ThrowsForCurrentHitPointsOutsideRange(int currentHitPoints)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new HitPointState(10, currentHitPoints);
            });
        }

        [Test]
        public void Equality_UsesMaxAndCurrentHitPoints()
        {
            var a = new HitPointState(10, 5);
            var b = new HitPointState(10, 5);
            var c = new HitPointState(10, 4);

            Assert.That(a, Is.EqualTo(b));
            Assert.That(a == b, Is.True);
            Assert.That(a != c, Is.True);
        }
    }
}