using System;
using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Mining;
using DeepSeal.ProceduralGeneration;
using NUnit.Framework;

namespace DeepSeal.Tests.ProceduralGeneration
{
    public sealed class MineGridGeneratorTests
    {
        private static readonly GridPosition[] CardinalOffsets =
        {
            new GridPosition(0, 1),
            new GridPosition(1, 0),
            new GridPosition(0, -1),
            new GridPosition(-1, 0)
        };

        [Test]
        public void Generate_ReturnsGridWithConfiguredSizeAndStartPosition()
        {
            MineGenerationSettings settings = CreateConnectedSettings(123);

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
            MineGenerationSettings settings = CreateConnectedSettings(777);

            MineGenerationResult first = MineGridGenerator.Generate(settings);
            MineGenerationResult second = MineGridGenerator.Generate(settings);

            AssertLayoutsEqual(first.Grid, second.Grid);
        }

        [Test]
        public void Generate_RandomScatterKeepsOuterBoundaryAsWalls()
        {
            var settings = new MineGenerationSettings(
                12,
                10,
                42,
                new GridPosition(6, 5),
                1,
                3,
                35,
                MineGenerationShapeMode.RandomScatter);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            for (int x = 0; x < result.Grid.Width; x++)
            {
                AssertBoundaryWall(result.Grid, new GridPosition(x, 0));
                AssertBoundaryWall(result.Grid, new GridPosition(x, result.Grid.Height - 1));
            }

            for (int y = 1; y < result.Grid.Height - 1; y++)
            {
                AssertBoundaryWall(result.Grid, new GridPosition(0, y));
                AssertBoundaryWall(result.Grid, new GridPosition(result.Grid.Width - 1, y));
            }
        }

        [Test]
        public void Generate_ClearsConfiguredStartArea()
        {
            var settings = new MineGenerationSettings(
                9,
                9,
                100,
                new GridPosition(4, 4),
                1,
                5,
                0,
                MineGenerationShapeMode.ConnectedCavern);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            for (int y = settings.StartPosition.Y - settings.StartClearRadius;
                 y <= settings.StartPosition.Y + settings.StartClearRadius;
                 y++)
            {
                for (int x = settings.StartPosition.X - settings.StartClearRadius;
                     x <= settings.StartPosition.X + settings.StartClearRadius;
                     x++)
                {
                    AssertFloor(result.Grid, new GridPosition(x, y));
                }
            }
        }

        [Test]
        public void Generate_RandomScatterWithZeroTargetKeepsNonStartInteriorAsWalls()
        {
            var settings = new MineGenerationSettings(
                9,
                9,
                200,
                new GridPosition(4, 4),
                1,
                2,
                0,
                MineGenerationShapeMode.RandomScatter);

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
        public void Generate_ConnectedCavernMeetsTargetFloorCellCount()
        {
            MineGenerationSettings settings = CreateConnectedSettings(
                seed: 300,
                targetFloorPercent: 45);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            Assert.That(CountCells(result.Grid, TerrainCellType.BoundaryWall), Is.GreaterThan(0));
        }

        [Test]
        public void Generate_ConnectedCavernKeepsAllPassableCellsConnected()
        {
            MineGenerationSettings settings = CreateConnectedSettings(
                seed: 301,
                targetFloorPercent: 50);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            AssertAllPassableCellsConnected(result.Grid, settings.StartPosition);
        }

        [Test]
        public void Generate_ConnectedCavernWithZeroTargetKeepsOnlyStartAreaPassable()
        {
            MineGenerationSettings settings = CreateConnectedSettings(
                seed: 302,
                targetFloorPercent: 0);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            Assert.That(CountPassableCells(result.Grid), Is.EqualTo(settings.StartClearCellCount));
            AssertAllPassableCellsConnected(result.Grid, settings.StartPosition);
        }

        [Test]
        public void Generate_ConnectedCavernKeepsOuterFrameAsVoid()
        {
            MineGenerationSettings settings = CreateConnectedSettings(
                seed: 42,
                targetFloorPercent: 45);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            for (int x = 0; x < result.Grid.Width; x++)
            {
                AssertVoid(result.Grid, new GridPosition(x, 0));
                AssertVoid(result.Grid, new GridPosition(x, result.Grid.Height - 1));
            }

            for (int y = 1; y < result.Grid.Height - 1; y++)
            {
                AssertVoid(result.Grid, new GridPosition(0, y));
                AssertVoid(result.Grid, new GridPosition(result.Grid.Width - 1, y));
            }
        }

        [Test]
        public void Generate_ConnectedCavernCreatesVoidOutsideVisibleFootprint()
        {
            MineGenerationSettings settings = CreateConnectedSettings(
                seed: 500,
                targetFloorPercent: 45);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            Assert.That(CountCells(result.Grid, TerrainCellType.Void), Is.GreaterThan(0));
            Assert.That(CountCells(result.Grid, TerrainCellType.Wall), Is.GreaterThan(0));
            Assert.That(CountCells(result.Grid, TerrainCellType.Floor), Is.EqualTo(settings.TargetFloorCellCount));
        }

        [Test]
        public void Generate_ConnectedCavernSeparatesFloorsFromVoidWithWalls()
        {
            MineGenerationSettings settings = CreateConnectedSettings(
                seed: 501,
                targetFloorPercent: 45);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            for (int y = 0; y < result.Grid.Height; y++)
            {
                for (int x = 0; x < result.Grid.Width; x++)
                {
                    var position = new GridPosition(x, y);

                    if (!result.Grid.TryGetCell(position, out TerrainCell cell) || !cell.IsPassable)
                    {
                        continue;
                    }

                    AssertNoCardinalVoidNeighbor(result.Grid, position);
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

        [Test]
        public void Generate_ConnectedCavernWithInternalWallPercentCreatesMineableInternalWalls()
        {
            MineGenerationSettings settings = CreateLargeConnectedSettings(
                seed: 700,
                targetFloorPercent: 45,
                internalWallPercent: 10);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            Assert.That(CountInternalMineableWalls(result.Grid), Is.GreaterThan(0));
            Assert.That(CountPassableCells(result.Grid), Is.GreaterThanOrEqualTo(settings.TargetFloorCellCount));
        }

        [Test]
        public void Generate_ConnectedCavernWithInternalWallsKeepsAllPassableCellsConnected()
        {
            MineGenerationSettings settings = CreateLargeConnectedSettings(
                seed: 701,
                targetFloorPercent: 45,
                internalWallPercent: 12);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            AssertAllPassableCellsConnected(result.Grid, settings.StartPosition);
        }

        [Test]
        public void Generate_ConnectedCavernWithInternalWallsKeepsStartAreaPassable()
        {
            MineGenerationSettings settings = CreateLargeConnectedSettings(
                seed: 702,
                targetFloorPercent: 45,
                internalWallPercent: 12);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            for (int y = settings.StartPosition.Y - settings.StartClearRadius;
                 y <= settings.StartPosition.Y + settings.StartClearRadius;
                 y++)
            {
                for (int x = settings.StartPosition.X - settings.StartClearRadius;
                     x <= settings.StartPosition.X + settings.StartClearRadius;
                     x++)
                {
                    AssertFloor(result.Grid, new GridPosition(x, y));
                }
            }
        }

        [Test]
        public void Generate_ConnectedCavernCanCreateUnmineableInternalWalls()
        {
            var settings = new MineGenerationSettings(
                24,
                18,
                710,
                new GridPosition(12, 9),
                1,
                3,
                45,
                MineGenerationShapeMode.ConnectedCavern,
                12,
                50);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            Assert.That(CountCells(result.Grid, TerrainCellType.UnmineableWall), Is.GreaterThan(0));
            AssertAllPassableCellsConnected(result.Grid, settings.StartPosition);
        }

        [Test]
        public void Generate_ConnectedCavernCreatesMineableWallRindInsideBoundaryShell()
        {
            MineGenerationSettings settings = CreateConnectedSettings(
                seed: 900,
                targetFloorPercent: 45);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            Assert.That(CountCells(result.Grid, TerrainCellType.MineableWall), Is.GreaterThan(0));
            Assert.That(CountCells(result.Grid, TerrainCellType.BoundaryWall), Is.GreaterThan(0));
        }

        [Test]
        public void Generate_ConnectedCavernDoesNotPlaceBoundaryWallDirectlyNextToFloor()
        {
            MineGenerationSettings settings = CreateConnectedSettings(
                seed: 901,
                targetFloorPercent: 45);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            for (int y = 0; y < result.Grid.Height; y++)
            {
                for (int x = 0; x < result.Grid.Width; x++)
                {
                    var position = new GridPosition(x, y);

                    if (!result.Grid.TryGetCell(position, out TerrainCell cell) || !cell.IsPassable)
                    {
                        continue;
                    }

                    AssertNoCardinalNeighborOfType(result.Grid, position, TerrainCellType.Void);
                    AssertNoCardinalNeighborOfType(result.Grid, position, TerrainCellType.BoundaryWall);
                }
            }
        }

        private static void AssertNoCardinalNeighborOfType(
            MineGrid grid,
            GridPosition position,
            TerrainCellType forbiddenType)
        {
            for (int i = 0; i < CardinalOffsets.Length; i++)
            {
                GridPosition neighbor = position + CardinalOffsets[i];

                if (grid.Contains(neighbor)
                    && grid.TryGetCell(neighbor, out TerrainCell neighborCell))
                {
                    Assert.That(
                        neighborCell.Type,
                        Is.Not.EqualTo(forbiddenType),
                        $"Cell {position} should not touch {forbiddenType} at {neighbor}.");
                }
            }
        }

        private static MineGenerationSettings CreateConnectedSettings(
            int seed,
            int targetFloorPercent = 45,
            int internalWallPercent = 0)
        {
            return new MineGenerationSettings(
                12,
                10,
                seed,
                new GridPosition(6, 5),
                1,
                3,
                targetFloorPercent,
                MineGenerationShapeMode.ConnectedCavern,
                internalWallPercent);
        }

        private static MineGenerationSettings CreateLargeConnectedSettings(
            int seed,
            int targetFloorPercent = 45,
            int internalWallPercent = 10)
        {
            return new MineGenerationSettings(
                24,
                18,
                seed,
                new GridPosition(12, 9),
                1,
                3,
                targetFloorPercent,
                MineGenerationShapeMode.ConnectedCavern,
                internalWallPercent);
        }

        private static int CountPassableCells(MineGrid grid)
        {
            int count = 0;

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    if (grid.TryGetCell(new GridPosition(x, y), out TerrainCell cell)
                        && cell.IsPassable)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private static void AssertAllPassableCellsConnected(
            MineGrid grid,
            GridPosition startPosition)
        {
            Assert.That(grid.TryGetCell(startPosition, out TerrainCell startCell), Is.True);
            Assert.That(startCell.IsPassable, Is.True);

            var visited = new bool[grid.Width, grid.Height];
            var queue = new Queue<GridPosition>();

            TryVisitPassable(grid, visited, queue, startPosition);

            while (queue.Count > 0)
            {
                GridPosition current = queue.Dequeue();

                for (int i = 0; i < CardinalOffsets.Length; i++)
                {
                    TryVisitPassable(
                        grid,
                        visited,
                        queue,
                        current + CardinalOffsets[i]);
                }
            }

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    var position = new GridPosition(x, y);

                    if (grid.TryGetCell(position, out TerrainCell cell) && cell.IsPassable)
                    {
                        Assert.That(
                            visited[x, y],
                            Is.True,
                            $"Passable cell {position} should be reachable from start.");
                    }
                }
            }
        }

        private static void TryVisitPassable(
            MineGrid grid,
            bool[,] visited,
            Queue<GridPosition> queue,
            GridPosition position)
        {
            if (!grid.Contains(position) || visited[position.X, position.Y])
            {
                return;
            }

            if (!grid.TryGetCell(position, out TerrainCell cell) || !cell.IsPassable)
            {
                return;
            }

            visited[position.X, position.Y] = true;
            queue.Enqueue(position);
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

        private static int CountCells(MineGrid grid, TerrainCellType cellType)
        {
            int count = 0;

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    if (grid.TryGetCell(new GridPosition(x, y), out TerrainCell cell)
                        && cell.Type == cellType)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private static void AssertVoid(MineGrid grid, GridPosition position)
        {
            bool found = grid.TryGetCell(position, out TerrainCell cell);

            Assert.That(found, Is.True);
            Assert.That(cell.Type, Is.EqualTo(TerrainCellType.Void));
            Assert.That(cell.IsPassable, Is.False);
            Assert.That(cell.IsMineable, Is.False);
        }

        private static void AssertNoCardinalVoidNeighbor(MineGrid grid, GridPosition position)
        {
            var offsets = new[]
            {
                new GridPosition(0, 1),
                new GridPosition(1, 0),
                new GridPosition(0, -1),
                new GridPosition(-1, 0)
            };

            for (int i = 0; i < offsets.Length; i++)
            {
                GridPosition neighbor = position + offsets[i];

                if (grid.Contains(neighbor)
                    && grid.TryGetCell(neighbor, out TerrainCell neighborCell))
                {
                    Assert.That(
                        neighborCell.Type,
                        Is.Not.EqualTo(TerrainCellType.Void),
                        $"Floor cell {position} should be separated from void at {neighbor}.");
                }
            }
        }

        private static int CountInternalMineableWalls(MineGrid grid)
        {
            int count = 0;

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    var position = new GridPosition(x, y);

                    if (grid.TryGetCell(position, out TerrainCell cell)
                        && cell.IsMineable
                        && !HasCardinalVoidNeighbor(grid, position))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private static bool HasCardinalVoidNeighbor(
            MineGrid grid,
            GridPosition position)
        {
            for (int i = 0; i < CardinalOffsets.Length; i++)
            {
                GridPosition neighbor = position + CardinalOffsets[i];

                if (grid.Contains(neighbor)
                    && grid.TryGetCell(neighbor, out TerrainCell neighborCell)
                    && neighborCell.Type == TerrainCellType.Void)
                {
                    return true;
                }
            }

            return false;
        }

        private static void AssertBoundaryWall(MineGrid grid, GridPosition position)
        {
            bool found = grid.TryGetCell(position, out TerrainCell cell);

            Assert.That(found, Is.True);
            Assert.That(cell.Type, Is.EqualTo(TerrainCellType.BoundaryWall));
            Assert.That(cell.IsWall, Is.True);
            Assert.That(cell.IsMineable, Is.False);
            Assert.That(cell.IsPassable, Is.False);
        }

        [Test]
        public void Generate_ThrowsForInvalidEdgeMineableWallThickness()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new MineGenerationSettings(
                    12,
                    10,
                    902,
                    new GridPosition(6, 5),
                    1,
                    3,
                    45,
                    MineGenerationShapeMode.ConnectedCavern,
                    0,
                    0,
                    MineGenerationSettings.MaxEdgeMineableWallThickness + 1);
            });
        }

        [Test]
        public void Generate_ConnectedCavernKeepsOnlyBoundaryWallsAdjacentToVoid()
        {
            MineGenerationSettings settings = CreateConnectedSettings(
                seed: 903,
                targetFloorPercent: 45);

            MineGenerationResult result = MineGridGenerator.Generate(settings);

            for (int y = 0; y < result.Grid.Height; y++)
            {
                for (int x = 0; x < result.Grid.Width; x++)
                {
                    var position = new GridPosition(x, y);

                    if (!result.Grid.TryGetCell(position, out TerrainCell cell)
                        || cell.Type == TerrainCellType.Void
                        || cell.Type == TerrainCellType.BoundaryWall)
                    {
                        continue;
                    }

                    AssertNoCardinalNeighborOfType(result.Grid, position, TerrainCellType.Void);
                }
            }
        }

        [Test]
        public void Generate_WithPresetPlacementProducesDeterministicLayout()
        {
            MineGenerationSettings settings = CreateLargeConnectedSettingsWithPresets(
                seed: 910,
                targetFloorPercent: 55,
                presetPlacementCount: 2);

            MineGenerationResult first = MineGridGenerator.Generate(settings);
            MineGenerationResult second = MineGridGenerator.Generate(settings);

            AssertLayoutsEqual(first.Grid, second.Grid);
        }

        [Test]
        public void Generate_WithPresetPlacementKeepsGeneratedGridValid()
        {
            MineGenerationSettings settings = CreateLargeConnectedSettingsWithPresets(
                seed: 911,
                targetFloorPercent: 55,
                presetPlacementCount: 2);

            MineGenerationResult result = MineGridGenerator.Generate(settings);
            MineGridValidationResult validationResult = MineGridValidator.Validate(result);

            Assert.That(validationResult.IsValid, Is.True);
            AssertAllPassableCellsConnected(result.Grid, settings.StartPosition);
        }

        [Test]
        public void Generate_WithPresetPlacementCanChangeTerrainLayout()
        {
            MineGenerationSettings withPresets = CreateLargeConnectedSettingsWithPresets(
                seed: 912,
                targetFloorPercent: 70,
                presetPlacementCount: 4);

            MineGenerationSettings withoutPresets = CreateLargeConnectedSettingsWithPresets(
                seed: 912,
                targetFloorPercent: 70,
                presetPlacementCount: 0);

            MineGenerationResult withPresetResult = MineGridGenerator.Generate(withPresets);
            MineGenerationResult withoutPresetResult = MineGridGenerator.Generate(withoutPresets);

            AssertLayoutsNotEqual(withoutPresetResult.Grid, withPresetResult.Grid);
        }

        private static MineGenerationSettings CreateLargeConnectedSettingsWithPresets(
            int seed,
            int targetFloorPercent = 55,
            int internalWallPercent = 0,
            int presetPlacementCount = 2)
        {
            return new MineGenerationSettings(
                24,
                18,
                seed,
                new GridPosition(12, 9),
                1,
                3,
                targetFloorPercent,
                MineGenerationShapeMode.ConnectedCavern,
                internalWallPercent,
                0,
                1,
                presetPlacementCount,
                200);
        }

        private static void AssertLayoutsNotEqual(MineGrid first, MineGrid second)
        {
            Assert.That(second.Width, Is.EqualTo(first.Width));
            Assert.That(second.Height, Is.EqualTo(first.Height));

            for (int y = 0; y < first.Height; y++)
            {
                for (int x = 0; x < first.Width; x++)
                {
                    var position = new GridPosition(x, y);

                    first.TryGetCell(position, out TerrainCell firstCell);
                    second.TryGetCell(position, out TerrainCell secondCell);

                    if (firstCell != secondCell)
                    {
                        return;
                    }
                }
            }

            Assert.Fail("Expected preset placement to change at least one terrain cell.");
        }
    }
}