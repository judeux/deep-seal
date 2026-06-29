using DeepSeal.Core;
using DeepSeal.Mining;
using DeepSeal.ProceduralGeneration;
using DeepSeal.UnityAdapters.Grid;
using DeepSeal.UnityAdapters.Player;
using DeepSeal.UnityAdapters.Prototype;
using UnityEngine;

namespace DeepSeal.UnityAdapters.UI
{
    /// <summary>
    /// Prototype-only world overlay that shows durability for the wall cell the player is facing.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypeWallMiningFeedbackOverlay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PrototypePlayerMovement playerMovement;
        [SerializeField] private PrototypeMineGridBootstrap mineGridBootstrap;
        [SerializeField] private Camera targetCamera;

        [Header("Display")]
        [SerializeField] private bool showOverlay = true;
        [SerializeField] private Vector2 screenOffset = new Vector2(0f, -58f);
        [SerializeField] private Vector2 labelSize = new Vector2(132f, 44f);

        [Header("Style")]
        [SerializeField] private int fontSize = 14;
        [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0.66f);
        [SerializeField] private Color textColor = Color.white;
        [SerializeField] private Color barBackgroundColor = new Color(0.18f, 0.18f, 0.18f, 0.9f);
        [SerializeField] private Color durabilityColor = new Color(1f, 0.68f, 0.22f, 1f);

        private GUIStyle labelStyle;
        private int cachedFontSize;

        private void OnGUI()
        {
            if (!showOverlay)
            {
                return;
            }

            if (!TryGetFacingWall(out GridPosition wallPosition, out TerrainCell wallCell, out int maxDurability))
            {
                return;
            }

            Camera cameraToUse = targetCamera != null
                ? targetCamera
                : Camera.main;

            if (cameraToUse == null)
            {
                return;
            }

            Vector3 worldPosition = GridCoordinateConverter.GridToWorldCenter(wallPosition);
            Vector3 screenPosition = cameraToUse.WorldToScreenPoint(worldPosition);

            if (screenPosition.z <= 0f)
            {
                return;
            }

            EnsureStyles();

            float x = screenPosition.x - labelSize.x * 0.5f + screenOffset.x;
            float y = Screen.height - screenPosition.y - labelSize.y * 0.5f + screenOffset.y;

            var rect = new Rect(x, y, labelSize.x, labelSize.y);
            DrawSolidRect(rect, backgroundColor);

            int safeMaxDurability = Mathf.Max(1, maxDurability);
            int safeCurrentDurability = Mathf.Clamp(wallCell.Durability, 0, safeMaxDurability);
            float remainingRatio = Mathf.Clamp01((float)safeCurrentDurability / safeMaxDurability);

            var textRect = new Rect(rect.x, rect.y + 3f, rect.width, 18f);
            GUI.Label(textRect, $"Wall {safeCurrentDurability}/{safeMaxDurability}", labelStyle);

            var barBackRect = new Rect(rect.x + 8f, rect.y + 26f, rect.width - 16f, 10f);
            DrawSolidRect(barBackRect, barBackgroundColor);

            var barFillRect = new Rect(
                barBackRect.x,
                barBackRect.y,
                barBackRect.width * remainingRatio,
                barBackRect.height);

            DrawSolidRect(barFillRect, durabilityColor);
        }

        private bool TryGetFacingWall(
            out GridPosition wallPosition,
            out TerrainCell wallCell,
            out int maxDurability)
        {
            wallPosition = default;
            wallCell = default;
            maxDurability = 1;

            if (playerMovement == null || mineGridBootstrap == null)
            {
                return false;
            }

            GridDirection direction = playerMovement.FacingDirection;

            if (!direction.IsCardinal())
            {
                return false;
            }

            if (!playerMovement.TryGetCurrentGridPosition(out GridPosition playerPosition))
            {
                return false;
            }

            if (!mineGridBootstrap.TryGetCurrentGrid(out MineGrid grid))
            {
                return false;
            }

            wallPosition = playerPosition.Offset(direction);

            if (!grid.TryGetCell(wallPosition, out wallCell))
            {
                return false;
            }

            if (!wallCell.IsMineable)
            {
                return false;
            }

            maxDurability = ResolveMaxWallDurability(wallCell);
            return true;
        }

        private int ResolveMaxWallDurability(TerrainCell wallCell)
        {
            if (mineGridBootstrap != null
                && mineGridBootstrap.TryGetCurrentResult(out MineGenerationResult result))
            {
                return Mathf.Max(1, result.Settings.WallDurability);
            }

            return Mathf.Max(1, wallCell.Durability);
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
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.UpperCenter;
            labelStyle.normal.textColor = textColor;
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
            targetCamera = Camera.main;
            showOverlay = true;
            screenOffset = new Vector2(0f, -58f);
            labelSize = new Vector2(132f, 44f);
            fontSize = 14;
            backgroundColor = new Color(0f, 0f, 0f, 0.66f);
            textColor = Color.white;
            barBackgroundColor = new Color(0.18f, 0.18f, 0.18f, 0.9f);
            durabilityColor = new Color(1f, 0.68f, 0.22f, 1f);
        }

        private void OnValidate()
        {
            labelSize.x = Mathf.Max(80f, labelSize.x);
            labelSize.y = Mathf.Max(28f, labelSize.y);
            fontSize = Mathf.Max(10, fontSize);

            labelStyle = null;
        }
    }
}