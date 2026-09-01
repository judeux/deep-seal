using System;
using System.Collections.Generic;
using DeepSeal.Core;

namespace DeepSeal.ProceduralGeneration
{
    /// <summary>
    /// 시드 하나로 바이옴을 선택하고 MineGenerationSettings를 만드는 순수 규칙.
    /// 같은 시드는 항상 같은 바이옴과 같은 설정을 만들어야 한다.
    /// 시작 위치는 항상 격자 중앙으로 정해 수동 좌표 의존을 없앤다.
    /// </summary>
    public static class MineBiomeSelectionRules
    {
        public static MineBiome SelectBiome(IReadOnlyList<MineBiome> biomes, Random random)
        {
            if (biomes == null)
            {
                throw new ArgumentNullException(nameof(biomes));
            }

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (biomes.Count == 0)
            {
                throw new ArgumentException("At least one biome is required.", nameof(biomes));
            }

            return biomes[random.Next(biomes.Count)];
        }

        public static MineGenerationSettings CreateSettings(MineBiome biome, int seed, Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            int width = random.Next(biome.MinWidth, biome.MaxWidth + 1);
            int height = random.Next(biome.MinHeight, biome.MaxHeight + 1);
            var startPosition = new GridPosition(width / 2, height / 2);

            return new MineGenerationSettings(
                width,
                height,
                seed,
                startPosition,
                biome.StartClearRadius,
                biome.WallDurability,
                biome.TargetFloorPercent,
                MineGenerationShapeMode.ConnectedCavern,
                biome.InternalWallPercent,
                biome.InternalUnmineableWallPercent,
                biome.EdgeMineableWallThickness,
                biome.PresetPlacementCount,
                biome.PresetPlacementAttempts);
        }

        public static MineGenerationSettings CreateSettings(
            IReadOnlyList<MineBiome> biomes,
            int seed,
            out MineBiome selectedBiome)
        {
            if (biomes == null)
            {
                throw new ArgumentNullException(nameof(biomes));
            }

            if (biomes.Count == 0)
            {
                throw new ArgumentException("At least one biome is required.", nameof(biomes));
            }

            var random = new Random(seed);
            selectedBiome = SelectBiome(biomes, random);
            return CreateSettings(selectedBiome, seed, random);
        }
    }
}