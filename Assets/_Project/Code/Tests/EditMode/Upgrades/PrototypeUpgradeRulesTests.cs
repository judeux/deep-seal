using System;
using DeepSeal.Upgrades;
using NUnit.Framework;

namespace DeepSeal.Tests.Upgrades
{
    public sealed class PrototypeUpgradeRulesTests
    {
        [Test]
        public void TryPurchase_SucceedsWhenEnoughRewardValueIsAvailable()
        {
            PrototypeUpgradeState state = PrototypeUpgradeState.Empty;
            PrototypeUpgradeOption option = CreateOption(cost: 3);

            PrototypeUpgradePurchaseResult result = PrototypeUpgradeRules.TryPurchase(
                state,
                option,
                collectedRewardValue: 5);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Changed, Is.True);
            Assert.That(result.SpentRewardValue, Is.EqualTo(3));
            Assert.That(result.CurrentState.SpentRewardValue, Is.EqualTo(3));
            Assert.That(result.CurrentState.PurchasedUpgradeCount, Is.EqualTo(1));
            Assert.That(result.AvailableRewardValueAfter, Is.EqualTo(2));
        }

        [Test]
        public void TryPurchase_FailsWhenRewardValueIsInsufficient()
        {
            PrototypeUpgradeState state = PrototypeUpgradeState.Empty;
            PrototypeUpgradeOption option = CreateOption(cost: 3);

            PrototypeUpgradePurchaseResult result = PrototypeUpgradeRules.TryPurchase(
                state,
                option,
                collectedRewardValue: 2);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.InsufficientReward, Is.True);
            Assert.That(result.Changed, Is.False);
            Assert.That(result.CurrentState, Is.EqualTo(state));
        }

        [Test]
        public void TryPurchase_AccountsForPreviouslySpentRewardValue()
        {
            var state = new PrototypeUpgradeState(
                spentRewardValue: 3,
                purchasedUpgradeCount: 1);

            PrototypeUpgradeOption option = CreateOption(cost: 3);

            PrototypeUpgradePurchaseResult result = PrototypeUpgradeRules.TryPurchase(
                state,
                option,
                collectedRewardValue: 5);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.AvailableRewardValueBefore, Is.EqualTo(2));
        }

        [Test]
        public void GetAvailableRewardValue_ClampsToZeroWhenSpentExceedsCollected()
        {
            var state = new PrototypeUpgradeState(
                spentRewardValue: 5,
                purchasedUpgradeCount: 2);

            int available = PrototypeUpgradeRules.GetAvailableRewardValue(
                state,
                collectedRewardValue: 3);

            Assert.That(available, Is.EqualTo(0));
        }

        [Test]
        public void GetAvailableRewardValue_ThrowsForNegativeCollectedRewardValue()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = PrototypeUpgradeRules.GetAvailableRewardValue(
                    PrototypeUpgradeState.Empty,
                    -1);
            });
        }

        [Test]
        public void PrototypeUpgradeState_SpendIncrementsSpentValueAndCount()
        {
            PrototypeUpgradeState state = PrototypeUpgradeState.Empty;

            PrototypeUpgradeState next = state.Spend(2);

            Assert.That(next.SpentRewardValue, Is.EqualTo(2));
            Assert.That(next.PurchasedUpgradeCount, Is.EqualTo(1));
        }

        private static PrototypeUpgradeOption CreateOption(int cost)
        {
            return new PrototypeUpgradeOption(
                0,
                "Test Upgrade",
                PrototypeUpgradeEffectType.AttackDamageBonus,
                cost,
                1f);
        }
    }
}