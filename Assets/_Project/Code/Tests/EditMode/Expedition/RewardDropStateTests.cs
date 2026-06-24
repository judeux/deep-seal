using System;
using DeepSeal.Core;
using DeepSeal.Expedition;
using NUnit.Framework;

namespace DeepSeal.Tests.Expedition
{
    public sealed class RewardDropStateTests
    {
        [Test]
        public void Constructor_CreatesAvailableRewardDrop()
        {
            var drop = new RewardDropState(
                3,
                new GridPosition(2, 4),
                5,
                RewardDropSource.EnemyDefeat);

            Assert.That(drop.Id, Is.EqualTo(3));
            Assert.That(drop.Position, Is.EqualTo(new GridPosition(2, 4)));
            Assert.That(drop.Value, Is.EqualTo(5));
            Assert.That(drop.Source, Is.EqualTo(RewardDropSource.EnemyDefeat));
            Assert.That(drop.IsCollected, Is.False);
            Assert.That(drop.IsInitialized, Is.True);
        }

        [Test]
        public void Collect_ReturnsCollectedCopy()
        {
            var drop = new RewardDropState(
                1,
                GridPosition.Zero,
                2,
                RewardDropSource.Mining);

            RewardDropState collected = drop.Collect();

            Assert.That(collected.Id, Is.EqualTo(drop.Id));
            Assert.That(collected.Position, Is.EqualTo(drop.Position));
            Assert.That(collected.Value, Is.EqualTo(drop.Value));
            Assert.That(collected.Source, Is.EqualTo(drop.Source));
            Assert.That(collected.IsCollected, Is.True);
            Assert.That(drop.IsCollected, Is.False);
        }

        [Test]
        public void Constructor_ThrowsForNegativeId()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new RewardDropState(
                    -1,
                    GridPosition.Zero,
                    1,
                    RewardDropSource.Unknown);
            });
        }

        [Test]
        public void Constructor_ThrowsForNonPositiveValue()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new RewardDropState(
                    0,
                    GridPosition.Zero,
                    0,
                    RewardDropSource.Unknown);
            });
        }

        [Test]
        public void Equality_UsesAllStateFields()
        {
            var left = new RewardDropState(
                1,
                new GridPosition(2, 3),
                4,
                RewardDropSource.Mining);

            var right = new RewardDropState(
                1,
                new GridPosition(2, 3),
                4,
                RewardDropSource.Mining);

            Assert.That(left, Is.EqualTo(right));
            Assert.That(left == right, Is.True);
            Assert.That(left != right, Is.False);
        }
    }
}