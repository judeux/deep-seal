using System;
using System.Collections.Generic;
using DeepSeal.Core;
using DeepSeal.Mining;

namespace DeepSeal.ProceduralGeneration
{
    public static class MineGridGenerator
    {
        private static readonly GridPosition[] CardinalOffsets =
        {
            new GridPosition(0, 1),
            new GridPosition(1, 0),
            new GridPosition(0, -1),
            new GridPosition(-1, 0)
        };

        private static readonly GridPosition[] NeighborOffsets =
        {
            new GridPosition(-1, 1),
            new GridPosition(0, 1),
            new GridPosition(1, 1),
            new GridPosition(-1, 0),
            new GridPosition(1, 0),
            new GridPosition(-1, -1),
            new GridPosition(0, -1),
            new GridPosition(1, -1)
        };

        public static MineGenerationResult Generate(MineGenerationSettings settings)
        {
            settings.Validate();

            var random = new Random(settings.Seed);
            TerrainCell defaultCell = settings.ShapeMode == MineGenerationShapeMode.ConnectedCavern
                ? TerrainCell.Void
                : TerrainCell.MineableWall(settings.WallDurability);

            var grid = new MineGrid(
                settings.Width,
                settings.Height,
                defaultCell);

            switch (settings.ShapeMode)
            {
                case MineGenerationShapeMode.RandomScatter:
                    CarveRandomScatter(grid, settings, random);
                    break;

                case MineGenerationShapeMode.ConnectedCavern:
                    CarveConnectedCavern(grid, settings, random);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(settings),
                        settings.ShapeMode,
                        "Unsupported mine generation shape mode.");
            }

            return new MineGenerationResult(grid, settings);
        }

        private static void CarveRandomScatter(
            MineGrid grid,
            MineGenerationSettings settings,
            Random random)
        {
            for (int y = 1; y < settings.Height - 1; y++)
            {
                for (int x = 1; x < settings.Width - 1; x++)
                {
                    var position = new GridPosition(x, y);

                    if (settings.IsInStartClearArea(position)
                        || random.Next(100) < settings.TargetFloorPercent)
                    {
                        grid.TrySetCell(position, TerrainCell.Floor);
                    }
                }
            }
            SetRectangularBoundaryWalls(grid, settings);
        }

        private static void CarveConnectedCavern(
            MineGrid grid,
            MineGenerationSettings settings,
            Random random)
        {
            var floorFlags = new bool[settings.Width, settings.Height];
            var frontierFlags = new bool[settings.Width, settings.Height];
            var frontier = new List<GridPosition>();

            int floorCount = CarveStartArea(grid, settings, floorFlags);

            AddStartAreaFrontier(
                settings,
                floorFlags,
                frontierFlags,
                frontier);

            int targetCarvedCellCount = settings.TargetCarvedCellCount;

            while (floorCount < targetCarvedCellCount && frontier.Count > 0)
            {
                GridPosition position = TakeRandomFrontier(
                    random,
                    frontier,
                    frontierFlags);

                if (!settings.IsInCarvableArea(position) || floorFlags[position.X, position.Y])
                {
                    continue;
                }

                if (TryCarveFloor(grid, floorFlags, position))
                {
                    floorCount++;
                    AddFrontierNeighbors(
                        settings,
                        floorFlags,
                        frontierFlags,
                        frontier,
                        position);
                }
            }

            PlaceInternalWalls(
                grid,
                settings,
                random,
                floorFlags,
                ref floorCount);

            MineTerrainPresetPlacementRules.PlacePresets(
                grid,
                settings,
                random,
                MineTerrainPresetLibrary.CreatePrototypePresets());

            BuildMineableWallRind(grid, settings);
            BuildBoundaryWallShell(grid, settings);
        }

        private static int CarveStartArea(
            MineGrid grid,
            MineGenerationSettings settings,
            bool[,] floorFlags)
        {
            int floorCount = 0;

            for (int y = settings.StartPosition.Y - settings.StartClearRadius;
                 y <= settings.StartPosition.Y + settings.StartClearRadius;
                 y++)
            {
                for (int x = settings.StartPosition.X - settings.StartClearRadius;
                     x <= settings.StartPosition.X + settings.StartClearRadius;
                     x++)
                {
                    var position = new GridPosition(x, y);

                    if (TryCarveFloor(grid, floorFlags, position))
                    {
                        floorCount++;
                    }
                }
            }

            return floorCount;
        }

        private static void AddStartAreaFrontier(
            MineGenerationSettings settings,
            bool[,] floorFlags,
            bool[,] frontierFlags,
            List<GridPosition> frontier)
        {
            for (int y = settings.StartPosition.Y - settings.StartClearRadius;
                 y <= settings.StartPosition.Y + settings.StartClearRadius;
                 y++)
            {
                for (int x = settings.StartPosition.X - settings.StartClearRadius;
                     x <= settings.StartPosition.X + settings.StartClearRadius;
                     x++)
                {
                    AddFrontierNeighbors(
                        settings,
                        floorFlags,
                        frontierFlags,
                        frontier,
                        new GridPosition(x, y));
                }
            }
        }

        private static void AddFrontierNeighbors(
            MineGenerationSettings settings,
            bool[,] floorFlags,
            bool[,] frontierFlags,
            List<GridPosition> frontier,
            GridPosition position)
        {
            for (int i = 0; i < CardinalOffsets.Length; i++)
            {
                GridPosition candidate = position + CardinalOffsets[i];

                if (!settings.IsInCarvableArea(candidate)
                    || floorFlags[candidate.X, candidate.Y]
                    || frontierFlags[candidate.X, candidate.Y])
                {
                    continue;
                }

                frontierFlags[candidate.X, candidate.Y] = true;
                frontier.Add(candidate);
            }
        }

        private static GridPosition TakeRandomFrontier(
            Random random,
            List<GridPosition> frontier,
            bool[,] frontierFlags)
        {
            int index = random.Next(frontier.Count);
            GridPosition position = frontier[index];

            int lastIndex = frontier.Count - 1;
            frontier[index] = frontier[lastIndex];
            frontier.RemoveAt(lastIndex);

            frontierFlags[position.X, position.Y] = false;

            return position;
        }

        private static bool TryCarveFloor(
            MineGrid grid,
            bool[,] floorFlags,
            GridPosition position)
        {
            if (floorFlags[position.X, position.Y])
            {
                return false;
            }

            floorFlags[position.X, position.Y] = true;
            grid.TrySetCell(position, TerrainCell.Floor);
            return true;
        }

        private static void PlaceInternalWalls(
            MineGrid grid,
            MineGenerationSettings settings,
            Random random,
            bool[,] floorFlags,
            ref int floorCount)
        {
            int targetInternalWallCount = Math.Min(
                settings.TargetInternalWallCellCount,
                Math.Max(0, floorCount - settings.TargetFloorCellCount));

            if (targetInternalWallCount <= 0)
            {
                return;
            }

            int targetUnmineableWallCount = Math.Min(
                settings.TargetInternalUnmineableWallCellCount,
                targetInternalWallCount);

            List<GridPosition> candidates = CollectInternalWallCandidates(
                settings,
                floorFlags);

            Shuffle(random, candidates);

            int placedWallCount = 0;
            int placedUnmineableWallCount = 0;

            for (int i = 0; i < candidates.Count && placedWallCount < targetInternalWallCount; i++)
            {
                if (floorCount <= settings.TargetFloorCellCount)
                {
                    break;
                }

                GridPosition candidate = candidates[i];

                if (!CanConvertFloorToInternalWall(settings, floorFlags, candidate))
                {
                    continue;
                }

                floorFlags[candidate.X, candidate.Y] = false;

                if (!AreFloorsConnected(
                    floorFlags,
                    settings.StartPosition,
                    floorCount - 1))
                {
                    floorFlags[candidate.X, candidate.Y] = true;
                    continue;
                }

                TerrainCell wallCell = placedUnmineableWallCount < targetUnmineableWallCount
                    ? TerrainCell.UnmineableWall
                    : TerrainCell.MineableWall(settings.WallDurability);

                grid.TrySetCell(candidate, wallCell);
                floorCount--;
                placedWallCount++;

                if (wallCell.Type == TerrainCellType.UnmineableWall)
                {
                    placedUnmineableWallCount++;
                }
            }
        }

        private static List<GridPosition> CollectInternalWallCandidates(
            MineGenerationSettings settings,
            bool[,] floorFlags)
        {
            var candidates = new List<GridPosition>();

            for (int y = 0; y < settings.Height; y++)
            {
                for (int x = 0; x < settings.Width; x++)
                {
                    var position = new GridPosition(x, y);

                    if (CanConvertFloorToInternalWall(settings, floorFlags, position))
                    {
                        candidates.Add(position);
                    }
                }
            }

            return candidates;
        }

        private static bool CanConvertFloorToInternalWall(
            MineGenerationSettings settings,
            bool[,] floorFlags,
            GridPosition position)
        {
            if (!settings.IsInCarvableArea(position)
                || settings.IsInStartClearArea(position)
                || !floorFlags[position.X, position.Y])
            {
                return false;
            }

            // Internal wall candidates need enough nearby floor to avoid becoming part of the outer wall shell.
            return CountCardinalFloorNeighbors(floorFlags, position) >= 3;
        }

        private static int CountCardinalFloorNeighbors(
            bool[,] floorFlags,
            GridPosition position)
        {
            int count = 0;

            for (int i = 0; i < CardinalOffsets.Length; i++)
            {
                GridPosition neighbor = position + CardinalOffsets[i];

                if (neighbor.X < 0
                    || neighbor.Y < 0
                    || neighbor.X >= floorFlags.GetLength(0)
                    || neighbor.Y >= floorFlags.GetLength(1))
                {
                    continue;
                }

                if (floorFlags[neighbor.X, neighbor.Y])
                {
                    count++;
                }
            }

            return count;
        }

        private static bool AreFloorsConnected(
            bool[,] floorFlags,
            GridPosition startPosition,
            int expectedFloorCount)
        {
            if (expectedFloorCount <= 0 || !floorFlags[startPosition.X, startPosition.Y])
            {
                return false;
            }

            var visited = new bool[floorFlags.GetLength(0), floorFlags.GetLength(1)];
            var queue = new Queue<GridPosition>();

            visited[startPosition.X, startPosition.Y] = true;
            queue.Enqueue(startPosition);

            int visitedCount = 0;

            while (queue.Count > 0)
            {
                GridPosition current = queue.Dequeue();
                visitedCount++;

                for (int i = 0; i < CardinalOffsets.Length; i++)
                {
                    GridPosition neighbor = current + CardinalOffsets[i];

                    if (neighbor.X < 0
                        || neighbor.Y < 0
                        || neighbor.X >= floorFlags.GetLength(0)
                        || neighbor.Y >= floorFlags.GetLength(1)
                        || visited[neighbor.X, neighbor.Y]
                        || !floorFlags[neighbor.X, neighbor.Y])
                    {
                        continue;
                    }

                    visited[neighbor.X, neighbor.Y] = true;
                    queue.Enqueue(neighbor);
                }
            }

            return visitedCount == expectedFloorCount;
        }

        private static void Shuffle(
            Random random,
            List<GridPosition> positions)
        {
            for (int i = positions.Count - 1; i > 0; i--)
            {
                int swapIndex = random.Next(i + 1);
                GridPosition temp = positions[i];
                positions[i] = positions[swapIndex];
                positions[swapIndex] = temp;
            }
        }

        private static void BuildMineableWallRind(
            MineGrid grid,
            MineGenerationSettings settings)
        {
            for (int layer = 0; layer < settings.EdgeMineableWallThickness; layer++)
            {
                var positionsToFill = new List<GridPosition>();

                for (int y = 1; y < settings.Height - 1; y++)
                {
                    for (int x = 1; x < settings.Width - 1; x++)
                    {
                        var position = new GridPosition(x, y);

                        if (IsVoid(grid, position) && HasNeighborVisibleTerrain(grid, position))
                        {
                            positionsToFill.Add(position);
                        }
                    }
                }

                for (int i = 0; i < positionsToFill.Count; i++)
                {
                    grid.TrySetCell(
                        positionsToFill[i],
                        TerrainCell.MineableWall(settings.WallDurability));
                }
            }
        }

        private static void BuildBoundaryWallShell(
            MineGrid grid,
            MineGenerationSettings settings)
        {
            for (int y = 1; y < settings.Height - 1; y++)
            {
                for (int x = 1; x < settings.Width - 1; x++)
                {
                    var position = new GridPosition(x, y);

                    if (IsVoid(grid, position) && HasNeighborVisibleTerrain(grid, position))
                    {
                        grid.TrySetCell(position, TerrainCell.BoundaryWall);
                    }
                }
            }
        }

        private static bool IsVoid(MineGrid grid, GridPosition position)
        {
            return grid.TryGetCell(position, out TerrainCell cell)
                && cell.Type == TerrainCellType.Void;
        }

        private static bool HasNeighborVisibleTerrain(MineGrid grid, GridPosition position)
        {
            for (int i = 0; i < NeighborOffsets.Length; i++)
            {
                GridPosition neighbor = position + NeighborOffsets[i];

                if (grid.Contains(neighbor)
                    && grid.TryGetCell(neighbor, out TerrainCell neighborCell)
                    && neighborCell.Type != TerrainCellType.Void)
                {
                    return true;
                }
            }

            return false;
        }

        private static void SetRectangularBoundaryWalls(
            MineGrid grid,
            MineGenerationSettings settings)
        {
            for (int x = 0; x < settings.Width; x++)
            {
                grid.TrySetCell(new GridPosition(x, 0), TerrainCell.BoundaryWall);
                grid.TrySetCell(new GridPosition(x, settings.Height - 1), TerrainCell.BoundaryWall);
            }

            for (int y = 1; y < settings.Height - 1; y++)
            {
                grid.TrySetCell(new GridPosition(0, y), TerrainCell.BoundaryWall);
                grid.TrySetCell(new GridPosition(settings.Width - 1, y), TerrainCell.BoundaryWall);
            }
        }
    }
}