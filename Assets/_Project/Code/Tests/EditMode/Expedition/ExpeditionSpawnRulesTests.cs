using System;
using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Expedition;
using DeepSeal.Mining;
using NUnit.Framework;

namespace DeepSeal.Tests.Expedition
{
    public sealed class ExpeditionSpawnRulesTests
    {
        [Test]
        public void CanSpawnAt_ReturnsTrueOnlyForPassableFloor()
        {
            var grid = new MineGrid(5, 5, TerrainCell.Void);
            var floor = new GridPosition(1, 1);
            var wall = new GridPosition(2, 1);
            var unmineable = new GridPosition(3, 1);
            var boundary = new GridPosition(1, 2);

            grid.TrySetCell(floor, TerrainCell.Floor);
            grid.TrySetCell(wall, TerrainCell.MineableWall(3));
            grid.TrySetCell(unmineable, TerrainCell.UnmineableWall);
            grid.TrySetCell(boundary, TerrainCell.BoundaryWall);

            Assert.That(ExpeditionSpawnRules.CanSpawnAt(grid, floor), Is.True);
            Assert.That(ExpeditionSpawnRules.CanSpawnAt(grid, wall), Is.False);
            Assert.That(ExpeditionSpawnRules.CanSpawnAt(grid, unmineable), Is.False);
            Assert.That(ExpeditionSpawnRules.CanSpawnAt(grid, boundary), Is.False);
            Assert.That(ExpeditionSpawnRules.CanSpawnAt(grid, new GridPosition(4, 4)), Is.False);
        }

        [Test]
        public void TryFindSpawnPosition_ReturnsReachablePassablePositionWithinDistance()
        {
            MineGrid grid = CreateOpenGrid();
            var origin = new GridPosition(2, 2);
            var occupied = new List<GridPosition> { origin };
            var settings = new ExpeditionSpawnSettings(2, 3);
            var random = new Random(123);

            bool found = ExpeditionSpawnRules.TryFindSpawnPosition(
                grid,
                origin,
                occupied,
                settings,
                random,
                out GridPosition spawnPosition);

            Assert.That(found, Is.True);
            Assert.That(ExpeditionSpawnRules.CanSpawnAt(grid, spawnPosition, occupied), Is.True);

            int distance = ExpeditionSpawnRules.ManhattanDistance(origin, spawnPosition);
            Assert.That(distance, Is.GreaterThanOrEqualTo(2));
            Assert.That(distance, Is.LessThanOrEqualTo(3));
        }

        [Test]
        public void TryFindSpawnPosition_DoesNotReturnOccupiedPosition()
        {
            var grid = new MineGrid(3, 3, TerrainCell.Void);
            var origin = new GridPosition(1, 1);
            var onlyCandidate = new GridPosition(1, 2);

            grid.TrySetCell(origin, TerrainCell.Floor);
            grid.TrySetCell(onlyCandidate, TerrainCell.Floor);

            var occupied = new List<GridPosition>
            {
                origin,
                onlyCandidate
            };

            bool found = ExpeditionSpawnRules.TryFindSpawnPosition(
                grid,
                origin,
                occupied,
                new ExpeditionSpawnSettings(1, 1),
                new Random(1),
                out _);

            Assert.That(found, Is.False);
        }

        [Test]
        public void TryFindSpawnPosition_ReturnsFalseWhenCandidateIsDisconnected()
        {
            var grid = new MineGrid(5, 3, TerrainCell.Void);
            var origin = new GridPosition(1, 1);
            var disconnected = new GridPosition(3, 1);

            grid.TrySetCell(origin, TerrainCell.Floor);
            grid.TrySetCell(disconnected, TerrainCell.Floor);

            bool found = ExpeditionSpawnRules.TryFindSpawnPosition(
                grid,
                origin,
                new List<GridPosition> { origin },
                new ExpeditionSpawnSettings(2, 2),
                new Random(1),
                out _);

            Assert.That(found, Is.False);
        }

        [Test]
        public void TryFindSpawnPosition_ThrowsForNullGrid()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _ = ExpeditionSpawnRules.TryFindSpawnPosition(
                    null,
                    GridPosition.Zero,
                    new List<GridPosition>(),
                    new ExpeditionSpawnSettings(1, 2),
                    new Random(1),
                    out _);
            });
        }

        [Test]
        public void TryFindSpawnPosition_ThrowsForNullOccupiedPositions()
        {
            MineGrid grid = CreateOpenGrid();

            Assert.Throws<ArgumentNullException>(() =>
            {
                _ = ExpeditionSpawnRules.TryFindSpawnPosition(
                    grid,
                    GridPosition.Zero,
                    null,
                    new ExpeditionSpawnSettings(1, 2),
                    new Random(1),
                    out _);
            });
        }

        [Test]
        public void TryFindSpawnPosition_ThrowsForNullRandom()
        {
            MineGrid grid = CreateOpenGrid();

            Assert.Throws<ArgumentNullException>(() =>
            {
                _ = ExpeditionSpawnRules.TryFindSpawnPosition(
                    grid,
                    GridPosition.Zero,
                    new List<GridPosition>(),
                    new ExpeditionSpawnSettings(1, 2),
                    null,
                    out _);
            });
        }

        [Test]
        public void ExpeditionSpawnSettings_ThrowsWhenDistanceRangeIsInvalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new ExpeditionSpawnSettings(3, 2);
            });
        }

        [Test]
        public void ExpeditionSpawnSettings_ThrowsWhenMaxVisitedCellsIsInvalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new ExpeditionSpawnSettings(1, 2, 0);
            });
        }

        private static MineGrid CreateOpenGrid()
        {
            return new MineGrid(5, 5, TerrainCell.Floor);
        }

        [Test]
        public void CanSpawnAt_ReturnsFalseWhenPositionIsOccupied()
        {
            var grid = new MineGrid(3, 3, TerrainCell.Floor);
            var position = new GridPosition(1, 1);
            var occupied = new List<GridPosition> { position };

            bool canSpawn = ExpeditionSpawnRules.CanSpawnAt(
                grid,
                position,
                occupied);

            Assert.That(canSpawn, Is.False);
        }
    }
}