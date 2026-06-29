using System;

namespace DeepSeal.Mining
{
    /// <summary>
    /// Stores the terrain state of a single mine grid cell.
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

        public bool IsVoid => Type == TerrainCellType.Void;

        public bool IsWall => Type == TerrainCellType.Wall
            || Type == TerrainCellType.MineableWall
            || Type == TerrainCellType.UnmineableWall
            || Type == TerrainCellType.BoundaryWall;

        public bool IsMineable => Type == TerrainCellType.MineableWall && Durability > 0;

        public bool IsBoundaryWall => Type == TerrainCellType.BoundaryWall;

        public bool IsUnmineableWall => Type == TerrainCellType.UnmineableWall
            || Type == TerrainCellType.BoundaryWall;

        public static TerrainCell Floor => new TerrainCell(TerrainCellType.Floor, 0);

        public static TerrainCell Void => new TerrainCell(TerrainCellType.Void, 0);

        public static TerrainCell UnmineableWall => new TerrainCell(TerrainCellType.UnmineableWall, 0);

        public static TerrainCell BoundaryWall => new TerrainCell(TerrainCellType.BoundaryWall, 0);

        public static TerrainCell Wall(int durability)
        {
            return MineableWall(durability);
        }

        public static TerrainCell MineableWall(int durability)
        {
            if (durability <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(durability),
                    durability,
                    "Mineable wall durability must be greater than zero.");
            }

            return new TerrainCell(TerrainCellType.MineableWall, durability);
        }

        public TerrainCell WithDurability(int durability)
        {
            if (!IsMineable)
            {
                throw new InvalidOperationException("Only mineable walls can change durability.");
            }

            return MineableWall(durability);
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