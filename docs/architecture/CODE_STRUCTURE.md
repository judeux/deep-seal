# Code Structure

This document describes the initial intended code structure for Deep Seal.

## Principle

Use pure C# for game rules whenever possible and use MonoBehaviours as Unity-facing adapters.

This keeps mining, procedural generation, combat calculations, and expedition rules easier to test.

## Initial Runtime Areas

Create folders only when files are needed.

Recommended areas:

- `Core`: shared primitives and utilities.
- `Bootstrap`: startup and scene initialization.
- `Expedition`: one-run expedition flow.
- `Mining`: tile damage, terrain cells, mining tools.
- `Combat`: weapons, targeting, damage, enemies.
- `ProceduralGeneration`: map layout and validation.
- `Encounters`: named elites, bosses, and sub-dungeon hooks.
- `Campaign`: long-term miner state, injuries, missing states, sealstones.
- `UI`: runtime UI presenters and view adapters.

## Editor Areas

- `Editor/Build`: command-line build helpers.
- `Editor/Tools`: project-specific editor utilities.

## Test Areas

- `Tests/EditMode`: pure logic tests.
- `Tests/PlayMode`: scene and Unity lifecycle tests.

## Dependency Direction

Prefer this dependency direction:

```text
Unity Scene / MonoBehaviour Adapters
        ↓
Runtime Systems
        ↓
Pure Domain Rules / Data Structures
```

Pure domain classes should not depend on Unity scenes or prefabs.

## Early Assembly Definition Policy

Do not over-split assemblies early.

If compilation time or test boundaries require assembly definitions, start with:

- `DeepSeal.Runtime.asmdef`
- `DeepSeal.Editor.asmdef`
- `DeepSeal.Tests.EditMode.asmdef`
- `DeepSeal.Tests.PlayMode.asmdef`

## Current Runtime Structure

The runtime code structure has started with pure C# domain logic plus a thin Unity adapter layer.

Implemented pure domain areas:

- `DeepSeal.Core`
  - Integer grid position primitives.
  - Grid direction primitives.

- `DeepSeal.Mining`
  - Terrain cell type.
  - Terrain cell state.
  - Mine grid storage and safe cell access.
  - Basic mining rule calculation.

- `DeepSeal.ProceduralGeneration`
  - Seed-based prototype mine map generation.
  - Basic generated mine grid validation.
  - Deterministic generation tests for repeated seed/settings.

- `DeepSeal.Combat`
  - Prototype enemy state.
  - Simple MineGrid-based enemy movement rules.
  - Enemy movement result types.

- `DeepSeal.UnityAdapters.Enemies`
  - Prototype enemy view adapter.
  - Prototype enemy spawner adapter.
  - Scene connection between enemy GameObjects and pure Combat movement rules.

Implemented Unity adapter areas:

- `DeepSeal.UnityAdapters.Tilemaps`
  - Terrain cell type to TileBase mapping.
  - MineGrid to Unity Tilemap rendering.

- `DeepSeal.UnityAdapters.Grid`
  - Prototype conversion between `GridPosition` and Unity world coordinates.

- `DeepSeal.UnityAdapters.Prototype`
  - Prototype scene bootstrap that generates a seeded MineGrid, renders it to Tilemap, and exposes the current generated grid to scene adapters.

- `DeepSeal.UnityAdapters.Player`
  - Temporary Keyboard-based prototype player movement.
  - Passability checks against the current `MineGrid`.
  - Prototype mining input that applies `MiningRules` and refreshes the Tilemap after terrain changes.

- `DeepSeal.UnityAdapters.Cameras`
  - Prototype camera follow adapter for keeping the player visible during movement and mining.

Current constraints:

- Pure domain logic must not depend on `UnityEngine`.
- Unity scene, Tilemap, rendering, input, and MonoBehaviour code must stay in Unity adapter namespaces.
- Player movement is currently a prototype adapter using `Keyboard.current`; it should be replaced by InputActions when production input bindings are introduced.
- The current Tilemap adapter is display-only.
- Combat, enemies, treasure, and extraction flows are intentionally not implemented yet.
- Automatic attack, damage/death, treasure, and extraction flows are intentionally not implemented yet.

Next planned area:

- Mining input that applies `MiningRules` to the current `MineGrid`.
- Runtime Tilemap refresh after terrain changes.