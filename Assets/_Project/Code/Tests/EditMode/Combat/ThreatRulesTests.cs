using System;
using DeepSeal.Combat;
using NUnit.Framework;

namespace DeepSeal.Tests.Combat
{
    public sealed class ThreatRulesTests
    {
        [Test]
        public void ResolveThreatLevel_RampsWithElapsedSeconds()
        {
            Assert.That(ThreatRules.ResolveThreatLevel(0f, 90f, 5), Is.EqualTo(0));
            Assert.That(ThreatRules.ResolveThreatLevel(89.9f, 90f, 5), Is.EqualTo(0));
            Assert.That(ThreatRules.ResolveThreatLevel(90f, 90f, 5), Is.EqualTo(1));
            Assert.That(ThreatRules.ResolveThreatLevel(180f, 90f, 5), Is.EqualTo(2));
        }

        [Test]
        public void ResolveThreatLevel_IsCappedAtMaximumLevel()
        {
            Assert.That(ThreatRules.ResolveThreatLevel(450f, 90f, 5), Is.EqualTo(5));
            Assert.That(ThreatRules.ResolveThreatLevel(10000f, 90f, 5), Is.EqualTo(5));
        }

        [Test]
        public void ResolveThreatLevel_ReturnsZeroForNonPositiveElapsed()
        {
            Assert.That(ThreatRules.ResolveThreatLevel(-5f, 90f, 5), Is.EqualTo(0));
        }

        [Test]
        public void ResolveThreatLevel_ThrowsForInvalidArguments()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                ThreatRules.ResolveThreatLevel(10f, 0f, 5));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                ThreatRules.ResolveThreatLevel(10f, -1f, 5));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                ThreatRules.ResolveThreatLevel(10f, 90f, -1));
        }
    }
}
