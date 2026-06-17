using DeepSeal.Core;
using DeepSeal.Mining;
using NUnit.Framework;

namespace DeepSeal.Tests.Mining
{
    /// <summary>
    /// MineGrid와 기본 셀 상태가 안전하게 동작하는지 검증한다.
    /// </summary>
    public sealed class MineGridTests
    {
        [Test]
        public void Constructor_FillsAllCellsWithDefaultCell()
        {
            var grid = new MineGrid(2, 3, TerrainCell.Wall(4));

            Assert.That(grid.Width, Is.EqualTo(2));
            Assert.That(grid.Height, Is.EqualTo(3));

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    bool found = grid.TryGetCell(new GridPosition(x, y), out TerrainCell cell);

                    Assert.That(found, Is.True);
                    Assert.That(cell, Is.EqualTo(TerrainCell.Wall(4)));
                }
            }
        }

        [TestCase(0, 1)]
        [TestCase(1, 0)]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        public void Constructor_ThrowsForInvalidSize(int width, int height)
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() =>
            {
                _ = new MineGrid(width, height, TerrainCell.Floor);
            });
        }

        [TestCase(0, 0, true)]
        [TestCase(2, 1, true)]
        [TestCase(-1, 0, false)]
        [TestCase(0, -1, false)]
        [TestCase(3, 0, false)]
        [TestCase(0, 2, false)]
        public void Contains_ReturnsWhetherPositionIsInsideBounds(int x, int y, bool expected)
        {
            var grid = new MineGrid(3, 2, TerrainCell.Floor);

            bool result = grid.Contains(new GridPosition(x, y));

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TryGetCell_ReturnsCellForInsidePosition()
        {
            var grid = new MineGrid(2, 2, TerrainCell.Wall(3));

            bool found = grid.TryGetCell(new GridPosition(1, 1), out TerrainCell cell);

            Assert.That(found, Is.True);
            Assert.That(cell, Is.EqualTo(TerrainCell.Wall(3)));
        }

        [Test]
        public void TryGetCell_ReturnsFalseForOutOfBoundsPosition()
        {
            var grid = new MineGrid(2, 2, TerrainCell.Floor);

            bool found = grid.TryGetCell(new GridPosition(5, 5), out _);

            Assert.That(found, Is.False);
        }

        [Test]
        public void TrySetCell_UpdatesInsidePosition()
        {
            var grid = new MineGrid(2, 2, TerrainCell.Floor);
            var position = new GridPosition(1, 1);

            bool changed = grid.TrySetCell(position, TerrainCell.Wall(2));
            bool found = grid.TryGetCell(position, out TerrainCell cell);

            Assert.That(changed, Is.True);
            Assert.That(found, Is.True);
            Assert.That(cell, Is.EqualTo(TerrainCell.Wall(2)));
        }

        [Test]
        public void TrySetCell_ReturnsFalseForOutOfBoundsPosition()
        {
            var grid = new MineGrid(2, 2, TerrainCell.Floor);

            bool changed = grid.TrySetCell(new GridPosition(2, 0), TerrainCell.Wall(2));

            Assert.That(changed, Is.False);
        }

        [Test]
        public void FloorCell_IsPassableAndNotMineable()
        {
            TerrainCell cell = TerrainCell.Floor;

            Assert.That(cell.Type, Is.EqualTo(TerrainCellType.Floor));
            Assert.That(cell.Durability, Is.EqualTo(0));
            Assert.That(cell.IsPassable, Is.True);
            Assert.That(cell.IsMineable, Is.False);
        }

        [Test]
        public void WallCell_IsMineableAndNotPassable()
        {
            TerrainCell cell = TerrainCell.Wall(5);

            Assert.That(cell.Type, Is.EqualTo(TerrainCellType.Wall));
            Assert.That(cell.Durability, Is.EqualTo(5));
            Assert.That(cell.IsPassable, Is.False);
            Assert.That(cell.IsMineable, Is.True);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Wall_ThrowsForInvalidDurability(int durability)
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() =>
            {
                _ = TerrainCell.Wall(durability);
            });
        }

        [Test]
        public void TryGetCell_OutOfBoundsSetsCellToDefault()
        {
            var grid = new MineGrid(2, 2, TerrainCell.Wall(3));
            TerrainCell cell = TerrainCell.Wall(9);

            bool found = grid.TryGetCell(new GridPosition(-1, 0), out cell);

            Assert.That(found, Is.False);
            Assert.That(cell, Is.EqualTo(default(TerrainCell)));
            Assert.That(cell, Is.EqualTo(TerrainCell.Floor));
        }

        [Test]
        public void TrySetCell_OutOfBoundsDoesNotChangeExistingCells()
        {
            var grid = new MineGrid(2, 2, TerrainCell.Floor);
            var existingPosition = new GridPosition(1, 1);
            grid.TrySetCell(existingPosition, TerrainCell.Wall(4));

            bool changed = grid.TrySetCell(new GridPosition(2, 0), TerrainCell.Wall(9));
            grid.TryGetCell(existingPosition, out TerrainCell existingCell);
            grid.TryGetCell(GridPosition.Zero, out TerrainCell untouchedCell);

            Assert.That(changed, Is.False);
            Assert.That(existingCell, Is.EqualTo(TerrainCell.Wall(4)));
            Assert.That(untouchedCell, Is.EqualTo(TerrainCell.Floor));
        }

        [Test]
        public void TerrainCell_DefaultValueBehavesLikeFloor()
        {
            TerrainCell cell = default;

            Assert.That(cell, Is.EqualTo(TerrainCell.Floor));
            Assert.That(cell.Type, Is.EqualTo(TerrainCellType.Floor));
            Assert.That(cell.Durability, Is.EqualTo(0));
            Assert.That(cell.IsPassable, Is.True);
            Assert.That(cell.IsMineable, Is.False);
        }
               
    }
}
