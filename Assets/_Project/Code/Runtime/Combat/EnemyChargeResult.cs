using System;
using DeepSeal.Core;

namespace DeepSeal.Combat
{
    /// <summary>
    /// 돌진 추적 결과. 경로에는 실제로 통과한 셀만 포함된다.
    /// </summary>
    public readonly struct EnemyChargeResult
    {
        public EnemyChargeResult(
            EnemyChargeStopReason stopReason,
            GridPosition finalPosition,
            GridPosition[] pathCells,
            bool stunned)
        {
            StopReason = stopReason;
            FinalPosition = finalPosition;
            PathCells = pathCells ?? Array.Empty<GridPosition>();
            Stunned = stunned;
        }

        public EnemyChargeStopReason StopReason { get; }

        public GridPosition FinalPosition { get; }

        public GridPosition[] PathCells { get; }

        public int PathCellCount => PathCells.Length;

        public bool Stunned { get; }
    }
}
