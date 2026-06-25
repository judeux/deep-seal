using System;

namespace DeepSeal.Upgrades
{
    /// <summary>
    /// Pure C# purchase rules for temporary prototype upgrades.
    /// </summary>
    public static class PrototypeUpgradeRules
    {
        public static int GetAvailableRewardValue(
            PrototypeUpgradeState state,
            int collectedRewardValue)
        {
            if (collectedRewardValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(collectedRewardValue), collectedRewardValue, "Collected reward value must be zero or greater.");
            }

            int availableRewardValue = collectedRewardValue - state.SpentRewardValue;
            return Math.Max(0, availableRewardValue);
        }

        public static PrototypeUpgradePurchaseResult TryPurchase(
            PrototypeUpgradeState state,
            PrototypeUpgradeOption option,
            int collectedRewardValue)
        {
            if (!option.IsInitialized)
            {
                throw new ArgumentException("Upgrade option must be initialized.", nameof(option));
            }

            int availableRewardValue = GetAvailableRewardValue(state, collectedRewardValue);

            if (availableRewardValue < option.Cost)
            {
                return PrototypeUpgradePurchaseResult.Insufficient(
                    state,
                    option,
                    collectedRewardValue,
                    availableRewardValue);
            }

            PrototypeUpgradeState nextState = state.Spend(option.Cost);

            return PrototypeUpgradePurchaseResult.Purchased(
                state,
                nextState,
                option,
                collectedRewardValue,
                availableRewardValue);
        }
    }
}