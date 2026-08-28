using DeepSeal.Core;

namespace DeepSeal.Combat
{
    /// <summary>
    /// Pure result of tracing one projectile flight over the mine grid.
    /// </summary>
    public readonly struct ProjectileTraceResult
    {
        public ProjectileTraceResult(
            bool hasImpact,
            GridPosition impactPosition,
            int traveledCells,
            bool blockedByWall,
            int hitEnemyId)
        {
            HasImpact = hasImpact;
            ImpactPosition = impactPosition;
            TraveledCells = traveledCells;
            BlockedByWall = blockedByWall;
            HitEnemyId = hitEnemyId;
        }

        public bool HasImpact { get; }

        public GridPosition ImpactPosition { get; }

        public int TraveledCells { get; }

        public bool BlockedByWall { get; }

        public int HitEnemyId { get; }

        public static ProjectileTraceResult NoShot()
        {
            return new ProjectileTraceResult(false, GridPosition.Zero, 0, false, -1);
        }
    }
}