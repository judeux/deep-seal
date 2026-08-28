using System;
using System.Collections.Generic;
using DeepSeal.Core;

namespace DeepSeal.Combat
{
    /// <summary>
    /// Pure C# rules for the area attack pattern.
    /// Every enemy within Manhattan range is affected, regardless of walls.
    /// </summary>
    public static class AreaAttackRules
    {
        public static int CollectAffectedEnemies(
            GridPosition attackerPosition,
            IReadOnlyList<EnemyState> enemies,
            int maxRangeCells,
            List<EnemyState> affectedEnemies)
        {
            if (enemies == null)
            {
                throw new ArgumentNullException(nameof(enemies));
            }

            if (affectedEnemies == null)
            {
                throw new ArgumentNullException(nameof(affectedEnemies));
            }

            if (maxRangeCells < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxRangeCells),
                    maxRangeCells,
                    "Attack range must be zero or greater.");
            }

            affectedEnemies.Clear();

            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyState candidate = enemies[i];

                if (AttackTargetingRules.ManhattanDistance(attackerPosition, candidate.Position) <= maxRangeCells)
                {
                    affectedEnemies.Add(candidate);
                }
            }

            return affectedEnemies.Count;
        }
    }
}