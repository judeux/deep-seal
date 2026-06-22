using System;
using DeepSeal.Core;
using DeepSeal.Expedition;
using NUnit.Framework;

namespace DeepSeal.Tests.Expedition
{
    public sealed class ExtractionRulesTests
    {
        [Test]
        public void TryExtract_WhenActorIsAtMarkerWithEnoughTreasure_CompletesExtraction()
        {
            var position = new GridPosition(2, 3);
            var marker = new ExtractionMarkerState(1, position);

            ExtractionResult result = ExtractionRules.TryExtract(
                marker,
                position,
                carriedTreasureValue: 2,
                requiredTreasureValue: 1);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Changed, Is.True);
            Assert.That(result.CurrentMarker.IsCompleted, Is.True);
            Assert.That(result.CarriedTreasureValue, Is.EqualTo(2));
            Assert.That(result.RequiredTreasureValue, Is.EqualTo(1));
        }

        [Test]
        public void TryExtract_WhenActorIsNotAtMarker_DoesNotCompleteExtraction()
        {
            var marker = new ExtractionMarkerState(1, new GridPosition(2, 3));

            ExtractionResult result = ExtractionRules.TryExtract(
                marker,
                new GridPosition(2, 4),
                carriedTreasureValue: 2,
                requiredTreasureValue: 1);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.NotAtMarker, Is.True);
            Assert.That(result.Changed, Is.False);
            Assert.That(result.CurrentMarker.IsCompleted, Is.False);
        }

        [Test]
        public void TryExtract_WhenTreasureValueIsTooLow_DoesNotCompleteExtraction()
        {
            var position = new GridPosition(2, 3);
            var marker = new ExtractionMarkerState(1, position);

            ExtractionResult result = ExtractionRules.TryExtract(
                marker,
                position,
                carriedTreasureValue: 0,
                requiredTreasureValue: 1);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.MissingRequiredTreasure, Is.True);
            Assert.That(result.Changed, Is.False);
            Assert.That(result.CurrentMarker.IsCompleted, Is.False);
        }

        [Test]
        public void TryExtract_WhenMarkerIsAlreadyCompleted_ReturnsAlreadyCompleted()
        {
            var position = new GridPosition(2, 3);
            var marker = new ExtractionMarkerState(1, position, true);

            ExtractionResult result = ExtractionRules.TryExtract(
                marker,
                position,
                carriedTreasureValue: 2,
                requiredTreasureValue: 1);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.AlreadyCompleted, Is.True);
            Assert.That(result.Changed, Is.False);
        }

        [Test]
        public void TryExtract_ThrowsForDefaultMarkerState()
        {
            ExtractionMarkerState marker = default;

            Assert.Throws<ArgumentException>(() =>
            {
                _ = ExtractionRules.TryExtract(
                    marker,
                    GridPosition.Zero,
                    carriedTreasureValue: 1,
                    requiredTreasureValue: 1);
            });
        }

        [Test]
        public void TryExtract_ThrowsForNegativeCarriedTreasureValue()
        {
            var marker = new ExtractionMarkerState(1, GridPosition.Zero);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = ExtractionRules.TryExtract(
                    marker,
                    GridPosition.Zero,
                    carriedTreasureValue: -1,
                    requiredTreasureValue: 1);
            });
        }

        [Test]
        public void TryExtract_ThrowsForNegativeRequiredTreasureValue()
        {
            var marker = new ExtractionMarkerState(1, GridPosition.Zero);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = ExtractionRules.TryExtract(
                    marker,
                    GridPosition.Zero,
                    carriedTreasureValue: 1,
                    requiredTreasureValue: -1);
            });
        }
    }
}