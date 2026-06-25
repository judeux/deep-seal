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

Included:

- Seed-stable irregular floor regions.
- Start area remains passable.
- Outer border remains wall.
- Generated result remains compatible with current MineGrid consumers.
- EditMode tests for determinism, bounds, start passability, and minimum usable floor area.

Excluded:

- New wall types.
- Unmineable boundaries.
- Tile asset changes.
- Hand-authored presets.
- Biomes.

### 2-C. Terrain Wall Type Pass

Candidate additions:

- Mineable wall.
- Unmineable wall.
- Boundary wall.
- Durability/material variants.

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