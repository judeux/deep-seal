using System;
using DeepSeal.Core;

namespace DeepSeal.Expedition
{
    /// <summary>
    /// Pure value state for one prototype reward drop on the expedition grid.
    /// </summary>
    public readonly struct RewardDropState : IEquatable<RewardDropState>
    {
        public RewardDropState(
            int id,
            GridPosition position,
            int value,
            RewardDropSource source,
            bool isCollected = false)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id),
                    id,
                    "Reward drop id must be zero or greater.");
            }

            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "Reward drop value must be greater than zero.");
            }

            Id = id;
            Position = position;
            Value = value;
            Source = source;
            IsCollected = isCollected;
        }

        public int Id { get; }

        public GridPosition Position { get; }

        public int Value { get; }

        public RewardDropSource Source { get; }

        public bool IsCollected { get; }

        public bool IsInitialized => Value > 0;

        public RewardDropState Collect()
        {
            return new RewardDropState(Id, Position, Value, Source, true);
        }

        public bool Equals(RewardDropState other)
        {
            return Id == other.Id
                && Position == other.Position
                && Value == other.Value
                && Source == other.Source
                && IsCollected == other.IsCollected;
        }

        public override bool Equals(object obj)
        {
            return obj is RewardDropState other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id;
                hashCode = (hashCode * 397) ^ Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Value;
                hashCode = (hashCode * 397) ^ (int)Source;
                hashCode = (hashCode * 397) ^ IsCollected.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            string status = IsCollected ? "collected" : "available";
            return $"RewardDrop({Id}) at {Position}, value={Value}, source={Source}, {status}";
        }

        public static bool operator ==(RewardDropState left, RewardDropState right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RewardDropState left, RewardDropState right)
        {
            return !left.Equals(right);
        }
    }
}