using System;
using DeepSeal.Upgrades;
using DeepSeal.UnityAdapters.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace DeepSeal.UnityAdapters.Upgrades
{
    /// <summary>
    /// Prototype-only OnGUI upgrade selector that spends reward drop value on temporary run upgrades.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypeUpgradeSelectionController : MonoBehaviour
    {
        [Serializable]
        private struct UpgradeOptionConfig
        {
            [SerializeField] private string displayName;
            [SerializeField] private PrototypeUpgradeEffectType effectType;
            [SerializeField] private int cost;
            [SerializeField] private float effectValue;
            [SerializeField] private string description;

            public UpgradeOptionConfig(
                string displayName,
                PrototypeUpgradeEffectType effectType,
                int cost,
                float effectValue,
                string description)
            {
                this.displayName = displayName;
                this.effectType = effectType;
                this.cost = cost;
                this.effectValue = effectValue;
                this.description = description;
            }

            public string DisplayName => string.IsNullOrEmpty(displayName) ? effectType.ToString() : displayName;

            public string Description => description ?? string.Empty;

            public int Cost => Mathf.Max(1, cost);

            public bool IsConfigured => effectType != PrototypeUpgradeEffectType.None && effectValue > 0f && cost > 0;

            public PrototypeUpgradeOption ToOption(int id)
            {
                return new PrototypeUpgradeOption(
                    id,
                    DisplayName,
                    effectType,
                    Cost,
                    effectValue);
            }
        }

        private static readonly Key[] OptionKeys =
        {
            Key.Digit1,
            Key.Digit2,
            Key.Digit3,
            Key.Digit4
        };

        [Header("References")]
        [SerializeField] private PrototypePlayerRewardDropPickup rewardDropPickup;
        [SerializeField] private PrototypePlayerAutoAttack autoAttack;
        [SerializeField] private PrototypePlayerMiningInput miningInput;
        [SerializeField] private PrototypePlayerMovement playerMovement;
        [SerializeField] private PrototypePlayerHealth playerHealth;
        [SerializeField] private PrototypePlayerExtraction playerExtraction;

        [Header("Selection")]
        [SerializeField] private bool selectionOnUpdate = true;
        [SerializeField] private bool showUpgradePanel = true;
        [SerializeField]
        private UpgradeOptionConfig[] upgradeOptions =
        {
            new UpgradeOptionConfig(
                "Sharper Strike",
                PrototypeUpgradeEffectType.AttackDamageBonus,
                3,
                1f,
                "+1 auto attack damage"),
            new UpgradeOptionConfig(
                "Longer Reach",
                PrototypeUpgradeEffectType.AttackRangeBonus,
                3,
                1f,
                "+1 auto attack range"),
            new UpgradeOptionConfig(
                "Faster Mining",
                PrototypeUpgradeEffectType.MiningIntervalMultiplier,
                2,
                0.85f,
                "Mining interval x0.85"),
            new UpgradeOptionConfig(
                "Quick Step",
                PrototypeUpgradeEffectType.MoveSpeedBonus,
                2,
                0.35f,
                "+0.35 move speed")
        };

        [Header("Runtime Debug")]
        [SerializeField] private int spentRewardValue;
        [SerializeField] private int purchasedUpgradeCount;
        [SerializeField] private string lastUpgradeMessage;

        [Header("Layout")]
        [SerializeField] private Vector2 panelOffset = new Vector2(16f, 178f);
        [SerializeField] private float panelWidth = 460f;
        [SerializeField] private float rowHeight = 24f;
        [SerializeField] private int fontSize = 15;

        [Header("Style")]
        [SerializeField] private Color panelColor = new Color(0f, 0f, 0f, 0.64f);
        [SerializeField] private Color normalTextColor = Color.white;
        [SerializeField] private Color unavailableTextColor = new Color(0.65f, 0.65f, 0.65f, 1f);
        [SerializeField] private Color successTextColor = new Color(0.35f, 1f, 0.45f, 1f);
        [SerializeField] private Color warningTextColor = new Color(1f, 0.85f, 0.25f, 1f);

        private PrototypeUpgradeState upgradeState = PrototypeUpgradeState.Empty;
        private GUIStyle labelStyle;
        private GUIStyle titleStyle;
        private int cachedFontSize;
        private bool warnedMissingRewardDropPickup;

        public int SpentRewardValue => upgradeState.SpentRewardValue;

        public int PurchasedUpgradeCount => upgradeState.PurchasedUpgradeCount;

        public int AvailableRewardValue
        {
            get
            {
                int collectedRewardValue = rewardDropPickup != null
                    ? rewardDropPickup.CollectedRewardDropValue
                    : 0;

                return PrototypeUpgradeRules.GetAvailableRewardValue(
                    upgradeState,
                    collectedRewardValue);
            }
        }

        private void Update()
        {
            if (!selectionOnUpdate || !IsSelectionAllowed())
            {
                return;
            }

            Keyboard keyboard = Keyboard.current;

            if (keyboard == null)
            {
                return;
            }

            int optionCount = Mathf.Min(GetOptionCount(), OptionKeys.Length);

            for (int i = 0; i < optionCount; i++)
            {
                KeyControl keyControl = keyboard[OptionKeys[i]];

                if (keyControl != null && keyControl.wasPressedThisFrame)
                {
                    TryPurchaseOption(i);
                    return;
                }
            }
        }

        private void OnGUI()
        {
            if (!showUpgradePanel)
            {
                return;
            }

            if (!TryGetRewardValues(out int collectedRewardValue, out int availableRewardValue))
            {
                return;
            }

            EnsureStyles();
            DrawUpgradePanel(collectedRewardValue, availableRewardValue);
        }

        [ContextMenu("Purchase First Upgrade Option")]
        public void PurchaseFirstUpgradeOption()
        {
            TryPurchaseOption(0);
        }

        private bool TryPurchaseOption(int optionIndex)
        {
            if (!IsSelectionAllowed())
            {
                return false;
            }

            if (!TryGetRewardValues(out int collectedRewardValue, out _))
            {
                return false;
            }

            if (!TryGetOption(optionIndex, out PrototypeUpgradeOption option))
            {
                return false;
            }

            if (!CanApplyUpgrade(option, out string unavailableReason))
            {
                lastUpgradeMessage = unavailableReason;
                return false;
            }

            PrototypeUpgradePurchaseResult result;

            try
            {
                result = PrototypeUpgradeRules.TryPurchase(
                    upgradeState,
                    option,
                    collectedRewardValue);
            }
            catch (ArgumentException exception)
            {
                Debug.LogError(
                    $"Prototype upgrade purchase failed because option state is invalid. {exception.Message}",
                    this);
                return false;
            }

            if (!result.Succeeded)
            {
                lastUpgradeMessage = $"Need {option.Cost} reward value for {option.DisplayName}.";
                return false;
            }

            if (!ApplyUpgrade(option, out string applyFailureReason))
            {
                lastUpgradeMessage = applyFailureReason;
                return false;
            }

            upgradeState = result.CurrentState;
            spentRewardValue = upgradeState.SpentRewardValue;
            purchasedUpgradeCount = upgradeState.PurchasedUpgradeCount;
            lastUpgradeMessage = $"Purchased {option.DisplayName}.";

            return true;
        }

        private bool TryGetRewardValues(
            out int collectedRewardValue,
            out int availableRewardValue)
        {
            collectedRewardValue = 0;
            availableRewardValue = 0;

            if (rewardDropPickup == null)
            {
                if (!warnedMissingRewardDropPickup)
                {
                    Debug.LogWarning("Prototype upgrade selection requires a Prototype Player Reward Drop Pickup reference.", this);
                    warnedMissingRewardDropPickup = true;
                }

                return false;
            }

            warnedMissingRewardDropPickup = false;
            collectedRewardValue = rewardDropPickup.CollectedRewardDropValue;
            availableRewardValue = PrototypeUpgradeRules.GetAvailableRewardValue(
                upgradeState,
                collectedRewardValue);

            return true;
        }

        private bool TryGetOption(
            int optionIndex,
            out PrototypeUpgradeOption option)
        {
            option = default;

            if (upgradeOptions == null || optionIndex < 0 || optionIndex >= upgradeOptions.Length)
            {
                return false;
            }

            UpgradeOptionConfig config = upgradeOptions[optionIndex];

            if (!config.IsConfigured)
            {
                return false;
            }

            try
            {
                option = config.ToOption(optionIndex);
                return true;
            }
            catch (ArgumentException exception)
            {
                Debug.LogError(
                    $"Prototype upgrade option is invalid. Index={optionIndex}. {exception.Message}",
                    this);
                return false;
            }
        }

        private bool CanApplyUpgrade(
            PrototypeUpgradeOption option,
            out string reason)
        {
            reason = string.Empty;

            switch (option.EffectType)
            {
                case PrototypeUpgradeEffectType.AttackDamageBonus:
                case PrototypeUpgradeEffectType.AttackRangeBonus:
                    if (autoAttack == null)
                    {
                        reason = "Cannot apply attack upgrade because Prototype Player Auto Attack is not assigned.";
                        return false;
                    }

                    return true;

                case PrototypeUpgradeEffectType.MiningIntervalMultiplier:
                    if (miningInput == null)
                    {
                        reason = "Cannot apply mining upgrade because Prototype Player Mining Input is not assigned.";
                        return false;
                    }

                    return true;

                case PrototypeUpgradeEffectType.MoveSpeedBonus:
                    if (playerMovement == null)
                    {
                        reason = "Cannot apply movement upgrade because Prototype Player Movement is not assigned.";
                        return false;
                    }

                    return true;

                default:
                    reason = $"Unsupported prototype upgrade effect type: {option.EffectType}.";
                    return false;
            }
        }

        private bool ApplyUpgrade(
            PrototypeUpgradeOption option,
            out string failureReason)
        {
            failureReason = string.Empty;

            switch (option.EffectType)
            {
                case PrototypeUpgradeEffectType.AttackDamageBonus:
                    autoAttack.AddAttackDamage(Mathf.RoundToInt(option.EffectValue));
                    return true;

                case PrototypeUpgradeEffectType.AttackRangeBonus:
                    autoAttack.AddAttackRangeCells(Mathf.RoundToInt(option.EffectValue));
                    return true;

                case PrototypeUpgradeEffectType.MiningIntervalMultiplier:
                    miningInput.MultiplyMiningInterval(option.EffectValue);
                    return true;

                case PrototypeUpgradeEffectType.MoveSpeedBonus:
                    playerMovement.AddMoveSpeed(option.EffectValue);
                    return true;

                default:
                    failureReason = $"Unsupported prototype upgrade effect type: {option.EffectType}.";
                    return false;
            }
        }

        private bool IsSelectionAllowed()
        {
            if (playerHealth != null && playerHealth.IsDefeated)
            {
                return false;
            }

            if (playerExtraction != null && playerExtraction.HasExtracted)
            {
                return false;
            }

            return true;
        }

        private int GetOptionCount()
        {
            return upgradeOptions != null ? upgradeOptions.Length : 0;
        }

        private void DrawUpgradePanel(
            int collectedRewardValue,
            int availableRewardValue)
        {
            int optionCount = Mathf.Min(GetOptionCount(), OptionKeys.Length);
            float safeRowHeight = Mathf.Max(rowHeight, fontSize + 8f);
            float paddingX = 10f;
            float paddingY = 10f;
            float panelHeight = paddingY * 2f + safeRowHeight * (optionCount + 3);

            var panelRect = new Rect(
                panelOffset.x,
                panelOffset.y,
                panelWidth,
                panelHeight);

            DrawSolidRect(panelRect, panelColor);

            var rowRect = new Rect(
                panelRect.x + paddingX,
                panelRect.y + paddingY,
                panelRect.width - paddingX * 2f,
                safeRowHeight);

            titleStyle.normal.textColor = normalTextColor;
            GUI.Label(
                rowRect,
                $"Upgrades: reward {availableRewardValue}/{collectedRewardValue} available",
                titleStyle);

            for (int i = 0; i < optionCount; i++)
            {
                rowRect.y += safeRowHeight;

                if (!TryGetOption(i, out PrototypeUpgradeOption option))
                {
                    continue;
                }

                UpgradeOptionConfig config = upgradeOptions[i];
                bool canAfford = availableRewardValue >= option.Cost;
                labelStyle.normal.textColor = canAfford ? normalTextColor : unavailableTextColor;

                GUI.Label(
                    rowRect,
                    $"[{i + 1}] {option.DisplayName} - cost {option.Cost} - {config.Description}",
                    labelStyle);
            }

            rowRect.y += safeRowHeight;
            labelStyle.normal.textColor = warningTextColor;
            GUI.Label(rowRect, string.IsNullOrEmpty(lastUpgradeMessage) ? "Collect reward drops, then press 1-4." : lastUpgradeMessage, labelStyle);

            rowRect.y += safeRowHeight;
            labelStyle.normal.textColor = successTextColor;
            GUI.Label(rowRect, $"Purchased: {upgradeState.PurchasedUpgradeCount}, spent: {upgradeState.SpentRewardValue}", labelStyle);
        }

        private void EnsureStyles()
        {
            if (labelStyle != null && cachedFontSize == fontSize)
            {
                return;
            }

            cachedFontSize = fontSize;

            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = fontSize;
            labelStyle.wordWrap = false;
            labelStyle.normal.textColor = normalTextColor;

            titleStyle = new GUIStyle(labelStyle);
            titleStyle.fontStyle = FontStyle.Bold;
        }

        private static void DrawSolidRect(Rect rect, Color color)
        {
            Color previousColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = previousColor;
        }

        private void Reset()
        {
            selectionOnUpdate = true;
            showUpgradePanel = true;
            panelOffset = new Vector2(16f, 178f);
            panelWidth = 460f;
            rowHeight = 24f;
            fontSize = 15;
            panelColor = new Color(0f, 0f, 0f, 0.64f);
            normalTextColor = Color.white;
            unavailableTextColor = new Color(0.65f, 0.65f, 0.65f, 1f);
            successTextColor = new Color(0.35f, 1f, 0.45f, 1f);
            warningTextColor = new Color(1f, 0.85f, 0.25f, 1f);
        }

        private void OnValidate()
        {
            panelWidth = Mathf.Max(260f, panelWidth);
            rowHeight = Mathf.Max(18f, rowHeight);
            fontSize = Mathf.Max(10, fontSize);
        }
    }
}