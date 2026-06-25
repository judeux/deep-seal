using System;

namespace DeepSeal.Upgrades
{
    /// <summary>
    /// Tracks prototype upgrade spending within one run.
    /// </summary>
    public readonly struct PrototypeUpgradeState : IEquatable<PrototypeUpgradeState>
    {
        public PrototypeUpgradeState(int spentRewardValue, int purchasedUpgradeCount)
        {
            if (spentRewardValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(spentRewardValue), spentRewardValue, "Spent reward value must be zero or greater.");
            }

            if (purchasedUpgradeCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(purchasedUpgradeCount), purchasedUpgradeCount, "Purchased upgrade count must be zero or greater.");
            }

            SpentRewardValue = spentRewardValue;
            PurchasedUpgradeCount = purchasedUpgradeCount;
        }

        public int SpentRewardValue { get; }

        public int PurchasedUpgradeCount { get; }

        public static PrototypeUpgradeState Empty => new PrototypeUpgradeState(0, 0);

        public PrototypeUpgradeState Spend(int cost)
        {
            if (cost <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cost), cost, "Upgrade cost must be greater than zero.");
            }

            return new PrototypeUpgradeState(
                SpentRewardValue + cost,
                PurchasedUpgradeCount + 1);
        }

        public bool Equals(PrototypeUpgradeState other)
        {
            return SpentRewardValue == other.SpentRewardValue
                && PurchasedUpgradeCount == other.PurchasedUpgradeCount;
        }

        public override bool Equals(object obj)
        {
            return obj is PrototypeUpgradeState other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (SpentRewardValue * 397) ^ PurchasedUpgradeCount;
            }
        }

        public override string ToString()
        {
            return $"Upgrades purchased={PurchasedUpgradeCount}, spent={SpentRewardValue}";
        }

        public static bool operator ==(PrototypeUpgradeState left, PrototypeUpgradeState right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PrototypeUpgradeState left, PrototypeUpgradeState right)
        {
            return !left.Equals(right);
        }
    }
}