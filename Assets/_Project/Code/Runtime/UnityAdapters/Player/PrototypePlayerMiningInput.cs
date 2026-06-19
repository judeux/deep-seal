using DeepSeal.Core;
using DeepSeal.Mining;
using DeepSeal.UnityAdapters.Prototype;
using DeepSeal.UnityAdapters.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace DeepSeal.UnityAdapters.Player
{
    /// <summary>
    /// Minimal prototype mining input.
    /// This uses Keyboard.current directly as temporary prototype input.
    /// Replace this with InputActions when production input bindings are introduced.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypePlayerMiningInput : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PrototypePlayerMovement playerMovement;
        [SerializeField] private PrototypeMineGridBootstrap mineGridBootstrap;
        [SerializeField] private MineGridTilemapRenderer mineGridRenderer;

        [Header("Input")]
        [SerializeField] private Key mineKey = Key.Space;
        [SerializeField] private bool repeatWhileHeld = true;
        [SerializeField] private float miningIntervalSeconds = 0.25f;

        [Header("Mining")]
        [SerializeField] private int miningDamage = 1;
        [SerializeField] private bool logMiningResults;

        private float nextAllowedMiningTime;
        private bool wasMineInputPressedLastFrame;

        private void Update()
        {
            bool isMineInputPressed = IsMineInputPressed();
            bool pressedThisFrame = isMineInputPressed && !wasMineInputPressedLastFrame;

            wasMineInputPressedLastFrame = isMineInputPressed;

            if (!isMineInputPressed)
            {
                return;
            }

            if (Time.time < nextAllowedMiningTime)
            {
                return;
            }

            if (!pressedThisFrame && !repeatWhileHeld)
            {
                return;
            }

            TryMineFacingCell();
            nextAllowedMiningTime = Time.time + miningIntervalSeconds;
        }

        [ContextMenu("Mine Facing Cell")]
        public void MineFacingCellFromContextMenu()
        {
            TryMineFacingCell();
        }

        private bool TryMineFacingCell()
        {
            if (!TryResolveReferences(out MineGrid grid))
            {
                return false;
            }

            GridDirection direction = playerMovement.FacingDirection;

            if (!direction.IsCardinal())
            {
                Debug.LogWarning(
                    $"Cannot mine because player facing direction is not cardinal. Direction={direction}.",
                    this);
                return false;
            }

            if (!playerMovement.TryGetCurrentGridPosition(out GridPosition playerPosition))
            {
                Debug.LogWarning(
                    "Cannot mine because the player grid position is unavailable.",
                    this);
                return false;
            }

            GridPosition targetPosition = playerPosition.Offset(direction);
            MiningResult result;

            try
            {
                result = MiningRules.ApplyMiningDamage(grid, targetPosition, miningDamage);
            }
            catch (System.ArgumentException exception)
            {
                Debug.LogError(
                    $"Mining attempt failed because mining settings are invalid. {exception.Message}",
                    this);
                return false;
            }

            if (result.ChangedCell)
            {
                // TODO: Replace this full re-render with dirty-cell tile updates when terrain changes become frequent.
                mineGridRenderer.Render(grid);
            }

            if (logMiningResults)
            {
                Debug.Log(
                    $"Mining result: Type={result.Type}, Position={result.Position}, Damage={result.Damage}, Previous={result.PreviousCell}, Current={result.CurrentCell}.",
                    this);
            }

            return result.Succeeded;
        }

        private bool TryResolveReferences(out MineGrid grid)
        {
            grid = null;

            if (playerMovement == null)
            {
                Debug.LogError("Cannot mine because Prototype Player Movement is not assigned.", this);
                return false;
            }

            if (mineGridBootstrap == null)
            {
                Debug.LogError("Cannot mine because Prototype Mine Grid Bootstrap is not assigned.", this);
                return false;
            }

            if (mineGridRenderer == null)
            {
                Debug.LogError("Cannot mine because Mine Grid Tilemap Renderer is not assigned.", this);
                return false;
            }

            if (!mineGridBootstrap.TryGetCurrentGrid(out grid))
            {
                Debug.LogWarning("Cannot mine because no MineGrid has been generated yet.", this);
                return false;
            }

            return true;
        }

        private bool IsMineInputPressed()
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard == null)
            {
                return false;
            }

            KeyControl mineKeyControl = keyboard[mineKey];

            return mineKeyControl != null && mineKeyControl.isPressed;
        }

        private void Reset()
        {
            playerMovement = GetComponent<PrototypePlayerMovement>();
        }

        private void OnValidate()
        {
            miningDamage = Mathf.Max(1, miningDamage);
            miningIntervalSeconds = Mathf.Max(0.01f, miningIntervalSeconds);

            if (mineKey == Key.None)
            {
                mineKey = Key.Space;
            }
        }
    }
}