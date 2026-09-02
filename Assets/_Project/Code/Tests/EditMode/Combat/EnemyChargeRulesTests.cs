using System;
using DeepSeal.Combat;
using DeepSeal.Core;
using DeepSeal.Mining;
using NUnit.Framework;

namespace DeepSeal.Tests.Combat
{
    public sealed class EnemyChargeRulesTests
    {
        [Test]
        public void TraceCharge_ReachesTargetThroughOpenPath()
        {
            MineGrid grid = new MineGrid(9, 3, TerrainCell.Floor);

            EnemyChargeResult result = EnemyChargeRules.TraceCharge(
                grid, new GridPosition(0, 1), new GridPosition(4, 1), 6);

            Assert.That(result.StopReason, Is.EqualTo(EnemyChargeStopReason.TargetReached));
            Assert.That(result.FinalPosition, Is.EqualTo(new GridPosition(4, 1)));
            Assert.That(result.PathCellCount, Is.EqualTo(4));
            Assert.That(result.Stunned, Is.False);
        }

        [Test]
        public void TraceCharge_StopsBeforeWallAndStuns()
        {
            MineGrid grid = new MineGrid(9, 3, TerrainCell.Floor);
            grid.TrySetCell(new GridPosition(3, 1), TerrainCell.Wall(3));

            EnemyChargeResult result = EnemyChargeRules.TraceCharge(
                grid, new GridPosition(0, 1), new GridPosition(6, 1), 6);

            Assert.That(result.StopReason, Is.EqualTo(EnemyChargeStopReason.WallHit));
            Assert.That(result.FinalPosition, Is.EqualTo(new GridPosition(2, 1)));
            Assert.That(result.PathCellCount, Is.EqualTo(2));
            Assert.That(result.Stunned, Is.True);
        }

        [Test]
        public void TraceCharge_StopsAtRangeLimit()
        {
            MineGrid grid = new MineGrid(11, 3, TerrainCell.Floor);

            EnemyChargeResult result = EnemyChargeRules.TraceCharge(
                grid, new GridPosition(0, 1), new GridPosition(9, 1), 4);

            Assert.That(result.StopReason, Is.EqualTo(EnemyChargeStopReason.RangeEnd));
            Assert.That(result.FinalPosition, Is.EqualTo(new GridPosition(4, 1)));
            Assert.That(result.PathCellCount, Is.EqualTo(4));
            Assert.That(result.Stunned, Is.False);
        }

        [Test]
        public void TraceCharge_ImmediateWallGivesEmptyStunnedPath()
        {
            MineGrid grid = new MineGrid(9, 3, TerrainCell.Floor);
            grid.TrySetCell(new GridPosition(1, 1), TerrainCell.Wall(3));

            EnemyChargeResult result = EnemyChargeRules.TraceCharge(
                grid, new GridPosition(0, 1), new GridPosition(5, 1), 6);

            Assert.That(result.StopReason, Is.EqualTo(EnemyChargeStopReason.WallHit));
            Assert.That(result.FinalPosition, Is.EqualTo(new GridPosition(0, 1)));
            Assert.That(result.PathCellCount, Is.EqualTo(0));
            Assert.That(result.Stunned, Is.True);
        }

        [Test]
        public void TraceCharge_SamePositionDoesNotCharge()
        {
            MineGrid grid = new MineGrid(5, 5, TerrainCell.Floor);

            EnemyChargeResult result = EnemyChargeRules.TraceCharge(
                grid, new GridPosition(2, 2), new GridPosition(2, 2), 4);

            Assert.That(result.StopReason, Is.EqualTo(EnemyChargeStopReason.RangeEnd));
            Assert.That(result.PathCellCount, Is.EqualTo(0));
            Assert.That(result.Stunned, Is.False);
        }

        [Test]
        public void TraceCharge_ThrowsForInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() =>
                EnemyChargeRules.TraceCharge(null, GridPosition.Zero, new GridPosition(1, 0), 1));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                EnemyChargeRules.TraceCharge(
                    new MineGrid(3, 3, TerrainCell.Floor),
                    GridPosition.Zero,
                    new GridPosition(1, 0),
                    -1));
        }
    }
}
