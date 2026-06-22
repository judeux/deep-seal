using System;
using DeepSeal.Core;

namespace DeepSeal.Expedition
{
    /// <summary>
    /// Pure C# treasure pickup rules for the prototype expedition loop.
    /// </summary>
    public static class TreasurePickupRules
    {
        public static TreasurePickupResult TryPickUp(
            TreasureState treasure,
            GridPosition collectorPosition)
        {
            if (!treasure.IsInitialized)
            {
                throw new ArgumentException(
                    "Treasure state must be initialized.",
                    nameof(treasure));
            }

            if (treasure.IsCollected)
            {
                return TreasurePickupResult.AlreadyPickedUp(
                    treasure,
                    collectorPosition);
            }

            if (treasure.Position != collectorPosition)
            {
                return TreasurePickupResult.NotAtPosition(
                    treasure,
                    collectorPosition);
            }

            return TreasurePickupResult.Collected(
                treasure,
                collectorPosition);
        }
    }
}