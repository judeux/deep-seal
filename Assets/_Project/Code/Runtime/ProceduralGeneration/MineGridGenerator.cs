using System;
using DeepSeal.Core;
using DeepSeal.Mining;

namespace DeepSeal.ProceduralGeneration
{
    public static class MineGridGenerator
    {
        public static MineGenerationResult Generate(MineGenerationSettings settings)
        {
            settings.Validate();

            var random = new Random(settings.Seed);
            var grid = new MineGrid(
                settings.Width,
                settings.Height,
                TerrainCell.Wall(settings.WallDurability));

            for(int y = 1; y < settings.Height - 1; y++)
            {
                for(int x = 1; x < settings.Width - 1; x++)
                {
                    var position = new GridPosition(x, y);
                    if (settings.IsInStartClearArea(position))
                    {
                        grid.TrySetCell(position, TerrainCell.Floor);
                        continue;
                    }

                    if(random.Next(100) < settings.RandomFloorPercent)
                    {
                        grid.TrySetCell(position, TerrainCell.Floor);
                    }
                }
            }

            return new MineGenerationResult(grid, settings);
        }
    
    }
}
