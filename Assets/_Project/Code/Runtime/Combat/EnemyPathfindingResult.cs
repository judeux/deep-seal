using DeepSeal.Core;

namespace DeepSeal.Combat
{
    /// <summary>
    /// Result of a prototype grid pathfinding query for enemy movement.
    /// </summary>
    public readonly struct EnemyPathfindingResult
    {
        private EnemyPathfindingResult(
            bool found,
            bool alreadyAtTarget,
            GridPosition startPosition,
            GridPosition targetPosition,
            GridPosition nextPosition,
            GridDirection direction,
            int visitedCellCount)
        {
            Found = found;
            AlreadyAtTarget = alreadyAtTarget;
            StartPosition = startPosition;
            TargetPosition = targetPosition;
            NextPosition = nextPosition;
            Direction = direction;
            VisitedCellCount = visitedCellCount;
        }

        public bool Found { get; }

        public bool AlreadyAtTarget { get; }

        public GridPosition StartPosition { get; }

        public GridPosition TargetPosition { get; }

        public GridPosition NextPosition { get; }

        public GridDirection Direction { get; }

        public int VisitedCellCount { get; }

        public bool CanMove => Found && !AlreadyAtTarget && Direction.IsCardinal();

        public static EnemyPathfindingResult FoundPath(
            GridPosition startPosition,
            GridPosition targetPosition,
            GridPosition nextPosition,
            GridDirection direction,
            int visitedCellCount)
        {
            return new EnemyPathfindingResult(
                true,
                false,
                startPosition,
                targetPosition,
                nextPosition,
                direction,
                visitedCellCount);
        }

        public static EnemyPathfindingResult AtTarget(
            GridPosition startPosition,
            GridPosition targetPosition,
            int visitedCellCount)
        {
            return new EnemyPathfindingResult(
                true,
                true,
                startPosition,
                targetPosition,
                startPosition,
                GridDirection.None,
                visitedCellCount);
        }

        public static EnemyPathfindingResult NoPath(
            GridPosition startPosition,
            GridPosition targetPosition,
            int visitedCellCount)
        {
            return new EnemyPathfindingResult(
                false,
                false,
                startPosition,
                targetPosition,
                startPosition,
                GridDirection.None,
                visitedCellCount);
        }
    }
}