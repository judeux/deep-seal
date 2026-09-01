using System;
using UnityEngine;

namespace DeepSeal.ProceduralGeneration
{
    /// <summary>
    /// 시드 기반 맵 생성에 사용할 바이옴 단위 파라미터를 담는 순수 데이터.
    /// 폭과 높이는 범위로 정의하고 나머지 값은 MineGenerationSettings와 같은 제약을 따른다.
    /// 바이옴 맵은 항상 ConnectedCavern 모드로 생성한다.
    /// </summary>
    public readonly struct MineBiome
    {
        public MineBiome(
            string displayName,
            int minWidth,
            int maxWidth,
            int minHeight,
            int maxHeight,
            int startClearRadius,
            int wallDurability,
            int targetFloorPercent,
            int internalWallPercent,
            int internalUnmineableWallPercent,
            int edgeMineableWallThickness,
            int presetPlacementCount,
            int presetPlacementAttempts)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentException("Biome display name must not be empty.", nameof(displayName));
            }

            if (minWidth < 3)
            {
                throw new ArgumentOutOfRangeException(nameof(minWidth), minWidth, "Minimum width must be at least 3.");
            }

            if (maxWidth < minWidth)
            {
                throw new ArgumentOutOfRangeException(nameof(maxWidth), maxWidth, "Maximum width must be greater than or equal to the minimum width.");
            }

            if (minHeight < 3)
            {
                throw new ArgumentOutOfRangeException(nameof(minHeight), minHeight, "Minimum height must be at least 3.");
            }

            if (maxHeight < minHeight)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHeight), maxHeight, "Maximum height must be greater than or equal to the minimum height.");
            }

            if (startClearRadius < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startClearRadius), startClearRadius, "Start clear radius cannot be negative.");
            }

            if (wallDurability <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(wallDurability), wallDurability, "Wall durability must be greater than zero.");
            }

            if (targetFloorPercent < 0 || targetFloorPercent > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(targetFloorPercent), targetFloorPercent, "Target floor percent must be between 0 and 100.");
            }

            if (internalWallPercent < 0 || internalWallPercent > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(internalWallPercent), internalWallPercent, "Internal wall percent must be between 0 and 100.");
            }

            if (internalUnmineableWallPercent < 0 || internalUnmineableWallPercent > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(internalUnmineableWallPercent), internalUnmineableWallPercent, "Internal unmineable wall percent must be between 0 and 100.");
            }

            if (edgeMineableWallThickness < 0
                || edgeMineableWallThickness > MineGenerationSettings.MaxEdgeMineableWallThickness)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(edgeMineableWallThickness),
                    edgeMineableWallThickness,
                    $"Edge mineable wall thickness must be between 0 and {MineGenerationSettings.MaxEdgeMineableWallThickness}.");
            }

            if (presetPlacementCount < 0 || presetPlacementCount > MineGenerationSettings.MaxPresetPlacementCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(presetPlacementCount),
                    presetPlacementCount,
                    $"Preset placement count must be between 0 and {MineGenerationSettings.MaxPresetPlacementCount}.");
            }

            if (presetPlacementAttempts < 0 || presetPlacementAttempts > MineGenerationSettings.MaxPresetPlacementAttempts)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(presetPlacementAttempts),
                    presetPlacementAttempts,
                    $"Preset placement attempts must be between 0 and {MineGenerationSettings.MaxPresetPlacementAttempts}.");
            }

            if (presetPlacementCount > 0 && presetPlacementAttempts <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(presetPlacementAttempts),
                    presetPlacementAttempts,
                    "Preset placement attempts must be greater than zero when preset placement count is greater than zero.");
            }

            DisplayName = displayName;
            MinWidth = minWidth;
            MaxWidth = maxWidth;
            MinHeight = minHeight;
            MaxHeight = maxHeight;
            StartClearRadius = startClearRadius;
            WallDurability = wallDurability;
            TargetFloorPercent = targetFloorPercent;
            InternalWallPercent = internalWallPercent;
            InternalUnmineableWallPercent = internalUnmineableWallPercent;
            EdgeMineableWallThickness = edgeMineableWallThickness;
            PresetPlacementCount = presetPlacementCount;
            PresetPlacementAttempts = presetPlacementAttempts;
        }

        public string DisplayName { get; }

        public int MinWidth { get; }

        public int MaxWidth { get; }

        public int MinHeight { get; }

        public int MaxHeight { get; }

        public int StartClearRadius { get; }

        public int WallDurability { get; }

        public int TargetFloorPercent { get; }

        public int InternalWallPercent { get; }

        public int InternalUnmineableWallPercent { get; }

        public int EdgeMineableWallThickness { get; }

        public int PresetPlacementCount { get; }

        public int PresetPlacementAttempts { get; }
    }
}
