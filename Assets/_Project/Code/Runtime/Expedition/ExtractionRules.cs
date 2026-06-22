using System;
using DeepSeal.Core;

namespace DeepSeal.Expedition
{
    /// <summary>
    /// Pure C# extraction rules for the prototype expedition loop.
    /// </summary>
    public static class ExtractionRules
    {
        public static ExtractionResult TryExtract(
            ExtractionMarkerState marker,
            GridPosition actorPosition,
            int carriedTreasureValue,
            int requiredTreasureValue)
        {
            if (!marker.IsInitialized)
            {
                throw new ArgumentException(
                    "Extraction marker state must be initialized.",
                    nameof(marker));
            }

            if (carriedTreasureValue < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(carriedTreasureValue),
                    carriedTreasureValue,
                    "Carried treasure value must be zero or greater.");
            }

            if (requiredTreasureValue < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(requiredTreasureValue),
                    requiredTreasureValue,
                    "Required treasure value must be zero or greater.");
            }

            if (marker.IsCompleted)
            {
                return ExtractionResult.MarkerAlreadyCompleted(
                    marker,
                    actorPosition,
                    carriedTreasureValue,
                    requiredTreasureValue);
            }

            if (marker.Position != actorPosition)
            {
                return ExtractionResult.NotAtPosition(
                    marker,
                    actorPosition,
                    carriedTreasureValue,
                    requiredTreasureValue);
            }

            if (carriedTreasureValue < requiredTreasureValue)
            {
                return ExtractionResult.MissingRequiredValue(
                    marker,
                    actorPosition,
                    carriedTreasureValue,
                    requiredTreasureValue);
            }

            return ExtractionResult.Completed(
                marker,
                actorPosition,
                carriedTreasureValue,
                requiredTreasureValue);
        }
    }
}