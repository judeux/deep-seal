namespace DeepSeal.Combat
{
    /// <summary>
    /// 돌진이 멈춘 이유.
    /// </summary>
    public enum EnemyChargeStopReason
    {
        RangeEnd = 0,
        WallHit = 1,
        TargetReached = 2
    }
}
