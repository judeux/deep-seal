using System;
using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Expedition;
using DeepSeal.UnityAdapters.Treasures;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Player
{
    /// <summary>
    /// Prototype-only adapter that lets the player pick up treasure by entering the same grid cell.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypePlayerTreasurePickup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PrototypePlayerMovement playerMovement;
        [SerializeField] private PrototypePlayerHealth playerHealth;
        [SerializeField] private PrototypeTreasureSpawner treasureSpawner;

        [Header("Pickup")]
        [SerializeField] private bool pickupOnUpdate = true;
        [SerializeField] private float pickupIntervalSeconds = 0.1f;

        [Header("Runtime Debug")]
        [SerializeField] private int collectedTreasureCount;
        [SerializeField] private int collectedTreasureValue;

        [Header("Debug")]
        [SerializeField] private bool logPickupResults;

        private readonly List<TreasureState> candidateTreasures = new List<TreasureState>(16);
        private float nextAllowedPickupTime;
        private bool warnedMissingPlayerMovement;
        private bool warnedMissingTreasureSpawner;

        public int CollectedTreasureCount => collectedTreasureCount;

        public int CollectedTreasureValue => collectedTreasureValue;

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

            TryPickUpTreasureAtPlayerPosition();
        }

        [ContextMenu("Pick Up Treasure At Player Position")]
        public bool TryPickUpTreasureAtPlayerPosition()
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
                Debug.LogWarning("Cannot pick up treasure because player grid position is unavailable.", this);
                return false;
            }

            treasureSpawner.RemoveInactiveTreasureReferences();
            candidateTreasures.Clear();

            IReadOnlyList<PrototypeTreasureView> spawnedTreasures = treasureSpawner.SpawnedTreasures;

            for (int i = 0; i < spawnedTreasures.Count; i++)
            {
                PrototypeTreasureView treasureView = spawnedTreasures[i];

                if (treasureView == null || treasureView.IsCollected || !treasureView.isActiveAndEnabled)
                {
                    continue;
                }

                if (treasureView.TryGetCurrentTreasure(out TreasureState treasure))
                {
                    candidateTreasures.Add(treasure);
                }
            }

            for (int i = 0; i < candidateTreasures.Count; i++)
            {
                TreasureState treasure = candidateTreasures[i];
                TreasurePickupResult result;

                try
                {
                    result = TreasurePickupRules.TryPickUp(treasure, playerPosition);
                }
                catch (ArgumentException exception)
                {
                    Debug.LogError(
                        $"Treasure pickup failed because treasure state is invalid. {exception.Message}",
                        this);
                    continue;
                }

                if (!result.Succeeded)
                {
                    continue;
                }

                PrototypeTreasureView treasureView = FindTreasureViewById(result.CurrentTreasure.Id);

                if (treasureView == null)
                {
                    Debug.LogWarning(
                        $"Treasure pickup selected treasure id {result.CurrentTreasure.Id}, but no matching treasure view was found.",
                        this);
                    return false;
                }

                bool applied = treasureView.ApplyPickupResult(result);

                if (!applied)
                {
                    return false;
                }

                collectedTreasureCount++;
                collectedTreasureValue += result.CollectedValue;
                nextAllowedPickupTime = Time.time + pickupIntervalSeconds;
                treasureSpawner.RemoveInactiveTreasureReferences();

                if (logPickupResults)
                {
                    Debug.Log(
                        $"Picked up treasure {result.CurrentTreasure.Id}. Count={collectedTreasureCount}, Value={collectedTreasureValue}.",
                        this);
                }

                return true;
            }

            return false;
        }

        private PrototypeTreasureView FindTreasureViewById(int treasureId)
        {
            IReadOnlyList<PrototypeTreasureView> spawnedTreasures = treasureSpawner.SpawnedTreasures;

            for (int i = 0; i < spawnedTreasures.Count; i++)
            {
                PrototypeTreasureView treasureView = spawnedTreasures[i];

                if (treasureView == null || treasureView.IsCollected || !treasureView.isActiveAndEnabled)
                {
                    continue;
                }

                if (treasureView.TryGetCurrentTreasure(out TreasureState treasure) && treasure.Id == treasureId)
                {
                    return treasureView;
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
                    Debug.LogError("Prototype treasure pickup requires a Prototype Player Movement reference.", this);
                    warnedMissingPlayerMovement = true;
                }

                return false;
            }

            warnedMissingPlayerMovement = false;

            if (treasureSpawner == null)
            {
                if (!warnedMissingTreasureSpawner)
                {
                    Debug.LogError("Prototype treasure pickup requires a Prototype Treasure Spawner reference.", this);
                    warnedMissingTreasureSpawner = true;
                }

                return false;
            }

            warnedMissingTreasureSpawner = false;
            return true;
        }

        private void Reset()
        {
            playerMovement = GetComponent<PrototypePlayerMovement>();
            playerHealth = GetComponent<PrototypePlayerHealth>();
            pickupOnUpdate = true;
            pickupIntervalSeconds = 0.1f;
        }

        private void OnValidate()
        {
            pickupIntervalSeconds = Mathf.Max(0.01f, pickupIntervalSeconds);
        }
    }
}