using System;
using DeepSeal.Core;

namespace DeepSeal.Expedition
{
    /// <summary>
    /// Pure value state for one extraction marker during a prototype expedition.
    /// </summary>
    public readonly struct ExtractionMarkerState : IEquatable<ExtractionMarkerState>
    {
        private readonly bool isInitialized;

        public ExtractionMarkerState(
            int id,
            GridPosition position,
            bool isCompleted = false)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id),
                    id,
                    "Extraction marker id must be zero or greater.");
            }

            Id = id;
            Position = position;
            IsCompleted = isCompleted;
            isInitialized = true;
        }

        public int Id { get; }

        public GridPosition Position { get; }

        public bool IsCompleted { get; }

        public bool IsInitialized => isInitialized;

        public ExtractionMarkerState Complete()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Extraction marker state must be initialized.");
            }

            return new ExtractionMarkerState(Id, Position, true);
        }

        public bool Equals(ExtractionMarkerState other)
        {
            return Id == other.Id
                && Position == other.Position
                && IsCompleted == other.IsCompleted
                && IsInitialized == other.IsInitialized;
        }

        public override bool Equals(object obj)
        {
            return obj is ExtractionMarkerState other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id;
                hashCode = (hashCode * 397) ^ Position.GetHashCode();
                hashCode = (hashCode * 397) ^ IsCompleted.GetHashCode();
                hashCode = (hashCode * 397) ^ IsInitialized.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            if (!IsInitialized)
            {
                return "ExtractionMarker(uninitialized)";
            }

            string status = IsCompleted ? "completed" : "available";
            return $"ExtractionMarker({Id}) at {Position}, {status}";
        }

        public static bool operator ==(ExtractionMarkerState left, ExtractionMarkerState right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ExtractionMarkerState left, ExtractionMarkerState right)
        {
            return !left.Equals(right);
        }
    }
}