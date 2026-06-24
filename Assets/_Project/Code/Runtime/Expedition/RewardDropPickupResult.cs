using System;
using DeepSeal.Core;

namespace DeepSeal.Expedition
{
    /// <summary>
    /// Pure result value returned after attempting to pick up one reward drop.
    /// </summary>
    public readonly struct RewardDropPickupResult
    {
        private RewardDropPickupResult(
            RewardDropState previousDrop,
            RewardDropState currentDrop,
            GridPosition collectorPosition,
            int pickupRangeCells,
            bool succeeded,
            bool alreadyCollected,
            bool outOfRange)
        {
            if (!previousDrop.IsInitialized)
            {
                throw new ArgumentException(
                    "Previous reward drop state must be initialized.",
                    nameof(previousDrop));
            }

            if (!currentDrop.IsInitialized)
            {
                throw new ArgumentException(
                    "Current reward drop state must be initialized.",
                    nameof(currentDrop));
            }

            if (previousDrop.Id != currentDrop.Id)
            {
                throw new ArgumentException(
                    "Current reward drop state must represent the same reward drop id as the previous state.",
                    nameof(currentDrop));
            }

            if (pickupRangeCells < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pickupRangeCells),
                    pickupRangeCells,
                    "Pickup range must be zero or greater.");
            }

            PreviousDrop = previousDrop;
            CurrentDrop = currentDrop;
            CollectorPosition = collectorPosition;
            PickupRangeCells = pickupRangeCells;
            Succeeded = succeeded;
            AlreadyCollected = alreadyCollected;
            OutOfRange = outOfRange;
        }

        public RewardDropState PreviousDrop { get; }

        public RewardDropState CurrentDrop { get; }

        public GridPosition CollectorPosition { get; }

        public int PickupRangeCells { get; }

        public bool Succeeded { get; }

        public bool AlreadyCollected { get; }

        public bool OutOfRange { get; }

        public bool Changed => PreviousDrop != CurrentDrop;

        public int CollectedValue => Succeeded ? CurrentDrop.Value : 0;

        public static RewardDropPickupResult Collected(
            RewardDropState previousDrop,
            GridPosition collectorPosition,
            int pickupRangeCells)
        {
            return new RewardDropPickupResult(
                previousDrop,
                previousDrop.Collect(),
                collectorPosition,
                pickupRangeCells,
                true,
                false,
                false);
        }

        public static RewardDropPickupResult AlreadyPickedUp(
            RewardDropState drop,
            GridPosition collectorPosition,
            int pickupRangeCells)
        {
            return new RewardDropPickupResult(
                drop,
                drop,
                collectorPosition,
                pickupRangeCells,
                false,
                true,
                false);
        }

        public static RewardDropPickupResult NotInRange(
            RewardDropState drop,
            GridPosition collectorPosition,
            int pickupRangeCells)
        {
            return new RewardDropPickupResult(
                drop,
                drop,
                collectorPosition,
                pickupRangeCells,
                false,
                false,
                true);
        }
    }
}