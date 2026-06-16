using DeepSeal.Core;
using DeepSeal.Mining;
using NUnit.Framework;

namespace DeepSeal.Tests.Mining
{
    /// <summary>
    /// 채굴 피해 적용 규칙을 검증한다.
    /// </summary>
    public sealed class MiningRulesTests
    {
        [Test]
        public void ApplyMiningDamage_ReducesWallDurability()
        {
            var grid = new MineGrid(2, 2, TerrainCell.Floor);
            var position = new GridPosition(1, 1);
            grid.TrySetCell(position, TerrainCell.Wall(5));

            MiningResult result = MiningRules.ApplyMiningDamage(grid, position, 2);
            grid.TryGetCell(position, out TerrainCell currentCell);

            Assert.That(result.Type, Is.EqualTo(MiningResultType.Damaged));
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.ChangedCell, Is.True);
            Assert.That(result.WasDestroyed, Is.False);
            Assert.That(result.HasCell, Is.True);
            Assert.That(result.PreviousCell, Is.EqualTo(TerrainCell.Wall(5)));
            Assert.That(result.CurrentCell, Is.EqualTo(TerrainCell.Wall(3)));
            Assert.That(currentCell, Is.EqualTo(TerrainCell.Wall(3)));
        }

        [Test]
        public void ApplyMiningDamage_DestroyWallWhenDurabilityReachesZero()
        {
            var grid = new MineGrid(2, 2, TerrainCell.Floor);
            var position = new GridPosition(0, 1);
            grid.TrySetCell(position, TerrainCell.Wall(3));

            MiningResult result = MiningRules.ApplyMiningDamage(grid, position, 3);
            grid.TryGetCell(position, out TerrainCell currentCell);

            Assert.That(result.Type, Is.EqualTo(MiningResultType.Destroyed));
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.ChangedCell, Is.True);
            Assert.That(result.WasDestroyed, Is.True);
            Assert.That(result.PreviousCell, Is.EqualTo(TerrainCell.Wall(3)));
            Assert.That(result.CurrentCell, Is.EqualTo(TerrainCell.Floor));
            Assert.That(currentCell, Is.EqualTo(TerrainCell.Floor));
            Assert.That(currentCell.IsPassable, Is.True);
        }

        [Test]
        public void ApplyMiningDamage_DestroyWallWhenDamageExceedsDurability()
        {
            var grid = new MineGrid(2, 2, TerrainCell.Floor);
            var position = new GridPosition(0, 0);
            grid.TrySetCell(position, TerrainCell.Wall(2));

            MiningResult result = MiningRules.ApplyMiningDamage(grid, position, 5);
            grid.TryGetCell(position, out TerrainCell currentCell);

            Assert.That(result.Type, Is.EqualTo(MiningResultType.Destroyed));
            Assert.That(result.WasDestroyed, Is.True);
            Assert.That(currentCell, Is.EqualTo(TerrainCell.Floor));
        }

        [Test]
        public void ApplyMiningDamage_OnFloorReturnsNotMineableAndKeepsCell()
        {
            var grid = new MineGrid(2, 2, TerrainCell.Floor);
            var position = new GridPosition(1, 0);

            MiningResult result = MiningRules.ApplyMiningDamage(grid, position, 2);
            grid.TryGetCell(position, out TerrainCell currentCell);

            Assert.That(result.Type, Is.EqualTo(MiningResultType.NotMineable));
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.ChangedCell, Is.False);
            Assert.That(result.WasDestroyed, Is.False);
            Assert.That(result.HasCell, Is.True);
            Assert.That(result.PreviousCell, Is.EqualTo(TerrainCell.Floor));
            Assert.That(result.CurrentCell, Is.EqualTo(TerrainCell.Floor));
            Assert.That(currentCell, Is.EqualTo(TerrainCell.Floor));
        }

        [Test]
        public void ApplyMiningDamage_OutOfBoundsReturnsOutOfBounds()
        {
            var grid = new MineGrid(2, 2, TerrainCell.Wall(3));
            var position = new GridPosition(5, 5);

            MiningResult result = MiningRules.ApplyMiningDamage(grid, position, 1);

            Assert.That(result.Type, Is.EqualTo(MiningResultType.OutOfBounds));
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.ChangedCell, Is.False);
            Assert.That(result.WasDestroyed, Is.False);
            Assert.That(result.HasCell, Is.False);
            Assert.That(result.Position, Is.EqualTo(position));
            Assert.That(result.Damage, Is.EqualTo(1));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void ApplyMiningDamage_ThrowsForInvalidDamage(int damage)
        {
            var grid = new MineGrid(2, 2, TerrainCell.Wall(3));

            Assert.Throws<System.ArgumentOutOfRangeException>(() =>
            {
                _ = MiningRules.ApplyMiningDamage(grid, GridPosition.Zero, damage);
            });
        }

        [Test]
        public void ApplyMiningDamage_ThrowsForNullGrid()
        {
            Assert.Throws<System.ArgumentNullException>(() =>
            {
                _ = MiningRules.ApplyMiningDamage(null, GridPosition.Zero, 1);
            });
        }
    }
}