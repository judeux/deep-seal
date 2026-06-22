using System;
using DeepSeal.Core;
using DeepSeal.Expedition;
using NUnit.Framework;

namespace DeepSeal.Tests.Expedition
{
    public sealed class TreasurePickupRulesTests
    {
        [Test]
        public void TryPickUp_WhenCollectorIsOnTreasurePosition_CollectsTreasure()
        {
            var position = new GridPosition(2, 3);
            var treasure = new TreasureState(1, position, 5);

            TreasurePickupResult result = TreasurePickupRules.TryPickUp(treasure, position);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Changed, Is.True);
            Assert.That(result.CollectedValue, Is.EqualTo(5));
            Assert.That(result.CurrentTreasure.IsCollected, Is.True);
        }

        [Test]
        public void TryPickUp_WhenCollectorIsOnDifferentPosition_DoesNotCollectTreasure()
        {
            var treasure = new TreasureState(1, new GridPosition(2, 3), 5);

            TreasurePickupResult result = TreasurePickupRules.TryPickUp(
                treasure,
                new GridPosition(2, 4));

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.AlreadyCollected, Is.False);
            Assert.That(result.Changed, Is.False);
            Assert.That(result.CurrentTreasure.IsCollected, Is.False);
        }

        [Test]
        public void TryPickUp_WhenTreasureIsAlreadyCollected_ReturnsAlreadyCollected()
        {
            var treasure = new TreasureState(1, new GridPosition(2, 3), 5, true);

            TreasurePickupResult result = TreasurePickupRules.TryPickUp(
                treasure,
                new GridPosition(2, 3));

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.AlreadyCollected, Is.True);
            Assert.That(result.Changed, Is.False);
        }

        [Test]
        public void TryPickUp_ThrowsForDefaultTreasureState()
        {
            TreasureState treasure = default;

            Assert.Throws<ArgumentException>(() =>
            {
                _ = TreasurePickupRules.TryPickUp(treasure, GridPosition.Zero);
            });
        }
    }
}