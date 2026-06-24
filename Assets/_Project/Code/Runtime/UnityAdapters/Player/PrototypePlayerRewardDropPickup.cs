using System;
using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Expedition;
using DeepSeal.UnityAdapters.RewardDrops;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Player
{
    /// <summary>
    /// Prototype-only adapter that lets the player pick up nearby reward drops.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypePlayerRewardDropPickup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PrototypePlayerMovement playerMovement;
        [SerializeField] private PrototypePlayerHealth playerHealth;
        [SerializeField] private PrototypeRewardDropSpawner rewardDropSpawner;

        [Header("Pickup")]
        [SerializeField] private bool pickupOnUpdate = true;
        [SerializeField] private float pickupIntervalSeconds = 0.1f;
        [SerializeField] private int pickupRangeCells = 1;

        [Header("Runtime Debug")]
        [SerializeField] private int collectedRewardDropCount;
        [SerializeField] private int collectedRewardDropValue;

        [Header("Debug")]
        [SerializeField] private bool logPickupResults;

        private readonly List<RewardDropState> candidateDrops = new List<RewardDropState>(32);
        private float nextAllowedPickupTime;
        private bool warnedMissingPlayerMovement;
        private bool warnedMissingRewardDropSpawner;

        public int CollectedRewardDropCount => collectedRewardDropCount;

        public int CollectedRewardDropValue => collectedRewardDropValue;

        private void Update()
        {
            if (!pickupOnUpdate)
            {
                return;
            }

            if (Time.time < nextAllowedPickupTime)
            {
                return;
            }

            TryPickUpNearbyRewardDrops();
        }

        [ContextMenu("Pick Up Nearby Reward Drops")]
        public bool TryPickUpNearbyRewardDrops()
        {
            if (!TryResolveReferences())
            {
                return false;
            }

            if (playerHealth != null && playerHealth.IsDefeated)
            {
                return false;
            }

            if (!playerMovement.TryGetCurrentGridPosition(out GridPosition playerPosition))
            {
                Debug.LogWarning("Cannot pick up reward drops because player grid position is unavailable.", this);
                return false;
            }

            rewardDropSpawner.RemoveInactiveRewardDropReferences();
            candidateDrops.Clear();

            IReadOnlyList<PrototypeRewardDropView> spawnedDrops = rewardDropSpawner.SpawnedDrops;

            for (int i = 0; i < spawnedDrops.Count; i++)
            {
                PrototypeRewardDropView dropView = spawnedDrops[i];

                if (dropView == null || dropView.IsCollected || !dropView.isActiveAndEnabled)
                {
                    continue;
                }

                if (dropView.TryGetCurrentDrop(out RewardDropState drop))
                {
                    candidateDrops.Add(drop);
                }
            }

            bool pickedUpAny = false;

            for (int i = 0; i < candidateDrops.Count; i++)
            {
                RewardDropState drop = candidateDrops[i];
                RewardDropPickupResult result;

                try
                {
                    result = RewardDropPickupRules.TryPickUp(
                        drop,
                        playerPosition,
                        pickupRangeCells);
                }
                catch (ArgumentException exception)
                {
                    Debug.LogError(
                        $"Reward drop pickup failed because drop state is invalid. {exception.Message}",
                        this);
                    continue;
                }

                if (!result.Succeeded)
                {
                    continue;
                }

                PrototypeRewardDropView dropView = FindRewardDropViewById(result.CurrentDrop.Id);

                if (dropView == null)
                {
                    Debug.LogWarning(
                        $"Reward drop pickup selected drop id {result.CurrentDrop.Id}, but no matching drop view was found.",
                        this);
                    continue;
                }

                bool applied = dropView.ApplyPickupResult(result);

                if (!applied)
                {
                    continue;
                }

                collectedRewardDropCount++;
                collectedRewardDropValue += result.CollectedValue;
                pickedUpAny = true;

                if (logPickupResults)
                {
                    Debug.Log(
                        $"Picked up reward drop {result.CurrentDrop.Id}. Count={collectedRewardDropCount}, Value={collectedRewardDropValue}.",
                        this);
                }
            }

            if (pickedUpAny)
            {
                nextAllowedPickupTime = Time.time + pickupIntervalSeconds;
                rewardDropSpawner.RemoveInactiveRewardDropReferences();
            }

            return pickedUpAny;
        }

        private PrototypeRewardDropView FindRewardDropViewById(int rewardDropId)
        {
            IReadOnlyList<PrototypeRewardDropView> spawnedDrops = rewardDropSpawner.SpawnedDrops;

            for (int i = 0; i < spawnedDrops.Count; i++)
            {
                PrototypeRewardDropView dropView = spawnedDrops[i];

                if (dropView == null || dropView.IsCollected || !dropView.isActiveAndEnabled)
                {
                    continue;
                }

                if (dropView.TryGetCurrentDrop(out RewardDropState drop) && drop.Id == rewardDropId)
                {
                    return dropView;
                }
            }

            return null;
        }

        private bool TryResolveReferences()
        {
            if (playerMovement == null)
            {
                if (!warnedMissingPlayerMovement)
                {
                    Debug.LogError("Prototype reward drop pickup requires a Prototype Player Movement reference.", this);
                    warnedMissingPlayerMovement = true;
                }

                return false;
            }

            warnedMissingPlayerMovement = false;

            if (rewardDropSpawner == null)
            {
                if (!warnedMissingRewardDropSpawner)
                {
                    Debug.LogError("Prototype reward drop pickup requires a Prototype Reward Drop Spawner reference.", this);
                    warnedMissingRewardDropSpawner = true;
                }

                return false;
            }

            warnedMissingRewardDropSpawner = false;
            return true;
        }

        private void Reset()
        {
            playerMovement = GetComponent<PrototypePlayerMovement>();
            playerHealth = GetComponent<PrototypePlayerHealth>();
            pickupOnUpdate = true;
            pickupIntervalSeconds = 0.1f;
            pickupRangeCells = 1;
        }

        private void OnValidate()
        {
            pickupIntervalSeconds = Mathf.Max(0.01f, pickupIntervalSeconds);
            pickupRangeCells = Mathf.Max(0, pickupRangeCells);
        }
    }
}