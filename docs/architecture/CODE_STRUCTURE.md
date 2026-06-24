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
  - Prototype automatic attack target selection rules.
  - Prototype hit point state and damage application rules.
  - Prototype enemy grid pathfinding rules.
  - Prototype enemy spawn position selection rules.

- `DeepSeal.UnityAdapters.Enemies`
  - Prototype enemy view adapter.
  - Prototype enemy spawner adapter.
  - Scene connection between enemy GameObjects and pure Combat movement rules.
  - Prototype-only enemy hit points and defeat handling for automatic attack validation.
  - Runtime prototype enemy spawning based on active enemy count and spawn interval.
  - Prototype enemy movement variation through spawner-provided movement interval and hit point values.

- `DeepSeal.Expedition`
  - Prototype treasure state.
  - Grid-position treasure pickup rules.
  - Prototype extraction marker state.
  - Simple extraction completion rules.
  - Prototype reward drop state.
  - Short-range reward drop pickup rules.

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
  - Prototype automatic attack adapter that targets spawned enemies.
  - Prototype player health adapter.
  - Prototype enemy contact damage adapter.
  - Minimal player defeat behavior for prototype combat validation.
  - Prototype treasure pickup adapter.
  - Prototype extraction completion adapter.
  - Prototype reward drop pickup adapter.

- `DeepSeal.UnityAdapters.Cameras`
  - Prototype camera follow adapter for keeping the player visible during movement and mining.

- `DeepSeal.UnityAdapters.Treasures`
  - Prototype treasure view adapter.
  - Prototype treasure spawner adapter.

- `DeepSeal.UnityAdapters.Extraction`
  - Prototype extraction marker view adapter.
  - Prototype extraction marker spawner adapter.

- `DeepSeal.UnityAdapters.UI`
  - Prototype OnGUI loop HUD for health, treasure, extraction state, defeat, and extraction completion.
  - Prototype wall mining durability overlay.

- `DeepSeal.UnityAdapters.RewardDrops`
  - Prototype reward drop view adapter.
  - Prototype reward drop spawner for enemy defeat and mining rewards.

Current constraints:

- Pure domain logic must not depend on `UnityEngine`.
- Unity scene, Tilemap, rendering, input, and MonoBehaviour code must stay in Unity adapter namespaces.
- Player movement and prototype combat input are currently temporary adapters; they should be replaced by InputActions when production input bindings are introduced.
- The current Tilemap adapter is clear and prototype-oriented, not optimized for large dirty-cell refresh workflows.
- Automatic attack, enemy contact damage, and player defeat are prototype-only and do not yet include formal weapon definitions, upgrades, combat UI, treasure, or extraction.
- Enemy and player hit point handling now share pure `DeepSeal.Combat` health rules, while Unity adapters remain responsible for scene visibility and component disabling.
- Treasure pickup is prototype-only and currently tracks only collected count/value, not inventory, economy, campaign persistence, or reward settlement.
- Extraction is prototype-only and currently represents completion with marker state and component disabling, not a full expedition result screen or campaign reward settlement.
- Prototype loop feedback currently uses temporary OnGUI adapters and should be replaced by a proper UI stack after the first loop is validated.
- Prototype tuning values currently live in scene and prefab Inspector settings, not in dedicated balance data assets.
- Enemy movement now has prototype grid pathfinding, but it is still not final AI and does not include enemy archetypes, attack patterns, or weighted terrain costs.
- Runtime enemy spawning exists as a prototype pressure tool and is still configured through scene Inspector values rather than production spawn tables.
- Reward drops are prototype-only and currently track only collected count/value, not inventory, economy, campaign settlement, or permanent progression.

Next planned area:

- Upgrade selection prototype.
- Use prototype reward value only as short-run upgrade input.
- Campaign progression, persistent upgrades, sealstones, and miner roster systems remain deferred.