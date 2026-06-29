using System;
using DeepSeal.Core;
using DeepSeal.Mining;
using DeepSeal.ProceduralGeneration;
using NUnit.Framework;

namespace DeepSeal.Tests.ProceduralGeneration
{
    public sealed class MineTerrainPresetPlacementRulesTests
    {
        [Test]
        public void TryPlacePreset_AppliesPresetCells()
        {
            MineGenerationSettings settings = CreateSettings();
            MineGrid grid = CreateFilledCarvableGrid(settings);
            MineTerrainPreset preset = CreateCenterPillarPreset();

            bool placed = MineTerrainPresetPlacementRules.TryPlacePreset(
                grid,
                settings,
                preset,
                new GridPosition(4, 4));

            Assert.That(placed, Is.True);

            Assert.That(grid.TryGetCell(new GridPosition(5, 5), out TerrainCell centerCell), Is.True);
            Assert.That(centerCell.Type, Is.EqualTo(TerrainCellType.UnmineableWall));

            Assert.That(grid.TryGetCell(new GridPosition(5, 4), out TerrainCell northCell), Is.True);
            Assert.That(northCell.Type, Is.EqualTo(TerrainCellType.MineableWall));
        }

        [Test]
        public void TryPlacePreset_ReturnsFalseWhenPresetOverlapsStartArea()
        {
            MineGenerationSettings settings = CreateSettings();
            MineGrid grid = CreateFilledCarvableGrid(settings);
            MineTerrainPreset preset = CreateCenterPillarPreset();

            bool placed = MineTerrainPresetPlacementRules.TryPlacePreset(
                grid,
                settings,
                preset,
                new GridPosition(8, 8));

            Assert.That(placed, Is.False);
            Assert.That(grid.TryGetCell(settings.StartPosition, out TerrainCell startCell), Is.True);
            Assert.That(startCell.Type, Is.EqualTo(TerrainCellType.Floor));
        }

        [Test]
        public void TryPlacePreset_ReturnsFalseAndRollsBackWhenPlacementDisconnectsFloors()
        {
            var settings = new MineGenerationSettings(
                13,
                13,
                7,
                new GridPosition(3, 6),
                0,
                3,
                0,
                MineGenerationShapeMode.ConnectedCavern);

            var grid = new MineGrid(settings.Width, settings.Height, TerrainCell.Void);

            for (int x = 3; x <= 9; x++)
            {
                grid.TrySetCell(new GridPosition(x, 6), TerrainCell.Floor);
            }

            var blockingPreset = new MineTerrainPreset(
                "blocking-cell",
                1,
                1,
                new[]
                {
                    MineTerrainPresetCell.MineableWall
                });

            bool placed = MineTerrainPresetPlacementRules.TryPlacePreset(
                grid,
                settings,
                blockingPreset,
                new GridPosition(6, 6));

            Assert.That(placed, Is.False);
            Assert.That(grid.TryGetCell(new GridPosition(6, 6), out TerrainCell cell), Is.True);
            Assert.That(cell.Type, Is.EqualTo(TerrainCellType.Floor));
        }

        [Test]
        public void PlacePresets_ReturnsZeroWhenPlacementCountIsZero()
        {
            var settings = new MineGenerationSettings(
                13,
                13,
                8,
                new GridPosition(6, 6),
                1,
                3,
                45,
                MineGenerationShapeMode.ConnectedCavern,
                0,
                0,
                1,
                0,
                0);

            MineGrid grid = CreateFilledCarvableGrid(settings);

            int placedCount = MineTerrainPresetPlacementRules.PlacePresets(
                grid,
                settings,
                new Random(settings.Seed),
                MineTerrainPresetLibrary.CreatePrototypePresets());

            Assert.That(placedCount, Is.EqualTo(0));
        }

        [Test]
        public void PlacePresets_IsDeterministicForSameSeedAndGrid()
        {
            var settings = new MineGenerationSettings(
                15,
                15,
                9,
                new GridPosition(7, 7),
                1,
                3,
                45,
                MineGenerationShapeMode.ConnectedCavern,
                0,
                0,
                1,
                2,
                50);

            MineGrid firstGrid = CreateFilledCarvableGrid(settings);
            MineGrid secondGrid = CreateFilledCarvableGrid(settings);

            MineTerrainPreset[] presets = MineTerrainPresetLibrary.CreatePrototypePresets();

            int firstPlaced = MineTerrainPresetPlacementRules.PlacePresets(
                firstGrid,
                settings,
                new Random(settings.Seed),
                presets);

            int secondPlaced = MineTerrainPresetPlacementRules.PlacePresets(
                secondGrid,
                settings,
                new Random(settings.Seed),
                presets);

            Assert.That(firstPlaced, Is.EqualTo(secondPlaced));
            AssertLayoutsEqual(firstGrid, secondGrid);
        }

        private static MineGenerationSettings CreateSettings()
        {
            return new MineGenerationSettings(
                17,
                17,
                1,
                new GridPosition(9, 9),
                1,
                3,
                45,
                MineGenerationShapeMode.ConnectedCavern);
        }

        private static MineGrid CreateFilledCarvableGrid(MineGenerationSettings settings)
        {
            var grid = new MineGrid(settings.Width, settings.Height, TerrainCell.Void);

            for (int y = settings.CarveInset; y < settings.Height - settings.CarveInset; y++)
            {
                for (int x = settings.CarveInset; x < settings.Width - settings.CarveInset; x++)
                {
                    grid.TrySetCell(new GridPosition(x, y), TerrainCell.Floor);
                }
            }

            return grid;
        }

        private static MineTerrainPreset CreateCenterPillarPreset()
        {
            const MineTerrainPresetCell F = MineTerrainPresetCell.Floor;
            const MineTerrainPresetCell M = MineTerrainPresetCell.MineableWall;
            const MineTerrainPresetCell U = MineTerrainPresetCell.UnmineableWall;

            return new MineTerrainPreset(
                "test-center-pillar",
                3,
                3,
                new[]
                {
                    F, M, F,
                    M, U, M,
                    F, M, F
                });
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
    }
}