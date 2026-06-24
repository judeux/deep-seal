using DeepSeal.Core;
using DeepSeal.Expedition;
using DeepSeal.UnityAdapters.Grid;
using UnityEngine;

namespace DeepSeal.UnityAdapters.RewardDrops
{
    /// <summary>
    /// Unity adapter that displays one prototype reward drop and applies pickup state to the scene object.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PrototypeRewardDropView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform controlledTransform;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Reward Drop")]
        [SerializeField] private int rewardDropId;
        [SerializeField] private Vector2Int gridPosition;
        [SerializeField] private int value = 1;
        [SerializeField] private RewardDropSource source = RewardDropSource.Unknown;
        [SerializeField] private bool placeAtGridPositionOnStart = true;
        [SerializeField] private bool hideOnCollected = true;

        [Header("Debug")]
        [SerializeField] private bool logPickupResults;

        private RewardDropState rewardDropState;
        private bool hasRewardDropState;

        public bool HasRewardDropState => hasRewardDropState;

        public bool IsCollected => hasRewardDropState && rewardDropState.IsCollected;

        public int Value => hasRewardDropState ? rewardDropState.Value : value;

        public RewardDropSource Source => hasRewardDropState ? rewardDropState.Source : source;

        public RewardDropState CurrentDrop => rewardDropState;

        private void Start()
        {
            EnsureReferences();

            if (!hasRewardDropState)
            {
                var position = new GridPosition(gridPosition.x, gridPosition.y);
                SetRewardDropState(
                    new RewardDropState(rewardDropId, position, value, source),
                    placeAtGridPositionOnStart);
            }
        }

        public void Initialize(
            int id,
            GridPosition position,
            int dropValue,
            RewardDropSource dropSource)
        {
            rewardDropId = Mathf.Max(0, id);
            gridPosition = new Vector2Int(position.X, position.Y);
            value = Mathf.Max(1, dropValue);
            source = dropSource;

            EnsureReferences();
            SetRewardDropState(
                new RewardDropState(rewardDropId, position, value, source),
                true);
            gameObject.SetActive(true);
        }

        public bool TryGetCurrentDrop(out RewardDropState drop)
        {
            drop = rewardDropState;
            return hasRewardDropState && !rewardDropState.IsCollected && isActiveAndEnabled;
        }

        public bool ApplyPickupResult(RewardDropPickupResult result)
        {
            if (!hasRewardDropState)
            {
                Debug.LogWarning("Cannot apply reward drop pickup because reward drop state is unavailable.", this);
                return false;
            }

            if (!result.CurrentDrop.IsInitialized || result.CurrentDrop.Id != rewardDropState.Id)
            {
                Debug.LogWarning(
                    $"Cannot apply reward drop pickup because result does not match this drop. DropId={rewardDropState.Id}.",
                    this);
                return false;
            }

            rewardDropState = result.CurrentDrop;

            if (logPickupResults)
            {
                Debug.Log(
                    $"Reward drop pickup result: Id={rewardDropState.Id}, Succeeded={result.Succeeded}, Value={result.CollectedValue}.",
                    this);
            }

            if (!rewardDropState.IsCollected)
            {
                return false;
            }

            if (hideOnCollected)
            {
                gameObject.SetActive(false);
            }

            return result.Succeeded;
        }

        private void SetRewardDropState(RewardDropState state, bool updateTransform)
        {
            rewardDropState = state;
            hasRewardDropState = true;

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
            source = RewardDropSource.Unknown;
            hideOnCollected = true;
            placeAtGridPositionOnStart = true;
        }

        private void OnValidate()
        {
            rewardDropId = Mathf.Max(0, rewardDropId);
            value = Mathf.Max(1, value);
        }
    }
}