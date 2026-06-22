using System;
using DeepSeal.Core;

namespace DeepSeal.Expedition
{
    /// <summary>
    /// Pure result value returned after attempting to pick up one treasure.
    /// </summary>
    public readonly struct TreasurePickupResult
    {
        private TreasurePickupResult(
            TreasureState previousTreasure,
            TreasureState currentTreasure,
            GridPosition collectorPosition,
            bool succeeded,
            bool alreadyCollected)
        {
            if (!previousTreasure.IsInitialized)
            {
                throw new ArgumentException(
                    "Previous treasure state must be initialized.",
                    nameof(previousTreasure));
            }

            if (!currentTreasure.IsInitialized)
            {
                throw new ArgumentException(
                    "Current treasure state must be initialized.",
                    nameof(currentTreasure));
            }

            if (previousTreasure.Id != currentTreasure.Id)
            {
                throw new ArgumentException(
                    "Current treasure state must represent the same treasure id as the previous state.",
                    nameof(currentTreasure));
            }

            PreviousTreasure = previousTreasure;
            CurrentTreasure = currentTreasure;
            CollectorPosition = collectorPosition;
            Succeeded = succeeded;
            AlreadyCollected = alreadyCollected;
        }

        public TreasureState PreviousTreasure { get; }

        public TreasureState CurrentTreasure { get; }

        public GridPosition CollectorPosition { get; }

        public bool Succeeded { get; }

        public bool AlreadyCollected { get; }

        public bool Changed => PreviousTreasure != CurrentTreasure;

        public int CollectedValue => Succeeded ? CurrentTreasure.Value : 0;

        public static TreasurePickupResult Collected(
            TreasureState previousTreasure,
            GridPosition collectorPosition)
        {
            return new TreasurePickupResult(
                previousTreasure,
                previousTreasure.Collect(),
                collectorPosition,
                true,
                false);
        }

        public static TreasurePickupResult AlreadyPickedUp(
            TreasureState treasure,
            GridPosition collectorPosition)
        {
            return new TreasurePickupResult(
                treasure,
                treasure,
                collectorPosition,
                false,
                true);
        }

        public static TreasurePickupResult NotAtPosition(
            TreasureState treasure,
            GridPosition collectorPosition)
        {
            return new TreasurePickupResult(
                treasure,
                treasure,
                collectorPosition,
                false,
                false);
        }
    }
}