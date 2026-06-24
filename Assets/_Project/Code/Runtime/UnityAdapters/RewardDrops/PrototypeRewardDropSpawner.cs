using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Expedition;
using DeepSeal.Mining;
using DeepSeal.UnityAdapters.Grid;
using DeepSeal.UnityAdapters.Prototype;
using UnityEngine;

namespace DeepSeal.UnityAdapters.RewardDrops
{
    /// <summary>
    /// Prototype-only Unity adapter that creates visible reward drops from enemy defeat and mining events.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypeRewardDropSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PrototypeMineGridBootstrap mineGridBootstrap;
        [SerializeField] private PrototypeRewardDropView rewardDropPrefab;
        [SerializeField] private Transform spawnParent;

        [Header("Values")]
        [SerializeField] private int firstRewardDropId;
        [SerializeField] private int enemyDefeatRewardValue = 1;
        [SerializeField] private int miningRewardValue = 1;

        [Header("Mining Drops")]
        [Range(0, 100)]
        [SerializeField] private int miningDropChancePercent = 35;
        [SerializeField] private int randomSeed = 2401;

        [Header("Spawn Rules")]
        [SerializeField] private bool preventDuplicateActiveDropOnSameCell = true;

        [Header("Debug")]
        [SerializeField] private bool logSpawnResults;

        private readonly List<PrototypeRewardDropView> spawnedDrops = new List<PrototypeRewardDropView>();
        private System.Random random;
        private bool hasInitializedRandom;
        private int nextRewardDropId;

        public IReadOnlyList<PrototypeRewardDropView> SpawnedDrops => spawnedDrops;

        private void Awake()
        {
            EnsureRandomInitialized();
        }

        public bool TrySpawnEnemyDefeatRewardDrop(GridPosition position)
        {
            return TrySpawnRewardDrop(
                position,
                enemyDefeatRewardValue,
                RewardDropSource.EnemyDefeat,
                true);
        }

        public bool TrySpawnMiningRewardDrop(GridPosition position)
        {
            EnsureRandomInitialized();

            if (!RollChance(miningDropChancePercent))
            {
                return false;
            }

            return TrySpawnRewardDrop(
                position,
                miningRewardValue,
                RewardDropSource.Mining,
                true);
        }

        public void RemoveInactiveRewardDropReferences()
        {
            for (int i = spawnedDrops.Count - 1; i >= 0; i--)
            {
                PrototypeRewardDropView drop = spawnedDrops[i];

                if (drop == null || drop.IsCollected || !drop.gameObject.activeInHierarchy)
                {
                    spawnedDrops.RemoveAt(i);
                }
            }
        }

        private bool TrySpawnRewardDrop(
            GridPosition position,
            int dropValue,
            RewardDropSource source,
            bool requirePassableCell)
        {
            EnsureRandomInitialized();
            RemoveInactiveRewardDropReferences();

            if (rewardDropPrefab == null)
            {
                Debug.LogError("Cannot spawn prototype reward drop because Reward Drop Prefab is not assigned.", this);
                return false;
            }

            if (requirePassableCell && !CanSpawnAt(position))
            {
                if (logSpawnResults)
                {
                    Debug.Log(
                        $"Skipped reward drop spawn at {position}. Cell is blocked or unavailable. Source={source}.",
                        this);
                }

                return false;
            }

            if (preventDuplicateActiveDropOnSameCell && HasActiveDropAt(position))
            {
                if (logSpawnResults)
                {
                    Debug.Log(
                        $"Skipped reward drop spawn at {position}. Active reward drop already exists on this cell.",
                        this);
                }

                return false;
            }

            Transform parent = spawnParent != null ? spawnParent : transform;
            int rewardDropId = AllocateRewardDropId();
            Vector3 worldPosition = GridCoordinateConverter.GridToWorldCenter(position);

            PrototypeRewardDropView dropView = Instantiate(
                rewardDropPrefab,
                worldPosition,
                Quaternion.identity,
                parent);

            dropView.name = $"PrototypeRewardDrop_{rewardDropId}";
            dropView.Initialize(
                rewardDropId,
                position,
                dropValue,
                source);

            spawnedDrops.Add(dropView);

            if (logSpawnResults)
            {
                Debug.Log(
                    $"Spawned reward drop. Id={rewardDropId}, Source={source}, Value={dropValue}, Position={position}.",
                    this);
            }

            return true;
        }

        private bool CanSpawnAt(GridPosition position)
        {
            if (mineGridBootstrap == null)
            {
                Debug.LogError("Cannot validate reward drop spawn because Prototype Mine Grid Bootstrap is not assigned.", this);
                return false;
            }

            if (!mineGridBootstrap.TryGetCurrentGrid(out MineGrid grid))
            {
                Debug.LogWarning("Cannot validate reward drop spawn because no MineGrid has been generated yet.", this);
                return false;
            }

            return grid.TryGetCell(position, out TerrainCell cell) && cell.IsPassable;
        }

        private bool HasActiveDropAt(GridPosition position)
        {
            for (int i = 0; i < spawnedDrops.Count; i++)
            {
                PrototypeRewardDropView drop = spawnedDrops[i];

                if (drop == null || drop.IsCollected || !drop.isActiveAndEnabled)
                {
                    continue;
                }

                if (drop.TryGetCurrentDrop(out RewardDropState state) && state.Position == position)
                {
                    return true;
                }
            }

            return false;
        }

        private int AllocateRewardDropId()
        {
            int id = nextRewardDropId;
            nextRewardDropId++;
            return id;
        }

        private bool RollChance(int chancePercent)
        {
            if (chancePercent <= 0)
            {
                return false;
            }

            if (chancePercent >= 100)
            {
                return true;
            }

            return random.Next(0, 100) < chancePercent;
        }

        private void EnsureRandomInitialized()
        {
            if (hasInitializedRandom)
            {
                return;
            }

            random = new System.Random(randomSeed);
            nextRewardDropId = firstRewardDropId;
            hasInitializedRandom = true;
        }

        private void Reset()
        {
            spawnParent = transform;
            firstRewardDropId = 0;
            enemyDefeatRewardValue = 1;
            miningRewardValue = 1;
            miningDropChancePercent = 35;
            randomSeed = 2401;
            preventDuplicateActiveDropOnSameCell = true;
        }

        private void OnValidate()
        {
            firstRewardDropId = Mathf.Max(0, firstRewardDropId);
            enemyDefeatRewardValue = Mathf.Max(1, enemyDefeatRewardValue);
            miningRewardValue = Mathf.Max(1, miningRewardValue);
            miningDropChancePercent = Mathf.Clamp(miningDropChancePercent, 0, 100);
        }
    }
}