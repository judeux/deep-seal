using System.Collections.Generic;
using DeepSeal.ProceduralGeneration;
using NUnit.Framework;

namespace DeepSeal.Tests.ProceduralGeneration
{
    public sealed class MineBiomeLibraryTests
    {
        [Test]
        public void CreatePrototypeBiomes_ReturnsFourDistinctBiomes()
        {
            IReadOnlyList<MineBiome> biomes = MineBiomeLibrary.CreatePrototypeBiomes();

            var names = new HashSet<string>();

            for (int i = 0; i < biomes.Count; i++)
            {
                names.Add(biomes[i].DisplayName);
            }

            Assert.That(biomes.Count, Is.EqualTo(4));
            Assert.That(names.Count, Is.EqualTo(4));
        }

        [Test]
        public void AllBiomes_GenerateValidGridsAcrossSeeds()
        {
            IReadOnlyList<MineBiome> biomes = MineBiomeLibrary.CreatePrototypeBiomes();

            for (int biomeIndex = 0; biomeIndex < biomes.Count; biomeIndex++)
            {
                for (int seed = 0; seed < 20; seed++)
                {
                    MineGenerationSettings settings = MineBiomeSelectionRules.CreateSettings(
                        biomes[biomeIndex],
                        seed,
                        new System.Random(seed));

                    MineGenerationResult result = MineGridGenerator.Generate(settings);
                    MineGridValidationResult validation = MineGridValidator.Validate(result);

                    Assert.That(
                        validation.IsValid,
                        Is.True,
                        $"Biome={biomes[biomeIndex].DisplayName}, Seed={seed}, Issue={validation.Issue}");
                }
            }
        }

        [Test]
        public void CreateSettings_ProducesVariedSizesAcrossSeeds()
        {
            IReadOnlyList<MineBiome> biomes = MineBiomeLibrary.CreatePrototypeBiomes();
            var widths = new HashSet<int>();

            for (int seed = 0; seed < 50; seed++)
            {
                var random = new System.Random(seed);
                MineGenerationSettings settings = MineBiomeSelectionRules.CreateSettings(biomes[0], seed, random);
                widths.Add(settings.Width);
            }

            Assert.That(widths.Count, Is.GreaterThanOrEqualTo(2));
        }
    }
}