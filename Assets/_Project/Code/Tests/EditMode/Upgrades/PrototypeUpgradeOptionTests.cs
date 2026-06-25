using System;
using DeepSeal.Upgrades;
using NUnit.Framework;

namespace DeepSeal.Tests.Upgrades
{
    public sealed class PrototypeUpgradeOptionTests
    {
        [Test]
        public void Constructor_CreatesInitializedOption()
        {
            var option = new PrototypeUpgradeOption(
                1,
                "Sharper Strike",
                PrototypeUpgradeEffectType.AttackDamageBonus,
                3,
                1f);

            Assert.That(option.Id, Is.EqualTo(1));
            Assert.That(option.DisplayName, Is.EqualTo("Sharper Strike"));
            Assert.That(option.EffectType, Is.EqualTo(PrototypeUpgradeEffectType.AttackDamageBonus));
            Assert.That(option.Cost, Is.EqualTo(3));
            Assert.That(option.EffectValue, Is.EqualTo(1f));
            Assert.That(option.IsInitialized, Is.True);
        }

        [Test]
        public void Constructor_UsesEffectTypeNameWhenDisplayNameIsEmpty()
        {
            var option = new PrototypeUpgradeOption(
                1,
                string.Empty,
                PrototypeUpgradeEffectType.MoveSpeedBonus,
                2,
                0.35f);

            Assert.That(option.DisplayName, Is.EqualTo("MoveSpeedBonus"));
        }

        [Test]
        public void Constructor_ThrowsForNoneEffectType()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                _ = new PrototypeUpgradeOption(
                    0,
                    "Invalid",
                    PrototypeUpgradeEffectType.None,
                    1,
                    1f);
            });
        }

        [Test]
        public void Constructor_ThrowsForNonPositiveCost()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new PrototypeUpgradeOption(
                    0,
                    "Invalid",
                    PrototypeUpgradeEffectType.AttackDamageBonus,
                    0,
                    1f);
            });
        }

        [Test]
        public void Constructor_ThrowsForInvalidEffectValue()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new PrototypeUpgradeOption(
                    0,
                    "Invalid",
                    PrototypeUpgradeEffectType.AttackDamageBonus,
                    1,
                    0f);
            });
        }
    }
}