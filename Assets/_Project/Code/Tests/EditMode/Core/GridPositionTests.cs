using DeepSeal.Core;
using NUnit.Framework;

namespace DeepSeal.Tests.Core
{
    /// <summary>
    /// GridPosition의 좌표 저장, 오프셋, 등가성, 연산자 동작을 검증한다.
    /// </summary>
    public sealed class GridPositionTests
    {
        [Test]
        public void Constructor_StoresCoordinates()
        {
            var position = new GridPosition(3, -2);
            
            Assert.That(position.X, Is.EqualTo(3));
            Assert.That(position.Y, Is.EqualTo(-2));
        }

        [Test]
        public void Zero_ReturnsOrigin()
        {
            Assert.That(GridPosition.Zero, Is.EqualTo(new GridPosition(0, 0)));
        }

        [Test]
        public void Offset_ByDelta_ReturnsTranslatedPosition()
        {
            var position = new GridPosition(2, 5);
            GridPosition result = position.Offset(-3, 4);
            Assert.That(result, Is.EqualTo(new GridPosition(-1, 9)));
        }

        [Test]
        public void Equality_UsesCoordinates()
        {
            var left = new GridPosition(4, 7);
            var same = new GridPosition(4, 7);
            var different = new GridPosition(7, 4);

            Assert.That(left, Is.EqualTo(same));
            Assert.That(left == same, Is.True);
            Assert.That(left != same, Is.False);
            Assert.That(left, Is.Not.EqualTo(different));
            Assert.That(left == different, Is.False);
            Assert.That(left != different, Is.True);
        }

        [Test]
        public void Addition_AddsCoordinates()
        {
            var left = new GridPosition(4, 7);
            var right = new GridPosition(-2, 3);

            GridPosition result = left + right;

            Assert.That(result, Is.EqualTo(new GridPosition(2, 10)));
        }

        [Test]
        public void Subtraction_SubtractsCoordinates()
        {
            var left = new GridPosition(4, 7);
            var right = new GridPosition(-2, 3);

            GridPosition result = left - right;

            Assert.That(result, Is.EqualTo(new GridPosition(6, 4)));
        }

        [Test]
        public void ToString_ReturnsReadableCoordinates()
        {
            var position = new GridPosition(-3, 8);

            Assert.That(position.ToString(), Is.EqualTo("(-3, 8)"));
        }

        [Test]
        public void Offset_ByDirection_ReturnsTranslatedPosition()
        {
            var position = new GridPosition(2, 5);

            Assert.That(position.Offset(GridDirection.Up), Is.EqualTo(new GridPosition(2, 6)));
            Assert.That(position.Offset(GridDirection.Right), Is.EqualTo(new GridPosition(3, 5)));
            Assert.That(position.Offset(GridDirection.Down), Is.EqualTo(new GridPosition(2, 4)));
            Assert.That(position.Offset(GridDirection.Left), Is.EqualTo(new GridPosition(1, 5)));
            Assert.That(position.Offset(GridDirection.None), Is.EqualTo(position));
        }

    }
}
