using System;
using DeepSeal.Core;

namespace DeepSeal.Mining
{
    /// <summary>
    /// Stores mutable terrain cells for a rectangular integer mine grid.
    /// 2D 정수 좌표 기반의 지형 셀 배열을 보관하고, 범위 검사화 안전한 조회/갱신 API를 제공한다.
    /// </summary>
    public sealed class MineGrid
    {
        private readonly TerrainCell[,] cells;

        public MineGrid(int width, int height, TerrainCell defaultCell)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(width),
                    width,
                    "Grid width must be greater than zero.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(height),
                    height,
                    "Grid height must be greater than zero.");
            }

            Width = width;
            Height = height;
            cells = new TerrainCell[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    cells[x, y] = defaultCell;
                }
            }
        }

        public int Width { get; }

        public int Height { get; }

        public bool Contains(GridPosition position)
        {
            return position.X >= 0
                && position.Y >= 0
                && position.X < Width
                && position.Y < Height;
        }

        public bool TryGetCell(GridPosition position, out TerrainCell cell)
        {
            if (!Contains(position))
            {
                cell = default;
                return false;
            }

            cell = cells[position.X, position.Y];
            return true;
        }

        public bool TrySetCell(GridPosition position, TerrainCell cell)
        {
            if (!Contains(position))
            {
                return false;
            }

            cells[position.X, position.Y] = cell;
            return true;
        }

    }
}
