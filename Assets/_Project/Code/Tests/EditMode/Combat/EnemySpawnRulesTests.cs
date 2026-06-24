using System;
using System.Collections.Generic;
using DeepSeal.Combat;
using DeepSeal.Core;
using DeepSeal.Mining;
using NUnit.Framework;

namespace DeepSeal.Tests.Combat
{
    public sealed class EnemySpawnRulesTests
    {
        [Test]
        public void TryFindSpawnPosition_ReturnsPassablePositionWithinDistanceRange()
        {
            MineGrid grid = CreateOpenGrid();
            var targetPosition = new GridPosition(2, 2);
            var occupied = new List<GridPosition> { targetPosition };
            var settings = new EnemySpawnSettings(2, 4);
            var random = new Random(123);

            bool found = EnemySpawnRules.TryFindSpawnPosition(
                grid,
                targetPosition,
                occupied,
                settings,
                random,
                out GridPosition spawnPosition);

            Assert.That(found, Is.True);
            Assert.That(grid.TryGetCell(spawnPosition, out TerrainCell cell), Is.True);
            Assert.That(cell.IsPassable, Is.True);

            int distance = AttackTargetingRules.ManhattanDistance(spawnPosition, targetPosition);
            Assert.That(distance, Is.GreaterThanOrEqualTo(2));
            Assert.That(distance, Is.LessThanOrEqualTo(4));
            Assert.That(spawnPosition, Is.Not.EqualTo(targetPosition));
        }

        [Test]
        public void TryFindSpawnPosition_DoesNotReturnOccupiedPosition()
        {
            MineGrid grid = new MineGrid(3, 3, TerrainCell.Wall(2));
            var targetPosition = new GridPosition(1, 1);
            var onlySpawnCandidate = new GridPosition(1, 2);

            grid.TrySetCell(targetPosition, TerrainCell.Floor);
            grid.TrySetCell(onlySpawnCandidate, TerrainCell.Floor);

            var occupied = new List<GridPosition>
            {
                targetPosition,
                onlySpawnCandidate
            };

            bool found = EnemySpawnRules.TryFindSpawnPosition(
                grid,
                targetPosition,
                occupied,
                new EnemySpawnSettings(1, 2),
                new Random(1),
                out _);

            Assert.That(found, Is.False);
        }

        [Test]
        public void TryFindSpawnPosition_ReturnsFalseWhenAllCandidatesAreBlocked()
        {
            MineGrid grid = new MineGrid(5, 5, TerrainCell.Wall(2));
            var targetPosition = new GridPosition(2, 2);
            grid.TrySetCell(targetPosition, TerrainCell.Floor);

            bool found = EnemySpawnRules.TryFindSpawnPosition(
                grid,
                targetPosition,
                new List<GridPosition> { targetPosition },
                new EnemySpawnSettings(1, 4),
                new Random(1),
                out _);

            Assert.That(found, Is.False);
        }

        [Test]
        public void TryFindSpawnPosition_ReturnsFalseWhenTargetIsOutOfBounds()
        {
            MineGrid grid = CreateOpenGrid();

            bool found = EnemySpawnRules.TryFindSpawnPosition(
                grid,
                new GridPosition(-1, 2),
                new List<GridPosition>(),
                new EnemySpawnSettings(1, 4),
                new Random(1),
                out _);

            Assert.That(found, Is.False);
        }

        [Test]
        public void TryFindSpawnPosition_ThrowsForNullGrid()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _ = EnemySpawnRules.TryFindSpawnPosition(
                    null,
                    GridPosition.Zero,
                    new List<GridPosition>(),
                    new EnemySpawnSettings(1, 4),
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
                _ = EnemySpawnRules.TryFindSpawnPosition(
                    grid,
                    GridPosition.Zero,
                    null,
                    new EnemySpawnSettings(1, 4),
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
                _ = EnemySpawnRules.TryFindSpawnPosition(
                    grid,
                    GridPosition.Zero,
                    new List<GridPosition>(),
                    new EnemySpawnSettings(1, 4),
                    null,
                    out _);
            });
        }

        [Test]
        public void EnemySpawnSettings_ThrowsWhenDistanceRangeIsInvalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new EnemySpawnSettings(4, 3);
            });
        }

        private static MineGrid CreateOpenGrid()
        {
            return new MineGrid(5, 5, TerrainCell.Floor);
        }

        [Test]
        public void TryFindSpawnPosition_ReturnsFalseWhenCandidateCannotReachTarget()
        {
            MineGrid grid = new MineGrid(5, 3, TerrainCell.Wall(2));
            var targetPosition = new GridPosition(1, 1);
            var isolatedCandidate = new GridPosition(3, 1);

            grid.TrySetCell(targetPosition, TerrainCell.Floor);
            grid.TrySetCell(isolatedCandidate, TerrainCell.Floor);

            bool found = EnemySpawnRules.TryFindSpawnPosition(
                grid,
                targetPosition,
                new List<GridPosition> { targetPosition },
                new EnemySpawnSettings(2, 2),
                new Random(1),
                out _);

            Assert.That(found, Is.False);
        }

        [Test]
        public void EnemySpawnSettings_ThrowsWhenMaxPathVisitedCellsIsInvalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new EnemySpawnSettings(1, 4, 0);
            });
        }
    }
}