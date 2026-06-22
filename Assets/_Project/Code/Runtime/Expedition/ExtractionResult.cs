using System;
using DeepSeal.Core;

namespace DeepSeal.Expedition
{
    /// <summary>
    /// Pure result value returned after attempting to complete extraction.
    /// </summary>
    public readonly struct ExtractionResult
    {
        private ExtractionResult(
            ExtractionMarkerState previousMarker,
            ExtractionMarkerState currentMarker,
            GridPosition actorPosition,
            int carriedTreasureValue,
            int requiredTreasureValue,
            bool succeeded,
            bool alreadyCompleted,
            bool notAtMarker,
            bool missingRequiredTreasure)
        {
            if (!previousMarker.IsInitialized)
            {
                throw new ArgumentException(
                    "Previous extraction marker state must be initialized.",
                    nameof(previousMarker));
            }

            if (!currentMarker.IsInitialized)
            {
                throw new ArgumentException(
                    "Current extraction marker state must be initialized.",
                    nameof(currentMarker));
            }

            if (previousMarker.Id != currentMarker.Id)
            {
                throw new ArgumentException(
                    "Current extraction marker state must represent the same marker id as the previous state.",
                    nameof(currentMarker));
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

            PreviousMarker = previousMarker;
            CurrentMarker = currentMarker;
            ActorPosition = actorPosition;
            CarriedTreasureValue = carriedTreasureValue;
            RequiredTreasureValue = requiredTreasureValue;
            Succeeded = succeeded;
            AlreadyCompleted = alreadyCompleted;
            NotAtMarker = notAtMarker;
            MissingRequiredTreasure = missingRequiredTreasure;
        }

        public ExtractionMarkerState PreviousMarker { get; }

        public ExtractionMarkerState CurrentMarker { get; }

        public GridPosition ActorPosition { get; }

        public int CarriedTreasureValue { get; }

        public int RequiredTreasureValue { get; }

        public bool Succeeded { get; }

        public bool AlreadyCompleted { get; }

        public bool NotAtMarker { get; }

        public bool MissingRequiredTreasure { get; }

        public bool Changed => PreviousMarker != CurrentMarker;

        public static ExtractionResult Completed(
            ExtractionMarkerState previousMarker,
            GridPosition actorPosition,
            int carriedTreasureValue,
            int requiredTreasureValue)
        {
            return new ExtractionResult(
                previousMarker,
                previousMarker.Complete(),
                actorPosition,
                carriedTreasureValue,
                requiredTreasureValue,
                true,
                false,
                false,
                false);
        }

    public static ExtractionResult MarkerAlreadyCompleted(
    ExtractionMarkerState marker,
    GridPosition actorPosition,
    int carriedTreasureValue,
    int requiredTreasureValue)
        {
            return new ExtractionResult(
                marker,
                marker,
                actorPosition,
                carriedTreasureValue,
                requiredTreasureValue,
                false,
                true,
                false,
                false);
        }

        public static ExtractionResult NotAtPosition(
            ExtractionMarkerState marker,
            GridPosition actorPosition,
            int carriedTreasureValue,
            int requiredTreasureValue)
        {
            return new ExtractionResult(
                marker,
                marker,
                actorPosition,
                carriedTreasureValue,
                requiredTreasureValue,
                false,
                false,
                true,
                false);
        }

        public static ExtractionResult MissingRequiredValue(
            ExtractionMarkerState marker,
            GridPosition actorPosition,
            int carriedTreasureValue,
            int requiredTreasureValue)
        {
            return new ExtractionResult(
                marker,
                marker,
                actorPosition,
                carriedTreasureValue,
                requiredTreasureValue,
                false,
                false,
                false,
                true);
        }
    }
}