using System;
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
        StartAreaBlocked = 5
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
        public static MineGridValidationResult Validate(MineGenerationResult result)
        {
            return Validate(result.Grid, result.Settings);
        }

        public static MineGridValidationResult Validate(
            MineGrid grid,
            MineGenerationSettings settings)
        {
            if(grid == null)
            {
                return MineGridValidationResult.Invalid(MineGridValidationIssue.GridIsNull);
            }

            try
            {
                settings.Validate();
            }
            catch(ArgumentException)
            {
                return MineGridValidationResult.Invalid(MineGridValidationIssue.InvalidSettings);
            }

            if(grid.Width != settings.Width || grid.Height != settings.Height)
            {
                return MineGridValidationResult.Invalid(MineGridValidationIssue.SizeMismatch);
            }
            
            MineGridValidationResult boundaryResult = ValidateBoundary(grid);

            if(!boundaryResult.IsValid)
            {
                return boundaryResult;
            }

            return ValidateStartArea(grid, settings);
        }

        private static MineGridValidationResult ValidateBoundary(MineGrid grid)
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

        private static bool IsBoundaryWall(MineGrid grid, GridPosition position)
        {
            if (!grid.TryGetCell(position, out TerrainCell cell))
            {
                return false;
            }

            return cell.Type == TerrainCellType.Wall && cell.Durability > 0;
        }
    }
}
