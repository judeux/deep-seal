using DeepSeal.Combat;
using DeepSeal.Core;
using DeepSeal.Mining;
using NUnit.Framework;

namespace DeepSeal.Tests.Combat
{
    public sealed class EnemyPathfindingRulesTests
    {
        [Test]
        public void FindNextStep_RoutesAroundBlockedDirectPath()
        {
            MineGrid grid = CreateOpenGrid(6, 4);
            grid.TrySetCell(new GridPosition(2, 1), TerrainCell.Wall(2));

            EnemyPathfindingResult result = EnemyPathfindingRules.FindNextStep(
                grid,
                new GridPosition(1, 1),
                new GridPosition(4, 1),
                64);

            Assert.That(result.Found, Is.True);
            Assert.That(result.CanMove, Is.True);
            Assert.That(result.Direction, Is.EqualTo(GridDirection.Up));
            Assert.That(result.NextPosition, Is.EqualTo(new GridPosition(1, 2)));
        }

        [Test]
        public void FindNextStep_ReturnsAtTargetWhenStartEqualsTarget()
        {
            MineGrid grid = CreateOpenGrid(5, 5);
            var position = new GridPosition(2, 2);

            EnemyPathfindingResult result = EnemyPathfindingRules.FindNextStep(
                grid,
                position,
                position,
                64);

            Assert.That(result.Found, Is.True);
            Assert.That(result.AlreadyAtTarget, Is.True);
            Assert.That(result.CanMove, Is.False);
            Assert.That(result.NextPosition, Is.EqualTo(position));
        }

        [Test]
        public void FindNextStep_ReturnsNoPathWhenEnemyIsEnclosed()
        {
            MineGrid grid = CreateOpenGrid(5, 5);
            grid.TrySetCell(new GridPosition(2, 1), TerrainCell.Wall(2));
            grid.TrySetCell(new GridPosition(2, 3), TerrainCell.Wall(2));
            grid.TrySetCell(new GridPosition(1, 2), TerrainCell.Wall(2));
            grid.TrySetCell(new GridPosition(3, 2), TerrainCell.Wall(2));

            EnemyPathfindingResult result = EnemyPathfindingRules.FindNextStep(
                grid,
                new GridPosition(2, 2),
                new GridPosition(4, 4),
                64);

            Assert.That(result.Found, Is.False);
            Assert.That(result.CanMove, Is.False);
        }

        [Test]
        public void FindNextStep_ReturnsNoPathWhenTargetIsWall()
        {
            MineGrid grid = CreateOpenGrid(5, 5);
            grid.TrySetCell(new GridPosition(4, 4), TerrainCell.Wall(2));

            EnemyPathfindingResult result = EnemyPathfindingRules.FindNextStep(
                grid,
                new GridPosition(1, 1),
                new GridPosition(4, 4),
                64);

            Assert.That(result.Found, Is.False);
        }

        [Test]
        public void FindNextStep_ReturnsNoPathWhenPositionIsOutOfBounds()
        {
            MineGrid grid = CreateOpenGrid(5, 5);

            EnemyPathfindingResult result = EnemyPathfindingRules.FindNextStep(
                grid,
                new GridPosition(-1, 1),
                new GridPosition(4, 4),
                64);

            Assert.That(result.Found, Is.False);
        }

        [Test]
        public void FindNextStep_StopsWhenVisitedLimitIsTooSmall()
        {
            MineGrid grid = CreateOpenGrid(8, 8);

            EnemyPathfindingResult result = EnemyPathfindingRules.FindNextStep(
                grid,
                new GridPosition(1, 1),
                new GridPosition(6, 6),
                2);

            Assert.That(result.Found, Is.False);
            Assert.That(result.VisitedCellCount, Is.EqualTo(2));
        }

        [Test]
        public void FindNextStep_ThrowsForNullGrid()
        {
            Assert.Throws<System.ArgumentNullException>(() =>
            {
                _ = EnemyPathfindingRules.FindNextStep(
                    null,
                    GridPosition.Zero,
                    new GridPosition(1, 1),
                    64);
            });
        }

        [Test]
        public void FindNextStep_ThrowsForInvalidVisitedLimit()
        {
            MineGrid grid = CreateOpenGrid(5, 5);

            Assert.Throws<System.ArgumentOutOfRangeException>(() =>
            {
                _ = EnemyPathfindingRules.FindNextStep(
                    grid,
                    GridPosition.Zero,
                    new GridPosition(1, 1),
                    0);
            });
        }

        private static MineGrid CreateOpenGrid(int width, int height)
        {
            return new MineGrid(width, height, TerrainCell.Floor);
        }
    }
}