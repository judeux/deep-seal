using System;
using System.Collections.Generic;
using DeepSeal.Core;

namespace DeepSeal.Combat
{
    /// <summary>
    /// Pure C# target selection rules for prototype automatic attacks.
    /// </summary>
    public static class AttackTargetingRules
    {
        public static bool TryFindNearestTarget(
            GridPosition attackerPosition,
            IReadOnlyList<EnemyState> enemies,
            int maxRangeCells,
            out EnemyState target)
        {
            if (enemies == null)
            {
                throw new ArgumentNullException(nameof(enemies));
            }

            if (maxRangeCells < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxRangeCells),
                    maxRangeCells,
                    "Attack range must be zero or greater.");
            }

            target = default;
            bool found = false;
            int bestDistance = int.MaxValue;
            int bestId = int.MaxValue;

            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyState candidate = enemies[i];
                int distance = ManhattanDistance(attackerPosition, candidate.Position);

                if (distance > maxRangeCells)
                {
                    continue;
                }

                if (distance < bestDistance || distance == bestDistance && candidate.Id < bestId)
                {
                    target = candidate;
                    bestDistance = distance;
                    bestId = candidate.Id;
                    found = true;
                }
            }

            return found;
        }

        public static int ManhattanDistance(GridPosition from, GridPosition to)
        {
            return Math.Abs(to.X - from.X) + Math.Abs(to.Y - from.Y);
        }
    }
}