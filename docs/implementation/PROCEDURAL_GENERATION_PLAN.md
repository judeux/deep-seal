# Procedural Generation Plan

Date: 2026-06-25
Status: Draft
Scope: Prototype procedural generation direction after 2-A.

## Purpose

The current prototype validates the first playable loop, but the mine shape is still too rectangular. This document stages procedural generation improvements without mixing too many terrain features into one implementation step.

## Current Limitations

- Maps are generated inside a fixed rectangular width/height.
- Terrain semantics are still only floor and wall.
- Outer walls are currently ordinary walls.
- Hand-authored terrain presets are not supported.
- Existing enemy, treasure, extraction, reward, and upgrade systems depend on generated passable cells.

## Staged Plan

### 2-B. Procedural Mine Shape Pass

Status: Done

Completed:
- Added seed-stable connected cavern-style floor generation.
- Added `Void` cells to represent space outside the generated mine footprint.
- Kept the visible mine silhouette irregular by rendering `Void` as empty Tilemap space.
- Kept the start area passable.
- Kept the main passable area connected.
- Added EditMode tests for determinism, visible footprint behavior, start passability, and connected passable cells.
- Added optional mineable internal wall obstacles while preserving passable area connectivity.

Decision:
- The main passable area should be connected during 2-B.
- Isolated hidden spaces are deferred until a later discovery/mining-focused generation pass.
- `Void` represents outside-map footprint space, not an unmineable wall.

### 2-C. Terrain Wall Type Pass

Status: Done

Completed:
- Added explicit terrain semantics for mineable wall, unmineable wall, boundary wall, floor, and void.
- Kept `Void` as outside-footprint space.
- Kept boundary walls unmineable.
- Allowed generated connected caverns to include both mineable and unmineable internal wall obstacles.
- Preserved passable area connectivity and existing prototype loop behavior.

Notes:
- `Void` is already used for outside-map footprint space.
- 2-C should focus on wall semantics inside the generated footprint, such as mineable walls, unmineable walls, boundary walls, durability variants, and material variants.

### 2-D. Procedural Preset Placement Pass

Candidate additions:

- Preset footprint data.
- Placement validation.
- Connection rules.
- Seed-stable preset selection.

## Design Notes

- Shape generation should remain pure C# and deterministic.
- The generator should not depend on UnityEngine.
- Connectivity requirements should be explicit per step.
- All generator changes should preserve compatibility with movement, mining, enemies, treasure, extraction, rewards, and upgrades.

## Open Questions For Implementation

- Should 2-B guarantee one connected walkable region, or allow isolated pockets for mining discovery?
- Should the first shape algorithm be cellular automata, drunkard walk, room/corridor carving, or a hybrid?
- What minimum floor percentage is required for the current prototype loop?
- Should extraction, treasure, reward, and enemy spawners be adjusted after irregular shape generation?