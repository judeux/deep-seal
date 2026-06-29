using System;
using DeepSeal.Core;
using DeepSeal.Mining;

namespace DeepSeal.ProceduralGeneration
{
    /// <summary>
    /// Places authored terrain presets into generated mine grids while preserving prototype generation invariants.
    /// </summary>
    public static class MineTerrainPresetPlacementRules
    {
        private static readonly GridPosition[] CardinalOffsets =
        {
            new GridPosition(0, 1),
            new GridPosition(1, 0),
            new GridPosition(0, -1),
            new GridPosition(-1, 0)
        };

        public static int PlacePresets(
            MineGrid grid,
            MineGenerationSettings settings,
            Random random,
            MineTerrainPreset[] presets)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            settings.Validate();

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (presets == null)
            {
                throw new ArgumentNullException(nameof(presets));
            }

            if (grid.Width != settings.Width || grid.Height != settings.Height)
            {
                throw new ArgumentException(
                    "Grid size must match generation settings.",
                    nameof(grid));
            }

            if (settings.ShapeMode != MineGenerationShapeMode.ConnectedCavern
                || settings.PresetPlacementCount <= 0
                || presets.Length == 0)
            {
                return 0;
            }

            int placedCount = 0;
            int attempts = 0;

            while (placedCount < settings.PresetPlacementCount
                   && attempts < settings.PresetPlacementAttempts)
            {
                attempts++;

                MineTerrainPreset preset = presets[random.Next(presets.Length)];

                if (!TryChooseOrigin(settings, preset, random, out GridPosition origin))
                {
                    continue;
                }

                if (TryPlacePreset(grid, settings, preset, origin))
                {
                    placedCount++;
                }
            }

            return placedCount;
        }

        public static bool TryPlacePreset(
            MineGrid grid,
            MineGenerationSettings settings,
            MineTerrainPreset preset,
            GridPosition origin)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            settings.Validate();

            if (grid.Width != settings.Width || grid.Height != settings.Height)
            {
                throw new ArgumentException(
                    "Grid size must match generation settings.",
                    nameof(grid));
            }

            if (preset.Width <= 0 || preset.Height <= 0 || preset.PaintedCellCount <= 0)
            {
                return false;
            }

            if (!CanPlacePreset(grid, settings, preset, origin))
            {
                return false;
            }

            var changedPositions = new GridPosition[preset.PaintedCellCount];
            var previousCells = new TerrainCell[preset.PaintedCellCount];
            int changedCount = 0;

            for (int y = 0; y < preset.Height; y++)
            {
                for (int x = 0; x < preset.Width; x++)
                {
                    MineTerrainPresetCell presetCell = preset.GetCell(x, y);

                    if (presetCell == MineTerrainPresetCell.Ignore)
                    {
                        continue;
                    }

                    var position = new GridPosition(origin.X + x, origin.Y + y);

                    grid.TryGetCell(position, out TerrainCell previousCell);

                    changedPositions[changedCount] = position;
                    previousCells[changedCount] = previousCell;
                    changedCount++;

                    grid.TrySetCell(
                        position,
                        ToTerrainCell(presetCell, settings.WallDurability));
                }
            }

            if (!IsStartAreaPassable(grid, settings)
                || !AreAllPassableCellsConnected(grid, settings.StartPosition))
            {
                Rollback(grid, changedPositions, previousCells, changedCount);
                return false;
            }

            return true;
        }

        private static bool TryChooseOrigin(
            MineGenerationSettings settings,
            MineTerrainPreset preset,
            Random random,
            out GridPosition origin)
        {
            int minX = settings.CarveInset;
            int minY = settings.CarveInset;
            int maxX = settings.Width - settings.CarveInset - preset.Width;
            int maxY = settings.Height - settings.CarveInset - preset.Height;

            if (maxX < minX || maxY < minY)
            {
                origin = default;
                return false;
            }

            origin = new GridPosition(
                random.Next(minX, maxX + 1),
                random.Next(minY, maxY + 1));

            return true;
        }

        private static bool CanPlacePreset(
            MineGrid grid,
            MineGenerationSettings settings,
            MineTerrainPreset preset,
            GridPosition origin)
        {
            for (int y = 0; y < preset.Height; y++)
            {
                for (int x = 0; x < preset.Width; x++)
                {
                    MineTerrainPresetCell presetCell = preset.GetCell(x, y);

                    if (presetCell == MineTerrainPresetCell.Ignore)
                    {
                        continue;
                    }

                    var position = new GridPosition(origin.X + x, origin.Y + y);

                    if (!settings.IsInCarvableArea(position)
                        || settings.IsInStartClearArea(position))
                    {
                        return false;
                    }

                    if (!grid.TryGetCell(position, out TerrainCell existingCell)
                        || existingCell.Type == TerrainCellType.Void
                        || existingCell.Type == TerrainCellType.BoundaryWall)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static TerrainCell ToTerrainCell(
            MineTerrainPresetCell presetCell,
            int wallDurability)
        {
            switch (presetCell)
            {
                case MineTerrainPresetCell.Floor:
                    return TerrainCell.Floor;

                case MineTerrainPresetCell.MineableWall:
                    return TerrainCell.MineableWall(wallDurability);

                case MineTerrainPresetCell.UnmineableWall:
                    return TerrainCell.UnmineableWall;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(presetCell),
                        presetCell,
                        "Cannot convert this preset cell to terrain.");
            }
        }

        private static bool IsStartAreaPassable(
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
                    if (!grid.TryGetCell(new GridPosition(x, y), out TerrainCell cell)
                        || !cell.IsPassable)
                    {
                        return false;
                    }
                }
            }

            return true;
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

        private static bool AreAllPassableCellsConnected(
            MineGrid grid,
            GridPosition startPosition)
        {
            if (!grid.TryGetCell(startPosition, out TerrainCell startCell)
                || !startCell.IsPassable)
            {
                return false;
            }

            int expectedPassableCount = CountPassableCells(grid);

            if (expectedPassableCount <= 0)
            {
                return false;
            }

            var visited = new bool[grid.Width, grid.Height];
            var queue = new GridPosition[expectedPassableCount];
            int head = 0;
            int tail = 0;

            visited[startPosition.X, startPosition.Y] = true;
            queue[tail] = startPosition;
            tail++;

            int visitedCount = 0;

            while (head < tail)
            {
                GridPosition current = queue[head];
                head++;
                visitedCount++;

                for (int i = 0; i < CardinalOffsets.Length; i++)
                {
                    GridPosition neighbor = current + CardinalOffsets[i];

                    if (!grid.Contains(neighbor)
                        || visited[neighbor.X, neighbor.Y]
                        || !grid.TryGetCell(neighbor, out TerrainCell neighborCell)
                        || !neighborCell.IsPassable)
                    {
                        continue;
                    }

                    visited[neighbor.X, neighbor.Y] = true;
                    queue[tail] = neighbor;
                    tail++;
                }
            }

            return visitedCount == expectedPassableCount;
        }

        private static void Rollback(
            MineGrid grid,
            GridPosition[] changedPositions,
            TerrainCell[] previousCells,
            int changedCount)
        {
            for (int i = 0; i < changedCount; i++)
            {
                grid.TrySetCell(changedPositions[i], previousCells[i]);
            }
        }
    }
}