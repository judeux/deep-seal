using System;
using DeepSeal.Core;
using DeepSeal.Mining;
using DeepSeal.ProceduralGeneration;
using NUnit.Framework;

namespace DeepSeal.Tests.ProceduralGeneration
{
    public sealed class MineGridGeneratorTests
    {
        [Test]
        public void Generate_ReturnsGridWithConfiguredSizeAndStartPosition()
        {
            MineGenerationSettings settings = CreateSettings(123);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            Assert.That(result.Grid, Is.Not.Null);
            Assert.That(result.Grid.Width, Is.EqualTo(settings.Width));
            Assert.That(result.Grid.Height, Is.EqualTo(settings.Height));
            Assert.That(result.StartPosition, Is.EqualTo(settings.StartPosition));
            Assert.That(result.Seed, Is.EqualTo(settings.Seed));
        }

        [Test]
        public void Generate_SameSeedAndSettingsProducesSameLayout()
        {
            MineGenerationSettings settings = CreateSettings(777);

            MineGenerationResult first = MineGridGenerator.Generate(settings);
            MineGenerationResult second = MineGridGenerator.Generate(settings);

            AssertLayoutsEqual(first.Grid, second.Grid);
        }

        [Test]
        public void Generate_KeepsOuterBoundaryAsWalls()
        {
            MineGenerationSettings settings = CreateSettings(42);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            for (int x = 0; x < result.Grid.Width; x++)
            {
                AssertWall(result.Grid, new GridPosition(x, 0), settings.WallDurability);
                AssertWall(result.Grid, new GridPosition(x, result.Grid.Height - 1), settings.WallDurability);
            }

            for (int y = 1; y < result.Grid.Height - 1; y++)
            {
                AssertWall(result.Grid, new GridPosition(0, y), settings.WallDurability);
                AssertWall(result.Grid, new GridPosition(result.Grid.Width - 1, y), settings.WallDurability);
            }
        }

        [Test]
        public void Generate_ClearsConfiguredStartArea()
        {
            var settings = new MineGenerationSettings(
                7,
                7,
                100,
                new GridPosition(3, 3),
                1,
                5,
                0);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            for (int y = 2; y <= 4; y++)
            {
                for (int x = 2; x <= 4; x++)
                {
                    AssertFloor(result.Grid, new GridPosition(x, y));
                }
            }
        }

        [Test]
        public void Generate_WithZeroRandomFloorPercentKeepsNonStartInteriorAsWalls()
        {
            var settings = new MineGenerationSettings(
                7,
                7,
                200,
                new GridPosition(3, 3),
                1,
                2,
                0);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            for (int y = 1; y < result.Grid.Height - 1; y++)
            {
                for (int x = 1; x < result.Grid.Width - 1; x++)
                {
                    var position = new GridPosition(x, y);

                    if (settings.IsInStartClearArea(position))
                    {
                        AssertFloor(result.Grid, position);
                    }
                    else
                    {
                        AssertWall(result.Grid, position, settings.WallDurability);
                    }
                }
            }
        }

        [Test]
        public void Generate_ThrowsForDefaultSettings()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = MineGridGenerator.Generate(default(MineGenerationSettings));
            });
        }

        private static MineGenerationSettings CreateSettings(int seed)
        {
            return new MineGenerationSettings(
                7,
                7,
                seed,
                new GridPosition(3, 3),
                1,
                3,
                35);
        }

        private static void AssertLayoutsEqual(MineGrid expected, MineGrid actual)
        {
            Assert.That(actual.Width, Is.EqualTo(expected.Width));
            Assert.That(actual.Height, Is.EqualTo(expected.Height));

            for (int y = 0; y < expected.Height; y++)
            {
                for (int x = 0; x < expected.Width; x++)
                {
                    var position = new GridPosition(x, y);

                    expected.TryGetCell(position, out TerrainCell expectedCell);
                    actual.TryGetCell(position, out TerrainCell actualCell);

                    Assert.That(actualCell, Is.EqualTo(expectedCell), $"Cell {position} should match.");
                }
            }
        }

        private static void AssertFloor(MineGrid grid, GridPosition position)
        {
            bool found = grid.TryGetCell(position, out TerrainCell cell);

            Assert.That(found, Is.True);
            Assert.That(cell.Type, Is.EqualTo(TerrainCellType.Floor));
            Assert.That(cell.IsPassable, Is.True);
        }

        private static void AssertWall(
            MineGrid grid,
            GridPosition position,
            int expectedDurability)
        {
            bool found = grid.TryGetCell(position, out TerrainCell cell);

            Assert.That(found, Is.True);
            Assert.That(cell.Type, Is.EqualTo(TerrainCellType.Wall));
            Assert.That(cell.Durability, Is.EqualTo(expectedDurability));
            Assert.That(cell.IsPassable, Is.False);
        }
    }
}