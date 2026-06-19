using DeepSeal.Combat;
using DeepSeal.Core;
using DeepSeal.Mining;
using NUnit.Framework;

namespace DeepSeal.Tests.Combat
{
    public sealed class EnemyMovementRulesTests
    {
        [Test]
        public void TryMoveToward_MovesHorizontallyTowardTargetWhenClear()
        {
            MineGrid grid = CreateOpenGrid();
            var enemy = new EnemyState(1, new GridPosition(1, 1));

            EnemyMoveResult result = EnemyMovementRules.TryMoveToward(
                grid,
                enemy,
                new GridPosition(4, 1));

            Assert.That(result.Type, Is.EqualTo(EnemyMoveResultType.Moved));
            Assert.That(result.Direction, Is.EqualTo(GridDirection.Right));
            Assert.That(result.CurrentEnemy.Position, Is.EqualTo(new GridPosition(2, 1)));
        }

        [Test]
        public void TryMoveToward_MovesVerticallyWhenVerticalDistanceIsLarger()
        {
            MineGrid grid = CreateOpenGrid();
            var enemy = new EnemyState(1, new GridPosition(1, 1));

            EnemyMoveResult result = EnemyMovementRules.TryMoveToward(
                grid,
                enemy,
                new GridPosition(2, 4));

            Assert.That(result.Type, Is.EqualTo(EnemyMoveResultType.Moved));
            Assert.That(result.Direction, Is.EqualTo(GridDirection.Up));
            Assert.That(result.CurrentEnemy.Position, Is.EqualTo(new GridPosition(1, 2)));
        }

        [Test]
        public void TryMoveToward_PrefersHorizontalDirectionOnTie()
        {
            MineGrid grid = CreateOpenGrid();
            var enemy = new EnemyState(1, new GridPosition(1, 1));

            EnemyMoveResult result = EnemyMovementRules.TryMoveToward(
                grid,
                enemy,
                new GridPosition(3, 3));

            Assert.That(result.Type, Is.EqualTo(EnemyMoveResultType.Moved));
            Assert.That(result.Direction, Is.EqualTo(GridDirection.Right));
            Assert.That(result.CurrentEnemy.Position, Is.EqualTo(new GridPosition(2, 1)));
        }

        [Test]
        public void TryMoveToward_FallsBackToSecondaryDirectionWhenPrimaryIsBlocked()
        {
            MineGrid grid = CreateOpenGrid();
            grid.TrySetCell(new GridPosition(2, 1), TerrainCell.Wall(2));

            var enemy = new EnemyState(1, new GridPosition(1, 1));

            EnemyMoveResult result = EnemyMovementRules.TryMoveToward(
                grid,
                enemy,
                new GridPosition(3, 2));

            Assert.That(result.Type, Is.EqualTo(EnemyMoveResultType.Moved));
            Assert.That(result.Direction, Is.EqualTo(GridDirection.Up));
            Assert.That(result.CurrentEnemy.Position, Is.EqualTo(new GridPosition(1, 2)));
        }

        [Test]
        public void TryMoveToward_ReturnsBlockedWhenPrimaryAndSecondaryAreBlocked()
        {
            MineGrid grid = CreateOpenGrid();
            grid.TrySetCell(new GridPosition(2, 1), TerrainCell.Wall(2));
            grid.TrySetCell(new GridPosition(1, 2), TerrainCell.Wall(2));

            var enemy = new EnemyState(1, new GridPosition(1, 1));

            EnemyMoveResult result = EnemyMovementRules.TryMoveToward(
                grid,
                enemy,
                new GridPosition(3, 2));

            Assert.That(result.Type, Is.EqualTo(EnemyMoveResultType.Blocked));
            Assert.That(result.Direction, Is.EqualTo(GridDirection.Right));
            Assert.That(result.CurrentEnemy, Is.EqualTo(enemy));
        }

        [Test]
        public void TryMoveToward_ReturnsAlreadyAtTargetWithoutMoving()
        {
            MineGrid grid = CreateOpenGrid();
            var enemy = new EnemyState(1, new GridPosition(2, 2));

            EnemyMoveResult result = EnemyMovementRules.TryMoveToward(
                grid,
                enemy,
                new GridPosition(2, 2));

            Assert.That(result.Type, Is.EqualTo(EnemyMoveResultType.AlreadyAtTarget));
            Assert.That(result.Moved, Is.False);
            Assert.That(result.CurrentEnemy, Is.EqualTo(enemy));
        }

        [Test]
        public void TryMoveToward_ReturnsEnemyOutOfBoundsWhenEnemyStartsOutsideGrid()
        {
            MineGrid grid = CreateOpenGrid();
            var enemy = new EnemyState(1, new GridPosition(-1, 1));

            EnemyMoveResult result = EnemyMovementRules.TryMoveToward(
                grid,
                enemy,
                new GridPosition(1, 1));

            Assert.That(result.Type, Is.EqualTo(EnemyMoveResultType.EnemyOutOfBounds));
            Assert.That(result.CurrentEnemy, Is.EqualTo(enemy));
        }

        [Test]
        public void TryMoveToward_ReturnsDestinationOutOfBoundsWhenTargetIsOutsideGrid()
        {
            MineGrid grid = CreateOpenGrid();
            var enemy = new EnemyState(1, new GridPosition(1, 1));

            EnemyMoveResult result = EnemyMovementRules.TryMoveToward(
                grid,
                enemy,
                new GridPosition(5, 1));

            Assert.That(result.Type, Is.EqualTo(EnemyMoveResultType.DestinationOutOfBounds));
            Assert.That(result.AttemptedPosition, Is.EqualTo(new GridPosition(5, 1)));
            Assert.That(result.CurrentEnemy, Is.EqualTo(enemy));
        }

        [Test]
        public void TryMoveOneCell_ReturnsDestinationOutOfBoundsWhenStepLeavesGrid()
        {
            MineGrid grid = CreateOpenGrid();
            var enemy = new EnemyState(1, GridPosition.Zero);

            EnemyMoveResult result = EnemyMovementRules.TryMoveOneCell(
                grid,
                enemy,
                GridDirection.Left);

            Assert.That(result.Type, Is.EqualTo(EnemyMoveResultType.DestinationOutOfBounds));
            Assert.That(result.AttemptedPosition, Is.EqualTo(new GridPosition(-1, 0)));
            Assert.That(result.CurrentEnemy, Is.EqualTo(enemy));
        }

        [Test]
        public void TryMoveOneCell_ReturnsBlockedWhenDestinationIsWall()
        {
            MineGrid grid = CreateOpenGrid();
            grid.TrySetCell(new GridPosition(2, 1), TerrainCell.Wall(2));

            var enemy = new EnemyState(1, new GridPosition(1, 1));

            EnemyMoveResult result = EnemyMovementRules.TryMoveOneCell(
                grid,
                enemy,
                GridDirection.Right);

            Assert.That(result.Type, Is.EqualTo(EnemyMoveResultType.Blocked));
            Assert.That(result.CurrentEnemy, Is.EqualTo(enemy));
        }

        [Test]
        public void TryMoveOneCell_ReturnsInvalidDirectionForNone()
        {
            MineGrid grid = CreateOpenGrid();
            var enemy = new EnemyState(1, new GridPosition(1, 1));

            EnemyMoveResult result = EnemyMovementRules.TryMoveOneCell(
                grid,
                enemy,
                GridDirection.None);

            Assert.That(result.Type, Is.EqualTo(EnemyMoveResultType.InvalidDirection));
            Assert.That(result.CurrentEnemy, Is.EqualTo(enemy));
        }

        [Test]
        public void TryMoveToward_ThrowsForNullGrid()
        {
            var enemy = new EnemyState(1, GridPosition.Zero);

            Assert.Throws<System.ArgumentNullException>(() =>
            {
                _ = EnemyMovementRules.TryMoveToward(null, enemy, new GridPosition(1, 0));
            });
        }

        [Test]
        public void TryMoveOneCell_ThrowsForNullGrid()
        {
            var enemy = new EnemyState(1, GridPosition.Zero);

            Assert.Throws<System.ArgumentNullException>(() =>
            {
                _ = EnemyMovementRules.TryMoveOneCell(null, enemy, GridDirection.Right);
            });
        }

        private static MineGrid CreateOpenGrid()
        {
            return new MineGrid(5, 5, TerrainCell.Floor);
        }
    }
}