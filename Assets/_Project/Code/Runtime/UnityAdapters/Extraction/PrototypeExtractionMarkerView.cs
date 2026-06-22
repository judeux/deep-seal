using DeepSeal.Core;
using DeepSeal.Expedition;
using DeepSeal.UnityAdapters.Grid;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Extraction
{
    /// <summary>
    /// Unity adapter that displays one prototype extraction marker and applies extraction state to the scene object.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PrototypeExtractionMarkerView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform controlledTransform;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Extraction Marker")]
        [SerializeField] private int markerId;
        [SerializeField] private Vector2Int gridPosition;
        [SerializeField] private bool isCompleted;
        [SerializeField] private bool placeAtGridPositionOnStart = true;
        [SerializeField] private bool hideOnCompleted;

        [Header("Visuals")]
        [SerializeField] private bool tintOnCompleted = true;
        [SerializeField] private Color availableTint = Color.white;
        [SerializeField] private Color completedTint = new Color(0.25f, 1f, 0.45f, 1f);

        [Header("Debug")]
        [SerializeField] private bool logExtractionResults;

        private ExtractionMarkerState markerState;
        private bool hasMarkerState;

        public bool HasMarkerState => hasMarkerState;

        public bool IsCompleted => hasMarkerState && markerState.IsCompleted;

        public ExtractionMarkerState CurrentMarker => markerState;

        private void Start()
        {
            EnsureReferences();

            if (!hasMarkerState)
            {
                var position = new GridPosition(gridPosition.x, gridPosition.y);
                SetMarkerState(
                    new ExtractionMarkerState(markerId, position, isCompleted),
                    placeAtGridPositionOnStart);
            }

            ApplyVisualState();
        }

        public void Initialize(
            int id,
            GridPosition position)
        {
            markerId = Mathf.Max(0, id);
            gridPosition = new Vector2Int(position.X, position.Y);
            isCompleted = false;

            EnsureReferences();
            SetMarkerState(
                new ExtractionMarkerState(markerId, position),
                true);
            gameObject.SetActive(true);
            ApplyVisualState();
        }

        public bool TryGetCurrentMarker(out ExtractionMarkerState marker)
        {
            marker = markerState;
            return hasMarkerState && isActiveAndEnabled;
        }

        public bool ApplyExtractionResult(ExtractionResult result)
        {
            if (!hasMarkerState)
            {
                Debug.LogWarning("Cannot apply extraction result because marker state is unavailable.", this);
                return false;
            }

            if (!result.CurrentMarker.IsInitialized || result.CurrentMarker.Id != markerState.Id)
            {
                Debug.LogWarning(
                    $"Cannot apply extraction result because result does not match this marker. MarkerId={markerState.Id}.",
                    this);
                return false;
            }

            SetMarkerState(result.CurrentMarker, false);
            ApplyVisualState();

            if (logExtractionResults)
            {
                Debug.Log(
                    $"Extraction result: Id={markerState.Id}, Succeeded={result.Succeeded}, CarriedTreasureValue={result.CarriedTreasureValue}.",
                    this);
            }

            return result.Succeeded;
        }

        private void SetMarkerState(ExtractionMarkerState state, bool updateTransform)
        {
            markerState = state;
            hasMarkerState = true;

            markerId = state.Id;
            gridPosition = new Vector2Int(state.Position.X, state.Position.Y);
            isCompleted = state.IsCompleted;

            if (updateTransform)
            {
                SyncTransformToGridPosition(state.Position);
            }
        }

        private void SyncTransformToGridPosition(GridPosition position)
        {
            EnsureReferences();

            if (controlledTransform == null)
            {
                return;
            }

            float z = controlledTransform.position.z;
            controlledTransform.position = GridCoordinateConverter.GridToWorldCenter(position, z);
        }

        private void ApplyVisualState()
        {
            EnsureReferences();

            if (spriteRenderer != null && tintOnCompleted)
            {
                spriteRenderer.color = IsCompleted ? completedTint : availableTint;
            }

            if (hideOnCompleted && IsCompleted)
            {
                gameObject.SetActive(false);
            }
        }

        private void EnsureReferences()
        {
            if (controlledTransform == null)
            {
                controlledTransform = transform;
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        private void Reset()
        {
            controlledTransform = transform;
            spriteRenderer = GetComponent<SpriteRenderer>();
            placeAtGridPositionOnStart = true;
            hideOnCompleted = false;
            tintOnCompleted = true;
            availableTint = Color.white;
            completedTint = new Color(0.25f, 1f, 0.45f, 1f);
        }

        private void OnValidate()
        {
            markerId = Mathf.Max(0, markerId);
        }
    }
}