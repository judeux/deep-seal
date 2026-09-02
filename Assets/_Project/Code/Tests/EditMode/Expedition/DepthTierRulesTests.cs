using System;
using DeepSeal.Core;
using DeepSeal.Expedition;
using NUnit.Framework;

namespace DeepSeal.Tests.Expedition
{
    public sealed class DepthTierRulesTests
    {
        [Test]
        public void ResolveDepthTier_RampsWithDistanceFromStart()
        {
            var start = new GridPosition(10, 10);

            Assert.That(DepthTierRules.ResolveDepthTier(new GridPosition(10, 10), start, 6, 2), Is.EqualTo(0));
            Assert.That(DepthTierRules.ResolveDepthTier(new GridPosition(15, 10), start, 6, 2), Is.EqualTo(0));
            Assert.That(DepthTierRules.ResolveDepthTier(new GridPosition(16, 10), start, 6, 2), Is.EqualTo(1));
            Assert.That(DepthTierRules.ResolveDepthTier(new GridPosition(22, 10), start, 6, 2), Is.EqualTo(2));
            Assert.That(DepthTierRules.ResolveDepthTier(new GridPosition(40, 10), start, 6, 2), Is.EqualTo(2));
        }

        [Test]
        public void ResolveDepthTier_UsesManhattanDistance()
        {
            var start = new GridPosition(0, 0);

            Assert.That(DepthTierRules.ResolveDepthTier(new GridPosition(3, 3), start, 6, 2), Is.EqualTo(1));
        }

        [Test]
        public void ResolveTieredValue_AddsBonusPerTier()
        {
            Assert.That(DepthTierRules.ResolveTieredValue(2, 0, 1), Is.EqualTo(2));
            Assert.That(DepthTierRules.ResolveTieredValue(2, 2, 1), Is.EqualTo(4));
            Assert.That(DepthTierRules.ResolveTieredValue(2, 2, 3), Is.EqualTo(8));
        }

        [Test]
        public void ThrowsForInvalidArguments()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                DepthTierRules.ResolveDepthTier(GridPosition.Zero, GridPosition.Zero, 0, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                DepthTierRules.ResolveDepthTier(GridPosition.Zero, GridPosition.Zero, 6, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                DepthTierRules.ResolveTieredValue(0, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                DepthTierRules.ResolveTieredValue(1, 1, -1));
        }
    }
}
