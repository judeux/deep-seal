using System;
using System.Collections.Generic;
using DeepSeal.Combat;
using DeepSeal.Core;
using DeepSeal.Mining;
using NUnit.Framework;

namespace DeepSeal.Tests.Combat
{
    public sealed class ProjectileAttackRulesTests
    {
        [Test]
        public void ResolveFireDirection_PrefersLargerDeltaAxis()
        {
            Assert.That(ProjectileAttackRules.ResolveFireDirection(new GridPosition(0, 0), new GridPosition(3, 1)), Is.EqualTo(GridDirection.Right));
            Assert.That(ProjectileAttackRules.ResolveFireDirection(new GridPosition(0, 0), new GridPosition(1, 3)), Is.EqualTo(GridDirection.Up));
            Assert.That(ProjectileAttackRules.ResolveFireDirection(new GridPosition(2, 2), new GridPosition(0, 1)), Is.EqualTo(GridDirection.Left));
            Assert.That(ProjectileAttackRules.ResolveFireDirection(new GridPosition(0, 0), new GridPosition(0, 0)), Is.EqualTo(GridDirection.None));
        }

        [Test]
        public void Trace_HitsFirstEnemyOnFlightPath()
        {
            MineGrid grid = new MineGrid(5, 5, TerrainCell.Floor);
            var enemies = new List<EnemyState>
            {
                new EnemyState(3, new GridPosition(4, 0)),
                new EnemyState(1, new GridPosition(2, 0))
            };

            ProjectileTraceResult result = ProjectileAttackRules.Trace(
                grid, new GridPosition(0, 0), new GridPosition(4, 0), enemies, 5);

            Assert.That(result.HasImpact, Is.True);
            Assert.That(result.HitEnemyId, Is.EqualTo(1));
            Assert.That(result.ImpactPosition, Is.EqualTo(new GridPosition(2, 0)));
            Assert.That(result.TraveledCells, Is.EqualTo(2));
            Assert.That(result.BlockedByWall, Is.False);
        }

        [Test]
        public void Trace_StopsAtFirstWall()
        {
            MineGrid grid = new MineGrid(5, 5, TerrainCell.Floor);
            grid.TrySetCell(new GridPosition(2, 0), TerrainCell.Wall(3));
            var enemies = new List<EnemyState> { new EnemyState(1, new GridPosition(4, 0)) };

            ProjectileTraceResult result = ProjectileAttackRules.Trace(
                grid, new GridPosition(0, 0), new GridPosition(4, 0), enemies, 5);

            Assert.That(result.HasImpact, Is.True);
            Assert.That(result.BlockedByWall, Is.True);
            Assert.That(result.HitEnemyId, Is.EqualTo(-1));
            Assert.That(result.ImpactPosition, Is.EqualTo(new GridPosition(2, 0)));
        }

        [Test]
        public void Trace_StopsAtRangeLimitWithoutEnemyHit()
        {
            MineGrid grid = new MineGrid(9, 3, TerrainCell.Floor);
            var enemies = new List<EnemyState> { new EnemyState(1, new GridPosition(8, 0)) };

            ProjectileTraceResult result = ProjectileAttackRules.Trace(
                grid, new GridPosition(0, 0), new GridPosition(8, 0), enemies, 4);

            Assert.That(result.HasImpact, Is.True);
            Assert.That(result.HitEnemyId, Is.EqualTo(-1));
            Assert.That(result.BlockedByWall, Is.False);
            Assert.That(result.ImpactPosition, Is.EqualTo(new GridPosition(4, 0)));
            Assert.That(result.TraveledCells, Is.EqualTo(4));
        }

        [Test]
        public void Trace_OutOfBoundsCellStopsFlightLikeAWall()
        {
            MineGrid grid = new MineGrid(3, 3, TerrainCell.Floor);

            ProjectileTraceResult result = ProjectileAttackRules.Trace(
                grid, new GridPosition(2, 1), new GridPosition(8, 1), new List<EnemyState>(), 5);

            Assert.That(result.HasImpact, Is.True);
            Assert.That(result.BlockedByWall, Is.True);
            Assert.That(result.ImpactPosition, Is.EqualTo(new GridPosition(3, 1)));
        }

        [Test]
        public void Trace_SameOriginAndTargetDoesNotShoot()
        {
            MineGrid grid = new MineGrid(3, 3, TerrainCell.Floor);

            ProjectileTraceResult result = ProjectileAttackRules.Trace(
                grid, new GridPosition(1, 1), new GridPosition(1, 1), new List<EnemyState>(), 4);

            Assert.That(result.HasImpact, Is.False);
        }

        [Test]
        public void Trace_ThrowsForInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ProjectileAttackRules.Trace(null, GridPosition.Zero, GridPosition.Zero, new List<EnemyState>(), 1));
            Assert.Throws<ArgumentNullException>(() =>
                ProjectileAttackRules.Trace(new MineGrid(1, 1, TerrainCell.Floor), GridPosition.Zero, GridPosition.Zero, null, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                ProjectileAttackRules.Trace(new MineGrid(1, 1, TerrainCell.Floor), GridPosition.Zero, GridPosition.Zero, new List<EnemyState>(), -1));
        }
    }
}