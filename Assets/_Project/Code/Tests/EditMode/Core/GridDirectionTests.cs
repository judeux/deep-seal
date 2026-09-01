using System;
using DeepSeal.Core;
using NUnit.Framework;

namespace DeepSeal.Tests.Core
{
    /// <summary>
    /// 방향 enum의 오프셋 변환, 반대 방향, cardinal 판정을 검증한다.
    /// </summary>
    public sealed class GridDirectionTests
    {
        [TestCase(GridDirection.None, 0, 0)]
        [TestCase(GridDirection.Up, 0, 1)]
        [TestCase(GridDirection.Right, 1, 0)]
        [TestCase(GridDirection.Down, 0, -1)]
        [TestCase(GridDirection.Left, -1, 0)]
        public void ToOffset_ReturnsExpectedGridOffset(GridDirection direction, int expectedX, int expectedY)
        {
            GridPosition offset = direction.ToOffset();

            Assert.That(offset, Is.EqualTo(new GridPosition(expectedX, expectedY)));
        }

        [TestCase(GridDirection.None, GridDirection.None)]
        [TestCase(GridDirection.Up, GridDirection.Down)]
        [TestCase(GridDirection.Right, GridDirection.Left)]
        [TestCase(GridDirection.Down, GridDirection.Up)]
        [TestCase(GridDirection.Left, GridDirection.Right)]
        public void Opposite_ReturnsExpectedDirection(GridDirection direction, GridDirection expected)
        {
            Assert.That(direction.Opposite(), Is.EqualTo(expected));
        }

        [TestCase(GridDirection.None, false)]
        [TestCase(GridDirection.Up, true)]
        [TestCase(GridDirection.Right, true)]
        [TestCase(GridDirection.Down, true)]
        [TestCase(GridDirection.Left, true)]
        public void IsCardinal_ReturnsWhetherDirectionCanMoveToNeighborCell(
            GridDirection direction,
            bool expected)
        {
            Assert.That(direction.IsCardinal(), Is.EqualTo(expected));
        }

        [Test]
        public void ToOffset_ThrowsForUnknownDirection()
        {
            var unknown = (GridDirection)999;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = unknown.ToOffset();
            });
        }

        [Test]
        public void Opposite_ThrowsForUnknownDirection()
        {
            var unknown = (GridDirection)999;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = unknown.Opposite();
            });
        }
    }
}
