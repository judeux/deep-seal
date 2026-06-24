using System;
using DeepSeal.Core;
using DeepSeal.Expedition;
using NUnit.Framework;

namespace DeepSeal.Tests.Expedition
{
    public sealed class RewardDropPickupRulesTests
    {
        [Test]
        public void TryPickUp_CollectsWhenCollectorIsWithinRange()
        {
            var drop = new RewardDropState(
                1,
                new GridPosition(2, 2),
                3,
                RewardDropSource.EnemyDefeat);

            RewardDropPickupResult result = RewardDropPickupRules.TryPickUp(
                drop,
                new GridPosition(3, 2),
                1);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Changed, Is.True);
            Assert.That(result.CollectedValue, Is.EqualTo(3));
            Assert.That(result.CurrentDrop.IsCollected, Is.True);
            Assert.That(result.OutOfRange, Is.False);
        }

        [Test]
        public void TryPickUp_DoesNotCollectWhenCollectorIsOutOfRange()
        {
            var drop = new RewardDropState(
                1,
                new GridPosition(2, 2),
                3,
                RewardDropSource.Mining);

            RewardDropPickupResult result = RewardDropPickupRules.TryPickUp(
                drop,
                new GridPosition(4, 2),
                1);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Changed, Is.False);
            Assert.That(result.CollectedValue, Is.EqualTo(0));
            Assert.That(result.OutOfRange, Is.True);
            Assert.That(result.CurrentDrop.IsCollected, Is.False);
        }

        [Test]
        public void TryPickUp_AllowsExactCellPickupWithZeroRange()
        {
            var drop = new RewardDropState(
                1,
                new GridPosition(2, 2),
                3,
                RewardDropSource.Mining);

            RewardDropPickupResult result = RewardDropPickupRules.TryPickUp(
                drop,
                new GridPosition(2, 2),
                0);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.CurrentDrop.IsCollected, Is.True);
        }

        [Test]
        public void TryPickUp_ReturnsAlreadyPickedUpWhenDropIsCollected()
        {
            RewardDropState drop = new RewardDropState(
                1,
                GridPosition.Zero,
                1,
                RewardDropSource.EnemyDefeat).Collect();

            RewardDropPickupResult result = RewardDropPickupRules.TryPickUp(
                drop,
                GridPosition.Zero,
                1);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.AlreadyCollected, Is.True);
            Assert.That(result.Changed, Is.False);
        }

        [Test]
        public void TryPickUp_ThrowsForUninitializedDrop()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                _ = RewardDropPickupRules.TryPickUp(
                    default,
                    GridPosition.Zero,
                    1);
            });
        }

        [Test]
        public void TryPickUp_ThrowsForNegativePickupRange()
        {
            var drop = new RewardDropState(
                1,
                GridPosition.Zero,
                1,
                RewardDropSource.Unknown);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = RewardDropPickupRules.TryPickUp(
                    drop,
                    GridPosition.Zero,
                    -1);
            });
        }
    }
}