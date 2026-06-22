using System;
using DeepSeal.Core;

namespace DeepSeal.Expedition
{
    /// <summary>
    /// Pure value state for one treasure marker during a prototype expedition.
    /// </summary>
    public readonly struct TreasureState : IEquatable<TreasureState>
    {
        public TreasureState(
            int id,
            GridPosition position,
            int value,
            bool isCollected = false)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id),
                    id,
                    "Treasure id must be zero or greater.");
            }

            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "Treasure value must be greater than zero.");
            }

            Id = id;
            Position = position;
            Value = value;
            IsCollected = isCollected;
        }

        public int Id { get; }

        public GridPosition Position { get; }

        public int Value { get; }

        public bool IsCollected { get; }

        public bool IsInitialized => Value > 0;

        public TreasureState Collect()
        {
            return new TreasureState(Id, Position, Value, true);
        }

        public bool Equals(TreasureState other)
        {
            return Id == other.Id
                && Position == other.Position
                && Value == other.Value
                && IsCollected == other.IsCollected;
        }

        public override bool Equals(object obj)
        {
            return obj is TreasureState other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id;
                hashCode = (hashCode * 397) ^ Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Value;
                hashCode = (hashCode * 397) ^ IsCollected.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            string status = IsCollected ? "collected" : "available";
            return $"Treasure({Id}) at {Position}, value={Value}, {status}";
        }

        public static bool operator ==(TreasureState left, TreasureState right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TreasureState left, TreasureState right)
        {
            return !left.Equals(right);
        }
    }
}