using DeepSeal.Core;
using DeepSeal.Mining;
using DeepSeal.UnityAdapters.Grid;
using DeepSeal.UnityAdapters.Prototype;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeepSeal.UnityAdapters.Player
{
    /// <summary>
    /// Minimal prototype movement over a generated MineGrid.
    /// This uses Keyboard.current directly as temporary prototype input.
    /// Replace this with InputActions when input bindings become part of the production control scheme.
    /// Collision is point-based: the player's transform position determines the occupied grid cell.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypePlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PrototypeMineGridBootstrap mineGridBootstrap;
        [SerializeField] private Transform controlledTransform;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private bool placeAtStartOnStart = true;
        [SerializeField] private bool generateGridIfMissing = true;
        [SerializeField] private bool normalizeDiagonalInput = true;
        [SerializeField] private float maxCollisionStepDistance = 0.2f;
        [SerializeField] private GridDirection initialFacingDirection = GridDirection.Right;

        private GridDirection facingDirection = GridDirection.Right;
        private bool warnedMissingBootstrap;
        private bool warnedMissingGrid;

        public GridDirection FacingDirection => facingDirection;

        public Transform ControlledTransform
        {
            get
            {
                EnsureControlledTransform();
                return controlledTransform;
            }
        }

        private void Awake()
        {
            facingDirection = NormalizeFacingDirection(initialFacingDirection);
        }

        private void Start()
        {
            EnsureControlledTransform();

            if (!TryResolveGrid(out _))
            {
                return;
            }

            if (placeAtStartOnStart)
            {
                PlaceAtMineStartPosition();
            }
        }

        private void Update()
        {
            EnsureControlledTransform();

            Vector2 input = ReadMoveInput();
            UpdateFacingFromInput(input);

            if (!TryResolveGrid(out MineGrid grid))
            {
                return;
            }

            if (input.sqrMagnitude <= 0f)
            {
                return;
            }

            if (normalizeDiagonalInput && input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }

            Vector3 delta = new Vector3(input.x, input.y, 0f) * (moveSpeed * Time.deltaTime);
            MoveWithGridCollision(grid, delta);
        }

        public bool TryGetCurrentGridPosition(out GridPosition position)
        {
            EnsureControlledTransform();

            if (controlledTransform == null)
            {
                position = default;
                return false;
            }

            position = GridCoordinateConverter.WorldToGridPosition(controlledTransform.position);
            return true;
        }

        private void PlaceAtMineStartPosition()
        {
            if (mineGridBootstrap == null)
            {
                return;
            }

            if (!mineGridBootstrap.TryGetStartPosition(out GridPosition startPosition))
            {
                Debug.LogWarning(
                    "Cannot place prototype player at start position because mine start position is unavailable.",
                    this);
                return;
            }

            float currentZ = controlledTransform.position.z;
            controlledTransform.position = GridCoordinateConverter.GridToWorldCenter(startPosition, currentZ);
        }

        private bool TryResolveGrid(out MineGrid grid)
        {
            grid = null;

            if (mineGridBootstrap == null)
            {
                if (!warnedMissingBootstrap)
                {
                    Debug.LogError(
                        "Prototype player movement requires a Prototype Mine Grid Bootstrap reference.",
                        this);
                    warnedMissingBootstrap = true;
                }

                return false;
            }

            warnedMissingBootstrap = false;

            if (!mineGridBootstrap.TryGetCurrentGrid(out grid) && generateGridIfMissing)
            {
                mineGridBootstrap.TryGenerateAndRender();
            }

            if (!mineGridBootstrap.TryGetCurrentGrid(out grid))
            {
                if (!warnedMissingGrid)
                {
                    Debug.LogWarning(
                        "Prototype player cannot move because no MineGrid has been generated yet.",
                        this);
                    warnedMissingGrid = true;
                }

                return false;
            }

            warnedMissingGrid = false;
            return true;
        }

        private void MoveWithGridCollision(MineGrid grid, Vector3 delta)
        {
            float safeStepDistance = Mathf.Max(0.01f, maxCollisionStepDistance);
            int stepCount = Mathf.Max(1, Mathf.CeilToInt(delta.magnitude / safeStepDistance));
            Vector3 step = delta / stepCount;

            for (int i = 0; i < stepCount; i++)
            {
                MoveSingleStep(grid, step);
            }
        }

        private void MoveSingleStep(MineGrid grid, Vector3 step)
        {
            Vector3 currentPosition = controlledTransform.position;

            Vector3 xTarget = currentPosition + new Vector3(step.x, 0f, 0f);

            if (CanOccupyWorldPosition(grid, xTarget))
            {
                currentPosition = xTarget;
            }

            Vector3 yTarget = currentPosition + new Vector3(0f, step.y, 0f);

            if (CanOccupyWorldPosition(grid, yTarget))
            {
                currentPosition = yTarget;
            }

            controlledTransform.position = currentPosition;
        }

        private void UpdateFacingFromInput(Vector2 input)
        {
            if (input.sqrMagnitude <= 0f)
            {
                return;
            }

            float absX = Mathf.Abs(input.x);
            float absY = Mathf.Abs(input.y);

            if (absX >= absY && input.x != 0f)
            {
                facingDirection = input.x > 0f ? GridDirection.Right : GridDirection.Left;
                return;
            }

            if (input.y != 0f)
            {
                facingDirection = input.y > 0f ? GridDirection.Up : GridDirection.Down;
            }
        }

        private static bool CanOccupyWorldPosition(MineGrid grid, Vector3 worldPosition)
        {
            GridPosition gridPosition = GridCoordinateConverter.WorldToGridPosition(worldPosition);

            if (!grid.TryGetCell(gridPosition, out TerrainCell cell))
            {
                return false;
            }

            return cell.IsPassable;
        }

        private static Vector2 ReadMoveInput()
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard == null)
            {
                return Vector2.zero;
            }

            Vector2 input = Vector2.zero;

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                input.y += 1f;
            }

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                input.y -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                input.x += 1f;
            }

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                input.x -= 1f;
            }

            return input;
        }

        private static GridDirection NormalizeFacingDirection(GridDirection direction)
        {
            return direction.IsCardinal() ? direction : GridDirection.Right;
        }

        private void EnsureControlledTransform()
        {
            if (controlledTransform == null)
            {
                controlledTransform = transform;
            }
        }

        private void Reset()
        {
            controlledTransform = transform;
            initialFacingDirection = GridDirection.Right;
            facingDirection = GridDirection.Right;
        }

        private void OnValidate()
        {
            moveSpeed = Mathf.Max(0f, moveSpeed);
            maxCollisionStepDistance = Mathf.Max(0.01f, maxCollisionStepDistance);
            initialFacingDirection = NormalizeFacingDirection(initialFacingDirection);
        }
    }
}