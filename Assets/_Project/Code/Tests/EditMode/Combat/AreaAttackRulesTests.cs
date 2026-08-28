using System;
using System.Collections.Generic;
using DeepSeal.Combat;
using DeepSeal.Core;
using NUnit.Framework;

namespace DeepSeal.Tests.Combat
{
    public sealed class AreaAttackRulesTests
    {
        [Test]
        public void CollectAffectedEnemies_ReturnsAllEnemiesWithinRange()
        {
            var enemies = new List<EnemyState>
            {
                new EnemyState(0, new GridPosition(1, 0)),
                new EnemyState(1, new GridPosition(0, 2)),
                new EnemyState(2, new GridPosition(5, 5))
            };
            var affected = new List<EnemyState>();

            int count = AreaAttackRules.CollectAffectedEnemies(new GridPosition(0, 0), enemies, 2, affected);

            Assert.That(count, Is.EqualTo(2));
            Assert.That(affected, Has.Count.EqualTo(2));
            Assert.That(affected[0].Id, Is.EqualTo(0));
            Assert.That(affected[1].Id, Is.EqualTo(1));
        }

        [Test]
        public void CollectAffectedEnemies_ClearsPreviousResults()
        {
            var enemies = new List<EnemyState> { new EnemyState(0, new GridPosition(1, 1)) };
            var affected = new List<EnemyState> { new EnemyState(9, new GridPosition(1, 1)) };

            int count = AreaAttackRules.CollectAffectedEnemies(new GridPosition(0, 0), enemies, 3, affected);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(affected, Has.Count.EqualTo(1));
            Assert.That(affected[0].Id, Is.EqualTo(0));
        }

        [Test]
        public void CollectAffectedEnemies_ReturnsZeroForEmptyEnemyList()
        {
            var affected = new List<EnemyState>();

            int count = AreaAttackRules.CollectAffectedEnemies(new GridPosition(0, 0), new List<EnemyState>(), 3, affected);

            Assert.That(count, Is.EqualTo(0));
            Assert.That(affected, Is.Empty);
        }

        [Test]
        public void CollectAffectedEnemies_ThrowsForInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() =>
                AreaAttackRules.CollectAffectedEnemies(GridPosition.Zero, null, 1, new List<EnemyState>()));
            Assert.Throws<ArgumentNullException>(() =>
                AreaAttackRules.CollectAffectedEnemies(GridPosition.Zero, new List<EnemyState>(), 1, null));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                AreaAttackRules.CollectAffectedEnemies(GridPosition.Zero, new List<EnemyState>(), -1, new List<EnemyState>()));
        }
    }
}