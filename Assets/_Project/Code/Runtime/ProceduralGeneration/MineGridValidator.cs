using System;
using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Mining;

namespace DeepSeal.ProceduralGeneration
{
    public enum MineGridValidationIssue
    {
        None = 0,
        GridIsNull = 1,
        InvalidSettings = 2,
        SizeMismatch = 3,
        BoundaryNotWall = 4,
        StartAreaBlocked = 5,
        DisconnectedPassableArea = 6,
        InsufficientPassableCells = 7,
        OuterFrameNotVoid = 8,
        PassableAreaTouchesVoid = 9,
        NonBoundaryTerrainTouchesVoid = 10
    }

    public readonly struct MineGridValidationResult
    {
        private MineGridValidationResult(
            bool isValid,
            MineGridValidationIssue issue,
            GridPosition position,
            bool hasPosition)
        {
            IsValid = isValid;
            Issue = issue;
            Position = position;
            HasPosition = hasPosition;
        }

        public bool IsValid { get; }

        public MineGridValidationIssue Issue { get; }

        public GridPosition Position { get; }

        public bool HasPosition { get; }

        internal static MineGridValidationResult Valid()
        {
            return new MineGridValidationResult(
                true,
                MineGridValidationIssue.None,
                GridPosition.Zero,
                false);
        }

        internal static MineGridValidationResult Invalid(MineGridValidationIssue issue)
        {
            return new MineGridValidationResult(
                false,
                issue,
                GridPosition.Zero,
                false);
        }

        internal static MineGridValidationResult Invalid(
            MineGridValidationIssue issue,
            GridPosition position)
        {
            return new MineGridValidationResult(
                false,
                issue,
                position,
                true);
        }
    }

    public static class MineGridValidator
    {
        private static readonly GridPosition[] CardinalOffsets =
        {
            new GridPosition(0, 1),
            new GridPosition(1, 0),
            new GridPosition(0, -1),
            new GridPosition(-1, 0)
        };

        public static MineGridValidationResult Validate(MineGenerationResult result)
        {
            return Validate(result.Grid, result.Settings);
        }

        public static MineGridValidationResult Validate(
            MineGrid grid,
            MineGenerationSettings settings)
        {
            if (grid == null)
            {
                return MineGridValidationResult.Invalid(MineGridValidationIssue.GridIsNull);
            }

            try
            {
                settings.Validate();
            }
            catch (ArgumentException)
            {
                return MineGridValidationResult.Invalid(MineGridValidationIssue.InvalidSettings);
            }

            if (grid.Width != settings.Width || grid.Height != settings.Height)
            {
                return MineGridValidationResult.Invalid(MineGridValidationIssue.SizeMismatch);
            }

            MineGridValidationResult shapeBoundaryResult = settings.ShapeMode == MineGenerationShapeMode.ConnectedCavern
                ? ValidateOuterFrameVoid(grid)
                : ValidateBoundaryWalls(grid);

            if (!shapeBoundaryResult.IsValid)
            {
                return shapeBoundaryResult;
            }

            MineGridValidationResult startAreaResult = ValidateStartArea(grid, settings);

            if (!startAreaResult.IsValid)
            {
                return startAreaResult;
            }

            if (settings.ShapeMode == MineGenerationShapeMode.ConnectedCavern)
            {
                MineGridValidationResult connectedResult = ValidateConnectedPassableArea(grid, settings);

                if (!connectedResult.IsValid)
                {
                    return connectedResult;
                }

                MineGridValidationResult passableBoundaryResult = ValidatePassableAreaDoesNotTouchVoid(grid);

                if (!passableBoundaryResult.IsValid)
                {
                    return passableBoundaryResult;
                }

                return ValidateOnlyBoundaryWallsTouchVoid(grid);
            }

            return MineGridValidationResult.Valid();
        }

        private static MineGridValidationResult ValidateBoundaryWalls(MineGrid grid)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                var bottom = new GridPosition(x, 0);
                var top = new GridPosition(x, grid.Height - 1);

                if (!IsBoundaryWall(grid, bottom))
                {
                    return MineGridValidationResult.Invalid(
                        MineGridValidationIssue.BoundaryNotWall,
                        bottom);
                }

                if (!IsBoundaryWall(grid, top))
                {
                    return MineGridValidationResult.Invalid(
                        MineGridValidationIssue.BoundaryNotWall,
                        top);
                }
            }

            for (int y = 1; y < grid.Height - 1; y++)
            {
                var left = new GridPosition(0, y);
                var right = new GridPosition(grid.Width - 1, y);

                if (!IsBoundaryWall(grid, left))
                {
                    return MineGridValidationResult.Invalid(
                        MineGridValidationIssue.BoundaryNotWall,
                        left);
                }

                if (!IsBoundaryWall(grid, right))
                {
                    return MineGridValidationResult.Invalid(
                        MineGridValidationIssue.BoundaryNotWall,
                        right);
                }
            }

            return MineGridValidationResult.Valid();
        }

        private static MineGridValidationResult ValidateOuterFrameVoid(MineGrid grid)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                var bottom = new GridPosition(x, 0);
                var top = new GridPosition(x, grid.Height - 1);

                if (!IsVoid(grid, bottom))
                {
                    return MineGridValidationResult.Invalid(
                        MineGridValidationIssue.OuterFrameNotVoid,
                        bottom);
                }

                if (!IsVoid(grid, top))
                {
                    return MineGridValidationResult.Invalid(
                        MineGridValidationIssue.OuterFrameNotVoid,
                        top);
                }
            }

            for (int y = 1; y < grid.Height - 1; y++)
            {
                var left = new GridPosition(0, y);
                var right = new GridPosition(grid.Width - 1, y);

                if (!IsVoid(grid, left))
                {
                    return MineGridValidationResult.Invalid(
                        MineGridValidationIssue.OuterFrameNotVoid,
                        left);
                }

                if (!IsVoid(grid, right))
                {
                    return MineGridValidationResult.Invalid(
                        MineGridValidationIssue.OuterFrameNotVoid,
                        right);
                }
            }

            return MineGridValidationResult.Valid();
        }

        private static MineGridValidationResult ValidateStartArea(
            MineGrid grid,
            MineGenerationSettings settings)
        {
            for (int y = settings.StartPosition.Y - settings.StartClearRadius;
                 y <= settings.StartPosition.Y + settings.StartClearRadius;
                 y++)
            {
                for (int x = settings.StartPosition.X - settings.StartClearRadius;
                     x <= settings.StartPosition.X + settings.StartClearRadius;
                     x++)
                {
                    var position = new GridPosition(x, y);

                    if (!grid.TryGetCell(position, out TerrainCell cell) || !cell.IsPassable)
                    {
                        return MineGridValidationResult.Invalid(
                            MineGridValidationIssue.StartAreaBlocked,
                            position);
                    }
                }
            }

            return MineGridValidationResult.Valid();
        }

        private static MineGridValidationResult ValidateConnectedPassableArea(
            MineGrid grid,
            MineGenerationSettings settings)
        {
            int passableCellCount = CountPassableCells(grid);

            if (passableCellCount < settings.MinimumPassableCellCount)
            {
                return MineGridValidationResult.Invalid(
                    MineGridValidationIssue.InsufficientPassableCells);
            }

            var visited = new bool[grid.Width, grid.Height];
            var queue = new Queue<GridPosition>();

            TryVisitPassable(grid, visited, queue, settings.StartPosition);

            while (queue.Count > 0)
            {
                GridPosition current = queue.Dequeue();

                for (int i = 0; i < CardinalOffsets.Length; i++)
                {
                    TryVisitPassable(
                        grid,
                        visited,
                        queue,
                        current + CardinalOffsets[i]);
                }
            }

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    var position = new GridPosition(x, y);

                    if (!grid.TryGetCell(position, out TerrainCell cell))
                    {
                        continue;
                    }

                    if (cell.IsPassable && !visited[x, y])
                    {
                        return MineGridValidationResult.Invalid(
                            MineGridValidationIssue.DisconnectedPassableArea,
                            position);
                    }
                }
            }

            return MineGridValidationResult.Valid();
        }

        private static MineGridValidationResult ValidatePassableAreaDoesNotTouchVoid(MineGrid grid)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    var position = new GridPosition(x, y);

                    if (!grid.TryGetCell(position, out TerrainCell cell) || !cell.IsPassable)
                    {
                        continue;
                    }

                    for (int i = 0; i < CardinalOffsets.Length; i++)
                    {
                        GridPosition neighbor = position + CardinalOffsets[i];

                        if (grid.Contains(neighbor)
                            && grid.TryGetCell(neighbor, out TerrainCell neighborCell)
                            && neighborCell.Type == TerrainCellType.Void)
                        {
                            return MineGridValidationResult.Invalid(
                                MineGridValidationIssue.PassableAreaTouchesVoid,
                                position);
                        }
                    }
                }
            }

            return MineGridValidationResult.Valid();
        }

        private static MineGridValidationResult ValidateOnlyBoundaryWallsTouchVoid(MineGrid grid)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    var position = new GridPosition(x, y);

                    if (!grid.TryGetCell(position, out TerrainCell cell)
                        || cell.Type == TerrainCellType.Void
                        || cell.Type == TerrainCellType.BoundaryWall)
                    {
                        continue;
                    }

                    for (int i = 0; i < CardinalOffsets.Length; i++)
                    {
                        GridPosition neighbor = position + CardinalOffsets[i];

                        if (grid.Contains(neighbor)
                            && grid.TryGetCell(neighbor, out TerrainCell neighborCell)
                            && neighborCell.Type == TerrainCellType.Void)
                        {
                            return MineGridValidationResult.Invalid(
                                MineGridValidationIssue.NonBoundaryTerrainTouchesVoid,
                                position);
                        }
                    }
                }
            }

            return MineGridValidationResult.Valid();
        }

        private static int CountPassableCells(MineGrid grid)
        {
            int count = 0;

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    if (grid.TryGetCell(new GridPosition(x, y), out TerrainCell cell)
                        && cell.IsPassable)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private static void TryVisitPassable(
            MineGrid grid,
            bool[,] visited,
            Queue<GridPosition> queue,
            GridPosition position)
        {
            if (!grid.Contains(position) || visited[position.X, position.Y])
            {
                return;
            }

            if (!grid.TryGetCell(position, out TerrainCell cell) || !cell.IsPassable)
            {
                return;
            }

            visited[position.X, position.Y] = true;
            queue.Enqueue(position);
        }

        private static bool IsBoundaryWall(MineGrid grid, GridPosition position)
        {
            if (!grid.TryGetCell(position, out TerrainCell cell))
            {
                return false;
            }

            return cell.Type == TerrainCellType.BoundaryWall;
        }

        private static bool IsVoid(MineGrid grid, GridPosition position)
        {
            if (!grid.TryGetCell(position, out TerrainCell cell))
            {
                return false;
            }

            return cell.Type == TerrainCellType.Void;
        }
    }
}