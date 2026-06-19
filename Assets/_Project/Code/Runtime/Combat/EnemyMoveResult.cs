using DeepSeal.Core;

namespace DeepSeal.Combat
{
    /// <summary>
    /// Immutable result returned by enemy movement rules.
    /// </summary>
    public readonly struct EnemyMoveResult
    {
        private EnemyMoveResult(
            EnemyMoveResultType type,
            EnemyState previousEnemy,
            EnemyState currentEnemy,
            GridPosition attemptedPosition,
            GridDirection direction)
        {
            Type = type;
            PreviousEnemy = previousEnemy;
            CurrentEnemy = currentEnemy;
            AttemptedPosition = attemptedPosition;
            Direction = direction;
        }

        public EnemyMoveResultType Type { get; }

        public EnemyState PreviousEnemy { get; }

        public EnemyState CurrentEnemy { get; }

        public GridPosition AttemptedPosition { get; }

        public GridDirection Direction { get; }

        public bool Moved => Type == EnemyMoveResultType.Moved;

        public static EnemyMoveResult MovedTo(
            EnemyState previousEnemy,
            EnemyState currentEnemy,
            GridPosition attemptedPosition,
            GridDirection direction)
        {
            return new EnemyMoveResult(
                EnemyMoveResultType.Moved,
                previousEnemy,
                currentEnemy,
                attemptedPosition,
                direction);
        }

        public static EnemyMoveResult AlreadyAtTarget(EnemyState enemy)
        {
            return new EnemyMoveResult(
                EnemyMoveResultType.AlreadyAtTarget,
                enemy,
                enemy,
                enemy.Position,
                GridDirection.None);
        }

        public static EnemyMoveResult Blocked(
            EnemyState enemy,
            GridPosition attemptedPosition,
            GridDirection direction)
        {
            return new EnemyMoveResult(
                EnemyMoveResultType.Blocked,
                enemy,
                enemy,
                attemptedPosition,
                direction);
        }

        public static EnemyMoveResult EnemyOutOfBounds(EnemyState enemy)
        {
            return new EnemyMoveResult(
                EnemyMoveResultType.EnemyOutOfBounds,
                enemy,
                enemy,
                enemy.Position,
                GridDirection.None);
        }

        public static EnemyMoveResult DestinationOutOfBounds(
            EnemyState enemy,
            GridPosition attemptedPosition,
            GridDirection direction)
        {
            return new EnemyMoveResult(
                EnemyMoveResultType.DestinationOutOfBounds,
                enemy,
                enemy,
                attemptedPosition,
                direction);
        }

        public static EnemyMoveResult InvalidDirection(
            EnemyState enemy,
            GridPosition attemptedPosition,
            GridDirection direction)
        {
            return new EnemyMoveResult(
                EnemyMoveResultType.InvalidDirection,
                enemy,
                enemy,
                attemptedPosition,
                direction);
        }
    }
}