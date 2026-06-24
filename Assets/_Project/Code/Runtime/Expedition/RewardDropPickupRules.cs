using System;
using DeepSeal.Core;

namespace DeepSeal.Expedition
{
    /// <summary>
    /// Pure C# pickup rules for prototype reward drops.
    /// </summary>
    public static class RewardDropPickupRules
    {
        public static RewardDropPickupResult TryPickUp(
            RewardDropState drop,
            GridPosition collectorPosition,
            int pickupRangeCells)
        {
            if (!drop.IsInitialized)
            {
                throw new ArgumentException(
                    "Reward drop state must be initialized.",
                    nameof(drop));
            }

            if (pickupRangeCells < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pickupRangeCells),
                    pickupRangeCells,
                    "Pickup range must be zero or greater.");
            }

            if (drop.IsCollected)
            {
                return RewardDropPickupResult.AlreadyPickedUp(
                    drop,
                    collectorPosition,
                    pickupRangeCells);
            }

            int distance = ManhattanDistance(
                drop.Position,
                collectorPosition);

            if (distance > pickupRangeCells)
            {
                return RewardDropPickupResult.NotInRange(
                    drop,
                    collectorPosition,
                    pickupRangeCells);
            }

            return RewardDropPickupResult.Collected(
                drop,
                collectorPosition,
                pickupRangeCells);
        }

        private static int ManhattanDistance(GridPosition from, GridPosition to)
        {
            return Math.Abs(to.X - from.X) + Math.Abs(to.Y - from.Y);
        }
    }
}