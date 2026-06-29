using System;
using DeepSeal.Core;

namespace DeepSeal.ProceduralGeneration
{
    public readonly struct MineGenerationSettings
    {
        public const int RectangularBoundaryInset = 1;
        public const int ConnectedCavernCarveInset = 2;
        public const int DefaultEdgeMineableWallThickness = 1;
        public const int MaxEdgeMineableWallThickness = 3;

        public MineGenerationSettings(
            int width,
            int height,
            int seed,
            GridPosition startPosition,
            int startClearRadius,
            int wallDurability,
            int randomFloorPercent)
            : this(
                width,
                height,
                seed,
                startPosition,
                startClearRadius,
                wallDurability,
                randomFloorPercent,
                MineGenerationShapeMode.RandomScatter,
                0,
                0,
                0)
        {
        }

        public MineGenerationSettings(
            int width,
            int height,
            int seed,
            GridPosition startPosition,
            int startClearRadius,
            int wallDurability,
            int targetFloorPercent,
            MineGenerationShapeMode shapeMode)
            : this(
                width,
                height,
                seed,
                startPosition,
                startClearRadius,
                wallDurability,
                targetFloorPercent,
                shapeMode,
                0,
                0,
                GetDefaultEdgeMineableWallThickness(shapeMode))
        {
        }

        public MineGenerationSettings(
            int width,
            int height,
            int seed,
            GridPosition startPosition,
            int startClearRadius,
            int wallDurability,
            int targetFloorPercent,
            MineGenerationShapeMode shapeMode,
            int internalWallPercent)
            : this(
                width,
                height,
                seed,
                startPosition,
                startClearRadius,
                wallDurability,
                targetFloorPercent,
                shapeMode,
                internalWallPercent,
                0,
                GetDefaultEdgeMineableWallThickness(shapeMode))
        {
        }

        public MineGenerationSettings(
            int width,
            int height,
            int seed,
            GridPosition startPosition,
            int startClearRadius,
            int wallDurability,
            int targetFloorPercent,
            MineGenerationShapeMode shapeMode,
            int internalWallPercent,
            int internalUnmineableWallPercent)
            : this(
                width,
                height,
                seed,
                startPosition,
                startClearRadius,
                wallDurability,
                targetFloorPercent,
                shapeMode,
                internalWallPercent,
                internalUnmineableWallPercent,
                GetDefaultEdgeMineableWallThickness(shapeMode))
        {
        }

        public MineGenerationSettings(
            int width,
            int height,
            int seed,
            GridPosition startPosition,
            int startClearRadius,
            int wallDurability,
            int targetFloorPercent,
            MineGenerationShapeMode shapeMode,
            int internalWallPercent,
            int internalUnmineableWallPercent,
            int edgeMineableWallThickness)
        {
            ValidateArguments(
                width,
                height,
                startPosition,
                startClearRadius,
                wallDurability,
                targetFloorPercent,
                shapeMode,
                internalWallPercent,
                internalUnmineableWallPercent,
                edgeMineableWallThickness);

            Width = width;
            Height = height;
            Seed = seed;
            StartPosition = startPosition;
            StartClearRadius = startClearRadius;
            WallDurability = wallDurability;
            TargetFloorPercent = targetFloorPercent;
            ShapeMode = shapeMode;
            InternalWallPercent = internalWallPercent;
            InternalUnmineableWallPercent = internalUnmineableWallPercent;
            EdgeMineableWallThickness = edgeMineableWallThickness;
        }

        public int Width { get; }

        public int Height { get; }

        public int Seed { get; }

        public GridPosition StartPosition { get; }

        public int StartClearRadius { get; }

        public int WallDurability { get; }

        public int TargetFloorPercent { get; }

        public int InternalUnmineableWallPercent { get; }

        public int RandomFloorPercent => TargetFloorPercent;

        public MineGenerationShapeMode ShapeMode { get; }

        public int InternalWallPercent { get; }

        public int InteriorCellCount => (Width - 2) * (Height - 2);

        public int EdgeMineableWallThickness { get; }

        public int CarvableInteriorCellCount
        {
            get
            {
                int inset = CarveInset;
                return (Width - inset * 2) * (Height - inset * 2);
            }
        }

        public int StartClearCellCount
        {
            get
            {
                int diameter = StartClearRadius * 2 + 1;
                return diameter * diameter;
            }
        }

        public int TargetFloorCellCount
        {
            get
            {
                int baseCellCount = ShapeMode == MineGenerationShapeMode.ConnectedCavern
                    ? CarvableInteriorCellCount
                    : InteriorCellCount;

                int requestedFloorCount = (baseCellCount * TargetFloorPercent + 50) / 100;

                return Math.Min(
                    baseCellCount,
                    Math.Max(StartClearCellCount, requestedFloorCount));
            }
        }

        public int TargetInternalWallCellCount
        {
            get
            {
                if (ShapeMode != MineGenerationShapeMode.ConnectedCavern || InternalWallPercent <= 0)
                {
                    return 0;
                }

                int requestedWallCount = (TargetFloorCellCount * InternalWallPercent + 50) / 100;
                int availableExtraCells = Math.Max(0, CarvableInteriorCellCount - TargetFloorCellCount);

                return Math.Min(availableExtraCells, requestedWallCount);
            }
        }

        public int TargetInternalUnmineableWallCellCount
        {
            get
            {
                if (TargetInternalWallCellCount <= 0 || InternalUnmineableWallPercent <= 0)
                {
                    return 0;
                }

                return (TargetInternalWallCellCount * InternalUnmineableWallPercent + 50) / 100;
            }
        }

        public int TargetCarvedCellCount => Math.Min(
            CarvableInteriorCellCount,
            TargetFloorCellCount + TargetInternalWallCellCount);

        public int CarveInset => GetCarveInset(ShapeMode, EdgeMineableWallThickness);

        public void Validate()
        {
            ValidateArguments(
                Width,
                Height,
                StartPosition,
                StartClearRadius,
                WallDurability,
                TargetFloorPercent,
                ShapeMode,
                InternalWallPercent,
                InternalUnmineableWallPercent,
                EdgeMineableWallThickness);
        }

        public bool IsInStartClearArea(GridPosition position)
        {
            return Math.Abs(position.X - StartPosition.X) <= StartClearRadius
                && Math.Abs(position.Y - StartPosition.Y) <= StartClearRadius;
        }

        public bool IsInCarvableArea(GridPosition position)
        {
            int inset = CarveInset;

            return position.X >= inset
                && position.Y >= inset
                && position.X < Width - inset
                && position.Y < Height - inset;
        }

        public static int GetCarveInset(MineGenerationShapeMode shapeMode)
        {
            return shapeMode == MineGenerationShapeMode.ConnectedCavern
                ? ConnectedCavernCarveInset
                : RectangularBoundaryInset;
        }

        public static int GetCarveInset(
            MineGenerationShapeMode shapeMode,
            int edgeMineableWallThickness)
        {
            return shapeMode == MineGenerationShapeMode.ConnectedCavern
                ? ConnectedCavernCarveInset + edgeMineableWallThickness
                : RectangularBoundaryInset;
        }

        private static void ValidateArguments(
            int width,
            int height,
            GridPosition startPosition,
            int startClearRadius,
            int wallDurability,
            int targetFloorPercent,
            MineGenerationShapeMode shapeMode,
            int internalWallPercent,
            int internalUnmineableWallPercent,
            int edgeMineableWallThickness)
        {
            if (!IsValidShapeMode(shapeMode))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(shapeMode),
                    shapeMode,
                    "Unsupported mine generation shape mode.");
            }

            if (width < 3)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(width),
                    width,
                    "Mine width must be at least 3.");
            }

            if (height < 3)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(height),
                    height,
                    "Mine height must be at least 3.");
            }

            if (startClearRadius < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(startClearRadius),
                    startClearRadius,
                    "Start clear radius cannot be negative.");
            }

            if (wallDurability <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(wallDurability),
                    wallDurability,
                    "Wall durability must be greater than zero.");
            }

            if (targetFloorPercent < 0 || targetFloorPercent > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(targetFloorPercent),
                    targetFloorPercent,
                    "Target floor percent must be between 0 and 100.");
            }

            if (internalWallPercent < 0 || internalWallPercent > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(internalWallPercent),
                    internalWallPercent,
                    "Internal wall percent must be between 0 and 100.");
            }

            if (internalUnmineableWallPercent < 0 || internalUnmineableWallPercent > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(internalUnmineableWallPercent),
                    internalUnmineableWallPercent,
                    "Internal unmineable wall percent must be between 0 and 100.");
            }

            if (edgeMineableWallThickness < 0 || edgeMineableWallThickness > MaxEdgeMineableWallThickness)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(edgeMineableWallThickness),
                    edgeMineableWallThickness,
                    $"Edge mineable wall thickness must be between 0 and {MaxEdgeMineableWallThickness}.");
            }

            int inset = GetCarveInset(shapeMode, edgeMineableWallThickness);
            int minStartX = inset + startClearRadius;
            int maxStartX = width - 1 - inset - startClearRadius;
            int minStartY = inset + startClearRadius;
            int maxStartY = height - 1 - inset - startClearRadius;

            if (minStartX > maxStartX || minStartY > maxStartY)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(startClearRadius),
                    startClearRadius,
                    "Start clear radius is too large for this grid size and shape mode.");
            }

            if (startPosition.X < minStartX
                || startPosition.X > maxStartX
                || startPosition.Y < minStartY
                || startPosition.Y > maxStartY)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(startPosition),
                    startPosition,
                    "Start position and clear radius must stay inside the carveable area.");
            }
        }

        private static bool IsValidShapeMode(MineGenerationShapeMode shapeMode)
        {
            return shapeMode == MineGenerationShapeMode.RandomScatter
                || shapeMode == MineGenerationShapeMode.ConnectedCavern;
        }

        private static int GetDefaultEdgeMineableWallThickness(MineGenerationShapeMode shapeMode)
        {
            return shapeMode == MineGenerationShapeMode.ConnectedCavern
                ? DefaultEdgeMineableWallThickness
                : 0;
        }
    }
}