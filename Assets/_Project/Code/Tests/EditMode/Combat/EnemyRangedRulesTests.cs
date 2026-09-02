using System;
using DeepSeal.Combat;
using DeepSeal.Core;
using DeepSeal.Mining;
using NUnit.Framework;

namespace DeepSeal.Tests.Combat
{
    public sealed class EnemyRangedRulesTests
    {
        [Test]
        public void HasClearCardinalLine_ReturnsTrueForAlignedOpenLine()
        {
            MineGrid grid = new MineGrid(9, 3, TerrainCell.Floor);

            bool clear = EnemyRangedRules.HasClearCardinalLine(
                grid, new GridPosition(0, 1), new GridPosition(5, 1));

            Assert.That(clear, Is.True);
        }

        [Test]
        public void HasClearCardinalLine_ReturnsFalseWhenWallBlocks()
        {
            MineGrid grid = new MineGrid(9, 3, TerrainCell.Floor);
            grid.TrySetCell(new GridPosition(2, 1), TerrainCell.Wall(3));

            bool clear = EnemyRangedRules.HasClearCardinalLine(
                grid, new GridPosition(0, 1), new GridPosition(5, 1));

            Assert.That(clear, Is.False);
        }

        [Test]
        public void HasClearCardinalLine_ReturnsFalseForMisalignedTarget()
        {
            MineGrid grid = new MineGrid(9, 9, TerrainCell.Floor);

            bool clear = EnemyRangedRules.HasClearCardinalLine(
                grid, new GridPosition(0, 0), new GridPosition(4, 3));

            Assert.That(clear, Is.False);
        }

        [Test]
        public void HasClearCardinalLine_ReturnsFalseForSameCell()
        {
            MineGrid grid = new MineGrid(5, 5, TerrainCell.Floor);

            bool clear = EnemyRangedRules.HasClearCardinalLine(
                grid, new GridPosition(2, 2), new GridPosition(2, 2));

            Assert.That(clear, Is.False);
        }

        [Test]
        public void ResolveBandStepDirection_ApproachesWhenTooFar()
        {
            GridDirection direction = EnemyRangedRules.ResolveBandStepDirection(
                new GridPosition(0, 0), new GridPosition(6, 0), 2, 4);

            Assert.That(direction, Is.EqualTo(GridDirection.Right));
        }

        [Test]
        public void ResolveBandStepDirection_RetreatsWhenTooClose()
        {
            GridDirection direction = EnemyRangedRules.ResolveBandStepDirection(
                new GridPosition(0, 0), new GridPosition(1, 0), 2, 4);

            Assert.That(direction, Is.EqualTo(GridDirection.Left));
        }

        [Test]
        public void ResolveBandStepDirection_StaysInsideBand()
        {
            GridDirection direction = EnemyRangedRules.ResolveBandStepDirection(
                new GridPosition(0, 0), new GridPosition(3, 0), 2, 4);

            Assert.That(direction, Is.EqualTo(GridDirection.None));
        }

        [Test]
        public void ResolveBandStepDirection_ThrowsForInvalidRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                EnemyRangedRules.ResolveBandStepDirection(
                    new GridPosition(0, 0), new GridPosition(1, 0), -1, 4));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                EnemyRangedRules.ResolveBandStepDirection(
                    new GridPosition(0, 0), new GridPosition(1, 0), 5, 4));
        }
    }
}
