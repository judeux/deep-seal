using System;

namespace DeepSeal.Mining
{
    /// <summary>
    /// Stores the terrain state of a single mine grid cell.
    /// Floor is passable, Wall is mineable, and Void is outside the generated map footprint.
    /// </summary>
    public readonly struct TerrainCell : IEquatable<TerrainCell>
    {
        private TerrainCell(TerrainCellType type, int durability)
        {
            Type = type;
            Durability = durability;
        }

        public TerrainCellType Type { get; }

        public int Durability { get; }

        public bool IsPassable => Type == TerrainCellType.Floor;

        public bool IsMineable => Type == TerrainCellType.Wall && Durability > 0;

        public static TerrainCell Floor => new TerrainCell(TerrainCellType.Floor, 0);

        public static TerrainCell Void => new TerrainCell(TerrainCellType.Void, 0);

        public static TerrainCell Wall(int durability)
        {
            if (durability <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(durability),
                    durability,
                    "Wall durability must be greater than zero.");
            }

            return new TerrainCell(TerrainCellType.Wall, durability);
        }

        public bool Equals(TerrainCell other)
        {
            return Type == other.Type && Durability == other.Durability;
        }

        public override bool Equals(object obj)
        {
            return obj is TerrainCell other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Type * 397) ^ Durability;
            }
        }

        public override string ToString()
        {
            return $"{Type}({Durability})";
        }

        public static bool operator ==(TerrainCell left, TerrainCell right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TerrainCell left, TerrainCell right)
        {
            return !left.Equals(right);
        }
    }
}