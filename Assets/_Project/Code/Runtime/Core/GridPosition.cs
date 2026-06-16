using System;

namespace DeepSeal.Core
{
    /// <summary>
    /// Represents an integer cell coordinate in a grid-based expedition map.
    /// 절차 생성, 채굴, 이동, 전투 공간 계산에서 공통으로 쓸 정수 셀 좌표.
    /// </summary>
    public readonly struct GridPosition : IEquatable<GridPosition>
    {
        public GridPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public static GridPosition Zero => new GridPosition(0, 0);

        public GridPosition Offset(int deltaX, int deltaY) 
        {
            return new GridPosition(X + deltaX, Y + deltaY);
        }

        public GridPosition Offset(GridDirection direction)
        {
            return this + direction.ToOffset();
        }

        public bool Equals(GridPosition other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is GridPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked 
            {
                return (X * 397) ^ Y;
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public static bool operator ==(GridPosition left, GridPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GridPosition left, GridPosition right)
        {
            return !left.Equals(right);
        }

        public static GridPosition operator +(GridPosition left, GridPosition right)
        {
            return new GridPosition(left.X + right.X, left.Y + right.Y);
        }

        public static GridPosition operator -(GridPosition left, GridPosition right)
        {
            return new GridPosition(left.X - right.X, left.Y - right.Y);
        }
    }
}
