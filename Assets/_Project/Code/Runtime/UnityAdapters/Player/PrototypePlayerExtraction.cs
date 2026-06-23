using DeepSeal.Core;
using DeepSeal.Expedition;
using DeepSeal.UnityAdapters.Extraction;
using System;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Player
{
    /// <summary>
    /// Prototype-only adapter that completes extraction when the player stands on the extraction marker with enough treasure.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypePlayerExtraction : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PrototypePlayerMovement playerMovement;
        [SerializeField] private PrototypePlayerHealth playerHealth;
        [SerializeField] private PrototypePlayerTreasurePickup treasurePickup;
        [SerializeField] private PrototypeExtractionMarkerSpawner extractionMarkerSpawner;

        [Header("Extraction")]
        [SerializeField] private bool extractionOnUpdate = true;
        [SerializeField] private float extractionCheckIntervalSeconds = 0.1f;
        [SerializeField] private int requiredTreasureValue = 1;

        [Header("Completion")]
        [SerializeField] private bool disableBehavioursOnExtraction = true;
        [SerializeField] private Behaviour[] disableOnExtraction;

        [Header("Runtime Debug")]
        [SerializeField] private bool hasExtracted;
        [SerializeField] private int extractedTreasureValue;

        [Header("Debug")]
        [SerializeField] private bool logExtractionResults;

        private float nextAllowedExtractionTime;
        private bool warnedMissingPlayerMovement;
        private bool warnedMissingTreasurePickup;
        private bool warnedMissingExtractionMarkerSpawner;

        public bool HasExtracted => hasExtracted;

        public int ExtractedTreasureValue => extractedTreasureValue;

        public int RequiredTreasureValue => requiredTreasureValue;

        private void Update()
        {
            if (!extractionOnUpdate)
            {
                return;
            }

            if (Time.time < nextAllowedExtractionTime)
            {
                return;
            }

            nextAllowedExtractionTime = Time.time + extractionCheckIntervalSeconds;
            TryExtractAtPlayerPosition();
        }

        [ContextMenu("Try Extract At Player Position")]
        public bool TryExtractAtPlayerPosition()
        {
            if (hasExtracted)
            {
                return false;
            }

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
                Debug.LogWarning("Cannot complete extraction because player grid position is unavailable.", this);
                return false;
            }

            if (!TryGetMarkerView(out PrototypeExtractionMarkerView markerView))
            {
                return false;
            }

            if (!markerView.TryGetCurrentMarker(out ExtractionMarkerState marker))
            {
                Debug.LogWarning("Cannot complete extraction because extraction marker state is unavailable.", this);
                return false;
            }

            int carriedTreasureValue = treasurePickup != null
                ? treasurePickup.CollectedTreasureValue
                : 0;

            ExtractionResult result;

            try
            {
                result = ExtractionRules.TryExtract(
                    marker,
                    playerPosition,
                    carriedTreasureValue,
                    requiredTreasureValue);
            }
            catch (ArgumentException exception)
            {
                Debug.LogError(
                    $"Prototype extraction failed because extraction state is invalid. {exception.Message}",
                    this);
                return false;
            }

            if (!result.Succeeded)
            {
                LogExtractionFailure(result);
                return false;
            }

            bool applied = markerView.ApplyExtractionResult(result);

            if (!applied)
            {
                return false;
            }

            hasExtracted = true;
            extractedTreasureValue = result.CarriedTreasureValue;

            if (disableBehavioursOnExtraction)
            {
                SetExtractionBehavioursEnabled(false);
            }

            if (logExtractionResults)
            {
                Debug.Log(
                    $"Prototype extraction completed. TreasureValue={extractedTreasureValue}, Required={requiredTreasureValue}.",
                    this);
            }

            return true;
        }

        private bool TryGetMarkerView(out PrototypeExtractionMarkerView markerView)
        {
            markerView = null;

            if (extractionMarkerSpawner.TryGetCurrentMarkerView(out markerView))
            {
                return true;
            }

            if (extractionMarkerSpawner.TrySpawnMarker()
                && extractionMarkerSpawner.TryGetCurrentMarkerView(out markerView))
            {
                return true;
            }

            Debug.LogWarning("Cannot complete extraction because no active extraction marker is available.", this);
            return false;
        }

        private void LogExtractionFailure(ExtractionResult result)
        {
            if (!logExtractionResults)
            {
                return;
            }

            if (result.NotAtMarker)
            {
                return;
            }

            if (result.MissingRequiredTreasure)
            {
                Debug.Log(
                    $"Extraction unavailable. TreasureValue={result.CarriedTreasureValue}, Required={result.RequiredTreasureValue}.",
                    this);
                return;
            }

            if (result.AlreadyCompleted)
            {
                Debug.Log("Extraction marker is already completed.", this);
            }
        }

        private bool TryResolveReferences()
        {
            if (playerMovement == null)
            {
                if (!warnedMissingPlayerMovement)
                {
                    Debug.LogError("Prototype player extraction requires a Prototype Player Movement reference.", this);
                    warnedMissingPlayerMovement = true;
                }

                return false;
            }

            warnedMissingPlayerMovement = false;

            if (requiredTreasureValue > 0 && treasurePickup == null)
            {
                if (!warnedMissingTreasurePickup)
                {
                    Debug.LogError("Prototype player extraction requires a Prototype Player Treasure Pickup reference when Required Treasure Value is greater than zero.", this);
                    warnedMissingTreasurePickup = true;
                }

                return false;
            }

            warnedMissingTreasurePickup = false;

            if (extractionMarkerSpawner == null)
            {
                if (!warnedMissingExtractionMarkerSpawner)
                {
                    Debug.LogError("Prototype player extraction requires a Prototype Extraction Marker Spawner reference.", this);
                    warnedMissingExtractionMarkerSpawner = true;
                }

                return false;
            }

            warnedMissingExtractionMarkerSpawner = false;
            return true;
        }

        private void SetExtractionBehavioursEnabled(bool enabledValue)
        {
            if (disableOnExtraction == null)
            {
                return;
            }

            for (int i = 0; i < disableOnExtraction.Length; i++)
            {
                Behaviour behaviour = disableOnExtraction[i];

                if (behaviour == null || behaviour == this)
                {
                    continue;
                }

                behaviour.enabled = enabledValue;
            }
        }

        private void Reset()
        {
            playerMovement = GetComponent<PrototypePlayerMovement>();
            playerHealth = GetComponent<PrototypePlayerHealth>();
            treasurePickup = GetComponent<PrototypePlayerTreasurePickup>();
            extractionOnUpdate = true;
            extractionCheckIntervalSeconds = 0.1f;
            requiredTreasureValue = 1;
            disableBehavioursOnExtraction = true;
        }

        private void OnValidate()
        {
            extractionCheckIntervalSeconds = Mathf.Max(0.01f, extractionCheckIntervalSeconds);
            requiredTreasureValue = Mathf.Max(0, requiredTreasureValue);
        }
    }
}