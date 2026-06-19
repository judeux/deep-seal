using System;
using DeepSeal.Core;

namespace DeepSeal.Combat
{
    /// <summary>
    /// Represents one enemy's pure domain state on the mine grid.
    /// </summary>
    public readonly struct EnemyState : IEquatable<EnemyState>
    {
        public EnemyState(int id, GridPosition position)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id),
                    id,
                    "Enemy id must be zero or greater.");
            }

            Id = id;
            Position = position;
        }

        public int Id { get; }

        public GridPosition Position { get; }

        public EnemyState WithPosition(GridPosition position)
        {
            return new EnemyState(Id, position);
        }

        public bool Equals(EnemyState other)
        {
            return Id == other.Id && Position == other.Position;
        }

        public override bool Equals(object obj)
        {
            return obj is EnemyState other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ Position.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"Enemy({Id}) at {Position}";
        }

        public static bool operator ==(EnemyState left, EnemyState right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EnemyState left, EnemyState right)
        {
            return !left.Equals(right);
        }
    }
}