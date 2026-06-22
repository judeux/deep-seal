using System;
using DeepSeal.Core;
using DeepSeal.Expedition;
using NUnit.Framework;

namespace DeepSeal.Tests.Expedition
{
    public sealed class ExtractionMarkerStateTests
    {
        [Test]
        public void Constructor_StoresValues()
        {
            var position = new GridPosition(2, 3);

            var marker = new ExtractionMarkerState(1, position);

            Assert.That(marker.Id, Is.EqualTo(1));
            Assert.That(marker.Position, Is.EqualTo(position));
            Assert.That(marker.IsCompleted, Is.False);
            Assert.That(marker.IsInitialized, Is.True);
        }

        [Test]
        public void Complete_ReturnsCompletedState()
        {
            var marker = new ExtractionMarkerState(1, new GridPosition(2, 3));

            ExtractionMarkerState completed = marker.Complete();

            Assert.That(completed.Id, Is.EqualTo(marker.Id));
            Assert.That(completed.Position, Is.EqualTo(marker.Position));
            Assert.That(completed.IsCompleted, Is.True);
            Assert.That(completed.IsInitialized, Is.True);
        }

        [Test]
        public void DefaultValue_IsNotInitialized()
        {
            ExtractionMarkerState marker = default;

            Assert.That(marker.IsInitialized, Is.False);
        }

        [Test]
        public void Complete_ThrowsForDefaultValue()
        {
            ExtractionMarkerState marker = default;

            Assert.Throws<InvalidOperationException>(() =>
            {
                _ = marker.Complete();
            });
        }

        [Test]
        public void Constructor_ThrowsForNegativeId()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new ExtractionMarkerState(-1, GridPosition.Zero);
            });
        }

        [Test]
        public void Equality_UsesAllFields()
        {
            var a = new ExtractionMarkerState(1, new GridPosition(2, 3));
            var b = new ExtractionMarkerState(1, new GridPosition(2, 3));
            var c = new ExtractionMarkerState(1, new GridPosition(2, 3), true);

            Assert.That(a, Is.EqualTo(b));
            Assert.That(a == b, Is.True);
            Assert.That(a != c, Is.True);
        }
    }
}