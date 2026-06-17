using DeepSeal.Core;

namespace DeepSeal.Mining
{
    public enum MiningResultType
    {
        OutOfBounds = 0,
        NotMineable = 1,
        Damaged = 2,
        Destroyed = 3
    }

    /// <summary>
    /// Describes the outcome of one mining attempt against a mine grid cell.
    /// 채굴 시도의 결과를 표현한다. 범위 밖, 채굴 불가, 내구도 감소, 파괴 완료를 구분한다.
    /// </summary>
    public readonly struct MiningResult
    {
        private MiningResult(
            MiningResultType type,
            GridPosition position,
            bool hasCell,
            TerrainCell previousCell,
            TerrainCell currentCell,
            int damage)
        {
            Type = type;
            Position = position;
            HasCell = hasCell;
            PreviousCell = previousCell;
            CurrentCell = currentCell;
            Damage = damage;
        }

        public MiningResultType Type { get; }

        public GridPosition Position { get; }

        public bool HasCell { get; }

        public TerrainCell PreviousCell { get; }

        public TerrainCell CurrentCell { get; }

        public int Damage { get; }

        public bool Succeeded => Type == MiningResultType.Damaged || Type == MiningResultType.Destroyed;

        public bool ChangedCell => Succeeded;

        public bool WasDestroyed => Type == MiningResultType.Destroyed;

        internal static MiningResult OutOfBounds(GridPosition position, int damage)
        {
            return new MiningResult(
                MiningResultType.OutOfBounds,
                position,
                false,
                default,
                default,
                damage);
        }

        internal static MiningResult NotMineable(GridPosition position, TerrainCell cell, int damage)
        {
            return new MiningResult(
                MiningResultType.NotMineable,
                position,
                true,
                cell,
                cell,
                damage);
        }

        internal static MiningResult Damaged(
            GridPosition position,
            TerrainCell previousCell,
            TerrainCell currentCell,
            int damage)
        {
            return new MiningResult(
                MiningResultType.Damaged,
                position,
                true,
                previousCell,
                currentCell,
                damage);
        }

        internal static MiningResult Destroyed(
            GridPosition position,
            TerrainCell previousCell,
            TerrainCell currentCell,
            int damage)
        {
            return new MiningResult(
                MiningResultType.Destroyed,
                position,
                true,
                previousCell,
                currentCell,
                damage);
        }
    }
}
