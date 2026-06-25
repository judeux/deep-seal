using System;

namespace DeepSeal.Upgrades
{
    /// <summary>
    /// Pure value definition for one selectable prototype upgrade option.
    /// </summary>
    public readonly struct PrototypeUpgradeOption : IEquatable<PrototypeUpgradeOption>
    {
        public PrototypeUpgradeOption(
            int id,
            string displayName,
            PrototypeUpgradeEffectType effectType,
            int cost,
            float effectValue)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, "Upgrade id must be zero or greater.");
            }

            if (effectType == PrototypeUpgradeEffectType.None)
            {
                throw new ArgumentException("Upgrade effect type must not be None.", nameof(effectType));
            }

            if (cost <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cost), cost, "Upgrade cost must be greater than zero.");
            }

            if (float.IsNaN(effectValue) || float.IsInfinity(effectValue) || effectValue <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(effectValue), effectValue, "Upgrade effect value must be a positive finite number.");
            }

            Id = id;
            DisplayName = string.IsNullOrEmpty(displayName) ? effectType.ToString() : displayName;
            EffectType = effectType;
            Cost = cost;
            EffectValue = effectValue;
        }

        public int Id { get; }

        public string DisplayName { get; }

        public PrototypeUpgradeEffectType EffectType { get; }

        public int Cost { get; }

        public float EffectValue { get; }

        public bool IsInitialized => Cost > 0 && EffectType != PrototypeUpgradeEffectType.None && EffectValue > 0f;

        public bool Equals(PrototypeUpgradeOption other)
        {
            return Id == other.Id
                && string.Equals(DisplayName, other.DisplayName, StringComparison.Ordinal)
                && EffectType == other.EffectType
                && Cost == other.Cost
                && EffectValue.Equals(other.EffectValue);
        }

        public override bool Equals(object obj)
        {
            return obj is PrototypeUpgradeOption other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id;
                hashCode = (hashCode * 397) ^ (DisplayName != null ? DisplayName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)EffectType;
                hashCode = (hashCode * 397) ^ Cost;
                hashCode = (hashCode * 397) ^ EffectValue.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{DisplayName} ({EffectType}), cost={Cost}, value={EffectValue}";
        }

        public static bool operator ==(PrototypeUpgradeOption left, PrototypeUpgradeOption right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PrototypeUpgradeOption left, PrototypeUpgradeOption right)
        {
            return !left.Equals(right);
        }
    }
}