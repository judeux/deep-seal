namespace DeepSeal.Combat
{
    /// <summary>
    /// Describes the result of a single enemy movement decision.
    /// </summary>
    public enum EnemyMoveResultType
    {
        None = 0,
        Moved = 1,
        AlreadyAtTarget = 2,
        Blocked = 3,
        EnemyOutOfBounds = 4,
        DestinationOutOfBounds = 5,
        InvalidDirection = 6
    }
}