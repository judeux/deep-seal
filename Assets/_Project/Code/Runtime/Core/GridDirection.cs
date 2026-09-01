using System;

namespace DeepSeal.Core
{
    /// <summary>
    /// Represents cardinal movement or mining direction on an integer grid
    /// 셀 그리드에서 쓰는 기본 방향과 방향 유틸리티를 제공한다.
    /// </summary>
    public enum GridDirection
    {
        None = 0,
        Up = 1,
        Right = 2,
        Down = 3,
        Left = 4    
    }

    public static class GridDirectionExtensions
    {
        public static bool IsCardinal(this GridDirection direction)
        {
            return direction == GridDirection.Up 
                || direction == GridDirection.Right 
                || direction == GridDirection.Down 
                || direction == GridDirection.Left;
        }

        public static GridPosition ToOffset(this GridDirection direction)
        {
            switch(direction)
            {
                case GridDirection.None: return GridPosition.Zero;
                case GridDirection.Up: return new GridPosition(0, 1);
                case GridDirection.Right: return new GridPosition(1, 0);
                case GridDirection.Down: return new GridPosition(0, -1);
                case GridDirection.Left: return new GridPosition(-1, 0);
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(direction), direction, "Unsupported grid direction.");
            }
        }

        public static GridDirection Opposite(this GridDirection direction)
        {
            switch(direction)
            {
                case GridDirection.None: return GridDirection.None;
                case GridDirection.Up: return GridDirection.Down;
                case GridDirection.Right: return GridDirection.Left;
                case GridDirection.Down: return GridDirection.Up;
                case GridDirection.Left: return GridDirection.Right;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(direction), direction, "Unsupported grid direction.");
            }
        }
    }
}
