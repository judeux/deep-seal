# 0005. Prototype Domain and Tilemap Bootstrap

Date: 2026-06-18

## Decision

The first prototype foundation will use pure C# domain logic for grid, mining, and procedural generation, with Unity Tilemap rendering isolated in Unity adapter code.

Implemented foundation:

- `DeepSeal.Core`
  - `GridPosition`
  - `GridDirection`
- `DeepSeal.Mining`
  - `TerrainCellType`
  - `TerrainCell`
  - `MineGrid`
  - `MiningRules`
  - mining result types
- `DeepSeal.ProceduralGeneration`
  - generation settings
  - generation result
  - seeded mine grid generator
  - generated grid validator
- `DeepSeal.UnityAdapters.Tilemaps`
  - terrain tile set mapping
  - MineGrid to Tilemap renderer
- `DeepSeal.UnityAdapters.Prototype`
  - prototype bootstrap for generating and rendering a seeded mine grid

The prototype scene can display a deterministic, seed-based mine grid through Unity Tilemap.

## Rationale

Mining, movement, combat, and procedural generation all need a shared grid foundation. Keeping the rules in pure C# makes the domain logic testable in EditMode without scene dependencies.

Unity-specific rendering is isolated behind adapter components so the domain model remains independent from `UnityEngine`, `Tilemap`, scenes, prefabs, and imported art assets.

The first visual prototype only proves that generated MineGrid data can be displayed in Unity. It does not introduce player movement, mining input, combat, enemies, treasure, or extraction.

## Consequences

The current Tilemap layer is display-only.

Future interactive prototype work should build on the existing boundaries:

- player movement should query `MineGrid` passability;
- mining input should call `MiningRules`;
- Tilemap refresh should remain in Unity adapter code;
- procedural generation should remain deterministic and testable;
- gameplay rules should not move into MonoBehaviours.

## Status

Accepted.