using DeepSeal.Combat;
using DeepSeal.Core;
using NUnit.Framework;

namespace DeepSeal.Tests.Combat
{
    public sealed class EnemyStateTests
    {
        [Test]
        public void Constructor_StoresIdAndPosition()
        {
            var position = new GridPosition(2, 3);

            var enemy = new EnemyState(7, position);

            Assert.That(enemy.Id, Is.EqualTo(7));
            Assert.That(enemy.Position, Is.EqualTo(position));
        }

        [Test]
        public void Constructor_ThrowsForNegativeId()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() =>
            {
                _ = new EnemyState(-1, GridPosition.Zero);
            });
        }

        [Test]
        public void WithPosition_ReturnsEnemyWithSameIdAndNewPosition()
        {
            var enemy = new EnemyState(3, new GridPosition(1, 1));

            EnemyState moved = enemy.WithPosition(new GridPosition(2, 1));

            Assert.That(moved.Id, Is.EqualTo(enemy.Id));
            Assert.That(moved.Position, Is.EqualTo(new GridPosition(2, 1)));
            Assert.That(enemy.Position, Is.EqualTo(new GridPosition(1, 1)));
        }

        [Test]
        public void Equality_UsesIdAndPosition()
        {
            var left = new EnemyState(1, new GridPosition(2, 2));
            var same = new EnemyState(1, new GridPosition(2, 2));
            var differentId = new EnemyState(2, new GridPosition(2, 2));
            var differentPosition = new EnemyState(1, new GridPosition(3, 2));

            Assert.That(left, Is.EqualTo(same));
            Assert.That(left == same, Is.True);
            Assert.That(left != differentId, Is.True);
            Assert.That(left != differentPosition, Is.True);
        }

        [Test]
        public void ToString_ReturnsReadableState()
        {
            var enemy = new EnemyState(5, new GridPosition(4, 6));

            Assert.That(enemy.ToString(), Is.EqualTo("Enemy(5) at (4, 6)"));
        }
    }
}