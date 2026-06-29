using System;

namespace DeepSeal.ProceduralGeneration
{
    /// <summary>
    /// Stores a small hand-authored terrain pattern that can be blended into generated mine grids.
    /// </summary>
    public readonly struct MineTerrainPreset
    {
        private readonly MineTerrainPresetCell[] cells;

        public MineTerrainPreset(
            string id,
            int width,
            int height,
            MineTerrainPresetCell[] cells)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(
                    "Preset id cannot be null or empty.",
                    nameof(id));
            }

            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(width),
                    width,
                    "Preset width must be greater than zero.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(height),
                    height,
                    "Preset height must be greater than zero.");
            }

            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells));
            }

            int expectedCellCount = width * height;

            if (cells.Length != expectedCellCount)
            {
                throw new ArgumentException(
                    "Preset cell count must match width * height.",
                    nameof(cells));
            }

            Id = id;
            Width = width;
            Height = height;

            this.cells = new MineTerrainPresetCell[cells.Length];
            Array.Copy(cells, this.cells, cells.Length);

            int paintedCellCount = 0;

            for (int i = 0; i < this.cells.Length; i++)
            {
                if (!IsValidCell(this.cells[i]))
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(cells),
                        this.cells[i],
                        "Preset contains an unsupported cell value.");
                }

                if (this.cells[i] != MineTerrainPresetCell.Ignore)
                {
                    paintedCellCount++;
                }
            }

            if (paintedCellCount <= 0)
            {
                throw new ArgumentException(
                    "Preset must contain at least one non-ignore cell.",
                    nameof(cells));
            }

            PaintedCellCount = paintedCellCount;
        }

        public string Id { get; }

        public int Width { get; }

        public int Height { get; }

        public int PaintedCellCount { get; }

        public MineTerrainPresetCell GetCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(x),
                    "Preset local position is outside the preset bounds.");
            }

            return cells[x + y * Width];
        }

        private static bool IsValidCell(MineTerrainPresetCell cell)
        {
            return cell == MineTerrainPresetCell.Ignore
                || cell == MineTerrainPresetCell.Floor
                || cell == MineTerrainPresetCell.MineableWall
                || cell == MineTerrainPresetCell.UnmineableWall;
        }
    }
}