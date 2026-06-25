using System;

namespace DeepSeal.Upgrades
{
    /// <summary>
    /// Pure result returned by a prototype upgrade purchase attempt.
    /// </summary>
    public readonly struct PrototypeUpgradePurchaseResult
    {
        private PrototypeUpgradePurchaseResult(
            PrototypeUpgradeState previousState,
            PrototypeUpgradeState currentState,
            PrototypeUpgradeOption option,
            int collectedRewardValue,
            int availableRewardValueBefore,
            bool succeeded,
            bool insufficientReward)
        {
            if (!option.IsInitialized)
            {
                throw new ArgumentException("Upgrade option must be initialized.", nameof(option));
            }

            if (collectedRewardValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(collectedRewardValue), collectedRewardValue, "Collected reward value must be zero or greater.");
            }

            if (availableRewardValueBefore < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(availableRewardValueBefore), availableRewardValueBefore, "Available reward value must be zero or greater.");
            }

            PreviousState = previousState;
            CurrentState = currentState;
            Option = option;
            CollectedRewardValue = collectedRewardValue;
            AvailableRewardValueBefore = availableRewardValueBefore;
            Succeeded = succeeded;
            InsufficientReward = insufficientReward;
        }

        public PrototypeUpgradeState PreviousState { get; }

        public PrototypeUpgradeState CurrentState { get; }

        public PrototypeUpgradeOption Option { get; }

        public int CollectedRewardValue { get; }

        public int AvailableRewardValueBefore { get; }

        public bool Succeeded { get; }

        public bool InsufficientReward { get; }

        public bool Changed => PreviousState != CurrentState;

        public int SpentRewardValue => Succeeded ? Option.Cost : 0;

        public int AvailableRewardValueAfter => PrototypeUpgradeRules.GetAvailableRewardValue(
            CurrentState,
            CollectedRewardValue);

        public static PrototypeUpgradePurchaseResult Purchased(
            PrototypeUpgradeState previousState,
            PrototypeUpgradeState currentState,
            PrototypeUpgradeOption option,
            int collectedRewardValue,
            int availableRewardValueBefore)
        {
            return new PrototypeUpgradePurchaseResult(
                previousState,
                currentState,
                option,
                collectedRewardValue,
                availableRewardValueBefore,
                true,
                false);
        }

        public static PrototypeUpgradePurchaseResult Insufficient(
            PrototypeUpgradeState state,
            PrototypeUpgradeOption option,
            int collectedRewardValue,
            int availableRewardValueBefore)
        {
            return new PrototypeUpgradePurchaseResult(
                state,
                state,
                option,
                collectedRewardValue,
                availableRewardValueBefore,
                false,
                true);
        }
    }
}