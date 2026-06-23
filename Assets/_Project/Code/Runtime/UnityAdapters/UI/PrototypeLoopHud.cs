using DeepSeal.UnityAdapters.Player;
using UnityEngine;

namespace DeepSeal.UnityAdapters.UI
{
    /// <summary>
    /// Prototype-only OnGUI HUD for the first playable loop.
    /// This is intentionally temporary and should be replaced by a proper UI stack later.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypeLoopHud : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PrototypePlayerHealth playerHealth;
        [SerializeField] private PrototypePlayerTreasurePickup treasurePickup;
        [SerializeField] private PrototypePlayerExtraction playerExtraction;

        [Header("Display")]
        [SerializeField] private bool showHud = true;
        [SerializeField] private bool showCenterLoopResult = true;

        [Header("Layout")]
        [SerializeField] private Vector2 topLeftOffset = new Vector2(16f, 16f);
        [SerializeField] private float panelWidth = 340f;
        [SerializeField] private float lineHeight = 24f;

        [Header("Style")]
        [SerializeField] private int fontSize = 16;
        [SerializeField] private Color panelColor = new Color(0f, 0f, 0f, 0.62f);
        [SerializeField] private Color normalTextColor = Color.white;
        [SerializeField] private Color warningTextColor = new Color(1f, 0.85f, 0.25f, 1f);
        [SerializeField] private Color successTextColor = new Color(0.35f, 1f, 0.45f, 1f);
        [SerializeField] private Color dangerTextColor = new Color(1f, 0.35f, 0.35f, 1f);

        private GUIStyle labelStyle;
        private GUIStyle statusStyle;
        private GUIStyle centerStyle;
        private int cachedFontSize;

        private void OnGUI()
        {
            if (!showHud)
            {
                return;
            }

            EnsureStyles();
            DrawHudPanel();

            if (showCenterLoopResult)
            {
                DrawCenterLoopResult();
            }
        }

        private void DrawHudPanel()
        {
            float rowHeight = Mathf.Max(lineHeight, fontSize + 10f);
            float rowGap = 2f;
            float paddingX = 10f;
            float paddingY = 10f;
            float panelHeight = paddingY * 2f + rowHeight * 4f + rowGap * 3f;

            var panelRect = new Rect(
                topLeftOffset.x,
                topLeftOffset.y,
                panelWidth,
                panelHeight);

            DrawSolidRect(panelRect, panelColor);

            var rowRect = new Rect(
                panelRect.x + paddingX,
                panelRect.y + paddingY,
                panelRect.width - paddingX * 2f,
                rowHeight);

            DrawHudLine(rowRect, BuildHealthText(), labelStyle, normalTextColor);

            rowRect.y += rowHeight + rowGap;
            DrawHudLine(rowRect, BuildTreasureText(), labelStyle, normalTextColor);

            rowRect.y += rowHeight + rowGap;
            DrawHudLine(rowRect, BuildExtractionText(), statusStyle, GetExtractionColor());

            rowRect.y += rowHeight + rowGap;
            DrawHudLine(rowRect, BuildLoopStateText(), statusStyle, GetLoopStateColor());
        }

        private static void DrawHudLine(
            Rect rect,
            string text,
            GUIStyle style,
            Color color)
        {
            style.normal.textColor = color;
            GUI.Label(rect, text, style);
        }

        private string BuildHealthText()
        {
            if (playerHealth == null)
            {
                return "HP: unassigned";
            }

            return $"HP: {playerHealth.CurrentHitPoints}/{playerHealth.MaxHitPoints}";
        }

        private string BuildTreasureText()
        {
            if (treasurePickup == null)
            {
                return "Treasure: unassigned";
            }

            return $"Treasure: {treasurePickup.CollectedTreasureCount} collected, value {treasurePickup.CollectedTreasureValue}";
        }

        private string BuildExtractionText()
        {
            if (playerExtraction == null)
            {
                return "Extraction: unassigned";
            }

            if (playerHealth != null && playerHealth.IsDefeated)
            {
                return "Extraction: unavailable - defeated";
            }

            if (playerExtraction.HasExtracted)
            {
                return $"Extraction: complete, value {playerExtraction.ExtractedTreasureValue}";
            }

            int requiredValue = playerExtraction.RequiredTreasureValue;
            int collectedValue = treasurePickup != null
                ? treasurePickup.CollectedTreasureValue
                : 0;

            if (collectedValue >= requiredValue)
            {
                return "Extraction: ready - return to marker";
            }

            return $"Extraction: need treasure {collectedValue}/{requiredValue}";
        }

        private string BuildLoopStateText()
        {
            if (playerHealth != null && playerHealth.IsDefeated)
            {
                return "Loop: defeated";
            }

            if (playerExtraction != null && playerExtraction.HasExtracted)
            {
                return "Loop: extraction complete";
            }

            return "Loop: active";
        }

        private Color GetExtractionColor()
        {
            if (playerExtraction == null)
            {
                return warningTextColor;
            }

            if (playerHealth != null && playerHealth.IsDefeated)
            {
                return dangerTextColor;
            }

            if (playerExtraction.HasExtracted)
            {
                return successTextColor;
            }

            int requiredValue = playerExtraction.RequiredTreasureValue;
            int collectedValue = treasurePickup != null
                ? treasurePickup.CollectedTreasureValue
                : 0;

            return collectedValue >= requiredValue
                ? successTextColor
                : warningTextColor;
        }

        private Color GetLoopStateColor()
        {
            if (playerHealth != null && playerHealth.IsDefeated)
            {
                return dangerTextColor;
            }

            if (playerExtraction != null && playerExtraction.HasExtracted)
            {
                return successTextColor;
            }

            return normalTextColor;
        }

        private void DrawCenterLoopResult()
        {
            if (playerHealth != null && playerHealth.IsDefeated)
            {
                DrawCenterMessage("DEFEATED", dangerTextColor);
                return;
            }

            if (playerExtraction != null && playerExtraction.HasExtracted)
            {
                DrawCenterMessage("EXTRACTION COMPLETE", successTextColor);
            }
        }

        private void DrawCenterMessage(string message, Color color)
        {
            centerStyle.normal.textColor = color;

            var rect = new Rect(
                0f,
                Screen.height * 0.38f,
                Screen.width,
                96f);

            GUI.Label(rect, message, centerStyle);
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
            labelStyle.normal.textColor = normalTextColor;
            labelStyle.wordWrap = false;

            statusStyle = new GUIStyle(labelStyle);

            centerStyle = new GUIStyle(GUI.skin.label);
            centerStyle.fontSize = fontSize * 2;
            centerStyle.fontStyle = FontStyle.Bold;
            centerStyle.alignment = TextAnchor.MiddleCenter;
            centerStyle.normal.textColor = normalTextColor;
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
            showHud = true;
            showCenterLoopResult = true;
            topLeftOffset = new Vector2(16f, 16f);
            panelWidth = 340f;
            lineHeight = 28f;
            fontSize = 16;
            panelColor = new Color(0f, 0f, 0f, 0.62f);
            normalTextColor = Color.white;
            warningTextColor = new Color(1f, 0.85f, 0.25f, 1f);
            successTextColor = new Color(0.35f, 1f, 0.45f, 1f);
            dangerTextColor = new Color(1f, 0.35f, 0.35f, 1f);
        }

        private void OnValidate()
        {
            panelWidth = Mathf.Max(180f, panelWidth);
            fontSize = Mathf.Max(10, fontSize);
            lineHeight = Mathf.Max(fontSize + 10f, lineHeight);

            labelStyle = null;
            statusStyle = null;
            centerStyle = null;
        }
    }
}