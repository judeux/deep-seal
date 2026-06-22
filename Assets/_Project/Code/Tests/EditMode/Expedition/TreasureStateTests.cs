using System;
using DeepSeal.Core;
using DeepSeal.Expedition;
using NUnit.Framework;

namespace DeepSeal.Tests.Expedition
{
    public sealed class TreasureStateTests
    {
        [Test]
        public void Constructor_StoresValues()
        {
            var position = new GridPosition(2, 3);

            var treasure = new TreasureState(1, position, 5);

            Assert.That(treasure.Id, Is.EqualTo(1));
            Assert.That(treasure.Position, Is.EqualTo(position));
            Assert.That(treasure.Value, Is.EqualTo(5));
            Assert.That(treasure.IsCollected, Is.False);
            Assert.That(treasure.IsInitialized, Is.True);
        }

        [Test]
        public void Collect_ReturnsCollectedState()
        {
            var treasure = new TreasureState(1, new GridPosition(2, 3), 5);

            TreasureState collected = treasure.Collect();

            Assert.That(collected.Id, Is.EqualTo(treasure.Id));
            Assert.That(collected.Position, Is.EqualTo(treasure.Position));
            Assert.That(collected.Value, Is.EqualTo(treasure.Value));
            Assert.That(collected.IsCollected, Is.True);
        }

        [Test]
        public void DefaultValue_IsNotInitialized()
        {
            TreasureState treasure = default;

            Assert.That(treasure.IsInitialized, Is.False);
        }

        [Test]
        public void Constructor_ThrowsForNegativeId()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new TreasureState(-1, GridPosition.Zero, 1);
            });
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_ThrowsForInvalidValue(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new TreasureState(1, GridPosition.Zero, value);
            });
        }

        [Test]
        public void Equality_UsesAllFields()
        {
            var a = new TreasureState(1, new GridPosition(2, 3), 5);
            var b = new TreasureState(1, new GridPosition(2, 3), 5);
            var c = new TreasureState(1, new GridPosition(2, 3), 5, true);

            Assert.That(a, Is.EqualTo(b));
            Assert.That(a == b, Is.True);
            Assert.That(a != c, Is.True);
        }
    }
}