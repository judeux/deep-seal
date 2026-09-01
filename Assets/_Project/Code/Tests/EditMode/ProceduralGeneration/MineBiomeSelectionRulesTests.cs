using System;
using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.ProceduralGeneration;
using NUnit.Framework;

namespace DeepSeal.Tests.ProceduralGeneration
{
    public sealed class MineBiomeSelectionRulesTests
    {
        [Test]
        public void CreateSettings_IsDeterministicPerSeed()
        {
            IReadOnlyList<MineBiome> biomes = MineBiomeLibrary.CreatePrototypeBiomes();

            MineGenerationSettings first = MineBiomeSelectionRules.CreateSettings(biomes, 7, out MineBiome firstBiome);
            MineGenerationSettings second = MineBiomeSelectionRules.CreateSettings(biomes, 7, out MineBiome secondBiome);

            Assert.That(second.Seed, Is.EqualTo(first.Seed));
            Assert.That(second.Width, Is.EqualTo(first.Width));
            Assert.That(second.Height, Is.EqualTo(first.Height));
            Assert.That(second.StartPosition, Is.EqualTo(first.StartPosition));
            Assert.That(second.TargetFloorPercent, Is.EqualTo(first.TargetFloorPercent));
            Assert.That(secondBiome.DisplayName, Is.EqualTo(firstBiome.DisplayName));
        }

        [Test]
        public void CreateSettings_UsesSelectedBiomeRanges()
        {
            IReadOnlyList<MineBiome> biomes = MineBiomeLibrary.CreatePrototypeBiomes();

            for (int seed = 0; seed < 64; seed++)
            {
                MineGenerationSettings settings = MineBiomeSelectionRules.CreateSettings(biomes, seed, out MineBiome biome);

                Assert.That(settings.Width, Is.InRange(biome.MinWidth, biome.MaxWidth));
                Assert.That(settings.Height, Is.InRange(biome.MinHeight, biome.MaxHeight));
                Assert.That(settings.ShapeMode, Is.EqualTo(MineGenerationShapeMode.ConnectedCavern));
                Assert.That(settings.StartPosition, Is.EqualTo(new GridPosition(settings.Width / 2, settings.Height / 2)));
            }
        }

        [Test]
        public void CreateSettings_SelectsDifferentBiomesAcrossSeeds()
        {
            var names = new HashSet<string>();

            for (int seed = 0; seed < 64; seed++)
            {
                MineBiomeSelectionRules.CreateSettings(MineBiomeLibrary.CreatePrototypeBiomes(), seed, out MineBiome biome);
                names.Add(biome.DisplayName);
            }

            Assert.That(names.Count, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void SelectBiome_ThrowsForInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() =>
                MineBiomeSelectionRules.SelectBiome(null, new Random(1)));
            Assert.Throws<ArgumentNullException>(() =>
                MineBiomeSelectionRules.SelectBiome(MineBiomeLibrary.CreatePrototypeBiomes(), null));
            Assert.Throws<ArgumentException>(() =>
                MineBiomeSelectionRules.SelectBiome(new List<MineBiome>(), new Random(1)));
        }

        [Test]
        public void CreateSettings_ThrowsForInvalidBiomeList()
        {
            Assert.Throws<ArgumentNullException>(() =>
                MineBiomeSelectionRules.CreateSettings(null, 1, out _));
            Assert.Throws<ArgumentException>(() =>
                MineBiomeSelectionRules.CreateSettings(new List<MineBiome>(), 1, out _));
        }
    }
}