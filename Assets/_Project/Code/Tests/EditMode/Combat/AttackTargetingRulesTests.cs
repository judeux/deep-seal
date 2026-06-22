using System;
using DeepSeal.Combat;
using DeepSeal.Core;
using NUnit.Framework;

namespace DeepSeal.Tests.Combat
{
    public sealed class AttackTargetingRulesTests
    {
        [Test]
        public void TryFindNearestTarget_ReturnsFalseForEmptyList()
        {
            EnemyState[] enemies = Array.Empty<EnemyState>();

            bool found = AttackTargetingRules.TryFindNearestTarget(
                GridPosition.Zero,
                enemies,
                3,
                out _);

            Assert.That(found, Is.False);
        }

        [Test]
        public void TryFindNearestTarget_ChoosesNearestEnemyInsideRange()
        {
            var enemies = new[]
            {
                new EnemyState(1, new GridPosition(4, 0)),
                new EnemyState(2, new GridPosition(1, 1)),
                new EnemyState(3, new GridPosition(2, 0))
            };

            bool found = AttackTargetingRules.TryFindNearestTarget(
                GridPosition.Zero,
                enemies,
                5,
                out EnemyState target);

            Assert.That(found, Is.True);
            Assert.That(target.Id, Is.EqualTo(2));
            Assert.That(target.Position, Is.EqualTo(new GridPosition(1, 1)));
        }

        [Test]
        public void TryFindNearestTarget_IgnoresEnemiesOutsideRange()
        {
            var enemies = new[]
            {
                new EnemyState(1, new GridPosition(5, 0)),
                new EnemyState(2, new GridPosition(0, 4))
            };

            bool found = AttackTargetingRules.TryFindNearestTarget(
                GridPosition.Zero,
                enemies,
                3,
                out _);

            Assert.That(found, Is.False);
        }

        [Test]
        public void TryFindNearestTarget_UsesLowestIdAsTieBreaker()
        {
            var enemies = new[]
            {
                new EnemyState(4, new GridPosition(2, 0)),
                new EnemyState(2, new GridPosition(0, 2)),
                new EnemyState(3, new GridPosition(-2, 0))
            };

            bool found = AttackTargetingRules.TryFindNearestTarget(
                GridPosition.Zero,
                enemies,
                2,
                out EnemyState target);

            Assert.That(found, Is.True);
            Assert.That(target.Id, Is.EqualTo(2));
        }

        [Test]
        public void TryFindNearestTarget_AllowsZeroRangeForSameCell()
        {
            var enemies = new[]
            {
                new EnemyState(1, new GridPosition(1, 0)),
                new EnemyState(2, GridPosition.Zero)
            };

            bool found = AttackTargetingRules.TryFindNearestTarget(
                GridPosition.Zero,
                enemies,
                0,
                out EnemyState target);

            Assert.That(found, Is.True);
            Assert.That(target.Id, Is.EqualTo(2));
        }

        [Test]
        public void TryFindNearestTarget_ThrowsForNullEnemies()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _ = AttackTargetingRules.TryFindNearestTarget(
                    GridPosition.Zero,
                    null,
                    3,
                    out _);
            });
        }

        [Test]
        public void TryFindNearestTarget_ThrowsForNegativeRange()
        {
            EnemyState[] enemies = Array.Empty<EnemyState>();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = AttackTargetingRules.TryFindNearestTarget(
                    GridPosition.Zero,
                    enemies,
                    -1,
                    out _);
            });
        }

        [Test]
        public void ManhattanDistance_ReturnsGridDistance()
        {
            int distance = AttackTargetingRules.ManhattanDistance(
                new GridPosition(-1, 2),
                new GridPosition(3, -1));

            Assert.That(distance, Is.EqualTo(7));
        }
    }
}