using System;
using DeepSeal.Core;
using DeepSeal.Expedition;
using DeepSeal.UnityAdapters.Grid;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Treasures
{
    /// <summary>
    /// Unity adapter that displays one prototype treasure and applies pickup state to the scene object.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PrototypeTreasureView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform controlledTransform;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Treasure")]
        [SerializeField] private int treasureId;
        [SerializeField] private Vector2Int gridPosition;
        [SerializeField] private int value = 1;
        [SerializeField] private bool placeAtGridPositionOnStart = true;
        [SerializeField] private bool hideOnCollected = true;

        [Header("Debug")]
        [SerializeField] private bool logPickupResults;

        private TreasureState treasureState;
        private bool hasTreasureState;

        public bool HasTreasureState => hasTreasureState;

        public bool IsCollected => hasTreasureState && treasureState.IsCollected;

        public int Value => hasTreasureState ? treasureState.Value : value;

        public TreasureState CurrentTreasure => treasureState;

        private void Start()
        {
            EnsureReferences();

            if (!hasTreasureState)
            {
                var position = new GridPosition(gridPosition.x, gridPosition.y);
                SetTreasureState(
                    new TreasureState(treasureId, position, value),
                    placeAtGridPositionOnStart);
            }
        }

        public void Initialize(
            int id,
            GridPosition position,
            int treasureValue)
        {
            treasureId = Mathf.Max(0, id);
            gridPosition = new Vector2Int(position.X, position.Y);
            value = Mathf.Max(1, treasureValue);

            EnsureReferences();
            SetTreasureState(
                new TreasureState(treasureId, position, value),
                true);
            gameObject.SetActive(true);
        }

        public bool TryGetCurrentTreasure(out TreasureState treasure)
        {
            treasure = treasureState;
            return hasTreasureState && !treasureState.IsCollected && isActiveAndEnabled;
        }

        public bool ApplyPickupResult(TreasurePickupResult result)
        {
            if (!hasTreasureState)
            {
                Debug.LogWarning("Cannot apply treasure pickup because treasure state is unavailable.", this);
                return false;
            }

            if (!result.CurrentTreasure.IsInitialized || result.CurrentTreasure.Id != treasureState.Id)
            {
                Debug.LogWarning(
                    $"Cannot apply treasure pickup because result does not match this treasure. TreasureId={treasureState.Id}.",
                    this);
                return false;
            }

            treasureState = result.CurrentTreasure;

            if (logPickupResults)
            {
                Debug.Log(
                    $"Treasure pickup result: Id={treasureState.Id}, Succeeded={result.Succeeded}, Value={result.CollectedValue}.",
                    this);
            }

            if (!treasureState.IsCollected)
            {
                return false;
            }

            if (hideOnCollected)
            {
                gameObject.SetActive(false);
            }

            return result.Succeeded;
        }

        private void SetTreasureState(TreasureState state, bool updateTransform)
        {
            treasureState = state;
            hasTreasureState = true;

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
            value = 1;
            hideOnCollected = true;
            placeAtGridPositionOnStart = true;
        }

        private void OnValidate()
        {
            treasureId = Mathf.Max(0, treasureId);
            value = Mathf.Max(1, value);
        }
    }
}