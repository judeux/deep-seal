using System.Collections.Generic;
using DeepSeal.Combat;
using DeepSeal.Core;
using DeepSeal.UnityAdapters.Enemies;
using DeepSeal.UnityAdapters.RewardDrops;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Player
{
    /// <summary>
    /// Prototype-only automatic attack adapter.
    /// Periodically targets the nearest spawned enemy and applies temporary prototype damage.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypePlayerAutoAttack : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PrototypePlayerMovement playerMovement;
        [SerializeField] private PrototypeEnemySpawner enemySpawner;
        [SerializeField] private PrototypeRewardDropSpawner rewardDropSpawner;

        [Header("Attack")]
        [SerializeField] private bool attackOnUpdate = true;
        [SerializeField] private float attackIntervalSeconds = 0.5f;
        [SerializeField] private int attackRangeCells = 4;
        [SerializeField] private int attackDamage = 1;
        [SerializeField] private bool spawnRewardDropsOnEnemyDefeat = true;

        [Header("Debug")]
        [SerializeField] private bool logAttackResults;

        private readonly List<EnemyState> candidateEnemies = new List<EnemyState>(16);
        private float nextAttackTime;
        private bool warnedMissingPlayerMovement;
        private bool warnedMissingEnemySpawner;
        private bool warnedMissingRewardDropSpawner;

        public int AttackDamage => attackDamage;
        public int AttackRangeCells => attackRangeCells;

        private void Start()
        {
            ScheduleNextAttack();
        }

        private void Update()
        {
            if (!attackOnUpdate)
            {
                return;
            }

            if (Time.time < nextAttackTime)
            {
                return;
            }

            ScheduleNextAttack();
            TryAttackNearestEnemy();
        }

        [ContextMenu("Attack Nearest Enemy")]
        public bool TryAttackNearestEnemy()
        {
            if (!TryResolveReferences())
            {
                return false;
            }

            if (!playerMovement.TryGetCurrentGridPosition(out GridPosition attackerPosition))
            {
                Debug.LogWarning("Cannot auto attack because player grid position is unavailable.", this);
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
                    attackerPosition,
                    candidateEnemies,
                    attackRangeCells,
                    out EnemyState targetEnemy))
            {
                if (logAttackResults)
                {
                    Debug.Log("Auto attack found no enemy in range.", this);
                }

                return false;
            }

            PrototypeEnemyView targetView = FindEnemyViewById(targetEnemy.Id);

            if (targetView == null)
            {
                Debug.LogWarning(
                    $"Auto attack selected enemy id {targetEnemy.Id}, but no matching enemy view was found.",
                    this);
                return false;
            }

            bool defeated = targetView.TryApplyPrototypeDamage(attackDamage);

            if (defeated && spawnRewardDropsOnEnemyDefeat)
            {
                TrySpawnEnemyDefeatRewardDrop(targetEnemy.Position);
            }

            if (logAttackResults)
            {
                Debug.Log(
                    $"Auto attack hit enemy {targetEnemy.Id} for {attackDamage}. Defeated={defeated}.",
                    this);
            }

            return true;
        }

        private PrototypeEnemyView FindEnemyViewById(int enemyId)
        {
            IReadOnlyList<PrototypeEnemyView> spawnedEnemies = enemySpawner.SpawnedEnemies;

            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                PrototypeEnemyView enemyView = spawnedEnemies[i];

                if (enemyView == null || enemyView.IsDefeated || !enemyView.isActiveAndEnabled)
                {
                    continue;
                }

                if (enemyView.TryGetCurrentEnemy(out EnemyState enemy) && enemy.Id == enemyId)
                {
                    return enemyView;
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
                    Debug.LogError("Prototype auto attack requires a Prototype Player Movement reference.", this);
                    warnedMissingPlayerMovement = true;
                }

                return false;
            }

            warnedMissingPlayerMovement = false;

            if (enemySpawner == null)
            {
                if (!warnedMissingEnemySpawner)
                {
                    Debug.LogError("Prototype auto attack requires a Prototype Enemy Spawner reference.", this);
                    warnedMissingEnemySpawner = true;
                }

                return false;
            }

            warnedMissingEnemySpawner = false;
            return true;
        }

        private void ScheduleNextAttack()
        {
            nextAttackTime = Time.time + attackIntervalSeconds;
        }

        private void Reset()
        {
            playerMovement = GetComponent<PrototypePlayerMovement>();
            attackOnUpdate = true;
            attackIntervalSeconds = 0.5f;
            attackRangeCells = 4;
            attackDamage = 1;
            spawnRewardDropsOnEnemyDefeat = true;
        }

        public void AddAttackDamage(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"Ignored attack damage upgrade because amount must be positive. Amount={amount}.", this);
                return;
            }

            attackDamage += amount;
        }

        public void AddAttackRangeCells(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"Ignored attack range upgrade because amount must be positive. Amount={amount}.", this);
                return;
            }

            attackRangeCells += amount;
        }

        private void OnValidate()
        {
            attackIntervalSeconds = Mathf.Max(0.05f, attackIntervalSeconds);
            attackRangeCells = Mathf.Max(0, attackRangeCells);
            attackDamage = Mathf.Max(1, attackDamage);
        }

        private void TrySpawnEnemyDefeatRewardDrop(GridPosition position)
        {
            if (rewardDropSpawner == null)
            {
                if (!warnedMissingRewardDropSpawner)
                {
                    Debug.LogWarning("Enemy defeat reward drops are enabled, but Prototype Reward Drop Spawner is not assigned.", this);
                    warnedMissingRewardDropSpawner = true;
                }

                return;
            }

            warnedMissingRewardDropSpawner = false;
            rewardDropSpawner.TrySpawnEnemyDefeatRewardDrop(position);
        }
    }
}