using System.Collections.Generic;
using DeepSeal.Combat;
using DeepSeal.Core;
using DeepSeal.UnityAdapters.Enemies;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Player
{
    /// <summary>
    /// Prototype-only adapter that applies enemy contact damage to the player.
    /// This intentionally uses grid distance instead of physics colliders for the early prototype.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypePlayerContactDamage : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PrototypePlayerMovement playerMovement;
        [SerializeField] private PrototypePlayerHealth playerHealth;
        [SerializeField] private PrototypeEnemySpawner enemySpawner;

        [Header("Contact Damage")]
        [SerializeField] private bool damageOnUpdate = true;
        [SerializeField] private int contactRangeCells;
        [SerializeField] private int contactDamage = 1;
        [SerializeField] private float damageIntervalSeconds = 0.75f;

        [Header("Debug")]
        [SerializeField] private bool logDamageResults;

        private readonly List<EnemyState> candidateEnemies = new List<EnemyState>(16);
        private float nextAllowedDamageTime;
        private bool warnedMissingPlayerMovement;
        private bool warnedMissingPlayerHealth;
        private bool warnedMissingEnemySpawner;

        private void Update()
        {
            if (!damageOnUpdate)
            {
                return;
            }

            if (Time.time < nextAllowedDamageTime)
            {
                return;
            }

            TryApplyContactDamage();
        }

        [ContextMenu("Apply Contact Damage")]
        public bool TryApplyContactDamage()
        {
            if (!TryResolveReferences())
            {
                return false;
            }

            if (playerHealth.IsDefeated)
            {
                return false;
            }

            if (!playerMovement.TryGetCurrentGridPosition(out GridPosition playerPosition))
            {
                Debug.LogWarning("Cannot apply contact damage because player grid position is unavailable.", this);
                return false;
            }

            enemySpawner.RemoveInactiveEnemyReferences();
            candidateEnemies.Clear();

            IReadOnlyList<PrototypeEnemyView> spawnedEnemies = enemySpawner.SpawnedEnemies;

            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                PrototypeEnemyView enemyView = spawnedEnemies[i];

                if (enemyView == null || enemyView.IsDefeated || !enemyView.isActiveAndEnabled)
                {
                    continue;
                }

                if (enemyView.TryGetCurrentEnemy(out EnemyState enemy))
                {
                    candidateEnemies.Add(enemy);
                }
            }

            if (!AttackTargetingRules.TryFindNearestTarget(
                    playerPosition,
                    candidateEnemies,
                    contactRangeCells,
                    out EnemyState contactEnemy))
            {
                return false;
            }

            bool changed = playerHealth.TryApplyDamage(contactDamage);
            nextAllowedDamageTime = Time.time + damageIntervalSeconds;

            if (logDamageResults)
            {
                Debug.Log(
                    $"Enemy contact damage: Enemy={contactEnemy.Id}, Damage={contactDamage}, PlayerHP={playerHealth.CurrentHitPoints}/{playerHealth.MaxHitPoints}.",
                    this);
            }

            return changed;
        }

        private bool TryResolveReferences()
        {
            if (playerMovement == null)
            {
                if (!warnedMissingPlayerMovement)
                {
                    Debug.LogError("Prototype contact damage requires a Prototype Player Movement reference.", this);
                    warnedMissingPlayerMovement = true;
                }

                return false;
            }

            warnedMissingPlayerMovement = false;

            if (playerHealth == null)
            {
                if (!warnedMissingPlayerHealth)
                {
                    Debug.LogError("Prototype contact damage requires a Prototype Player Health reference.", this);
                    warnedMissingPlayerHealth = true;
                }

                return false;
            }

            warnedMissingPlayerHealth = false;

            if (enemySpawner == null)
            {
                if (!warnedMissingEnemySpawner)
                {
                    Debug.LogError("Prototype contact damage requires a Prototype Enemy Spawner reference.", this);
                    warnedMissingEnemySpawner = true;
                }

                return false;
            }

            warnedMissingEnemySpawner = false;
            return true;
        }

        private void Reset()
        {
            playerMovement = GetComponent<PrototypePlayerMovement>();
            playerHealth = GetComponent<PrototypePlayerHealth>();
            damageOnUpdate = true;
            contactRangeCells = 0;
            contactDamage = 1;
            damageIntervalSeconds = 0.75f;
        }

        private void OnValidate()
        {
            contactRangeCells = Mathf.Max(0, contactRangeCells);
            contactDamage = Mathf.Max(1, contactDamage);
            damageIntervalSeconds = Mathf.Max(0.05f, damageIntervalSeconds);
        }
    }
}