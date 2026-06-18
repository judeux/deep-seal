## Current Development Phase

Interactive prototype foundation.

Implemented:
- Core grid primitives.
- Basic mining domain model.
- Seed-based procedural mine grid generation.
- Basic generated mine grid validation.
- Unity Tilemap rendering adapter for generated MineGrid data.
- Prototype scene that displays a seed-based mine grid through Tilemap.
- Prototype player movement over passable MineGrid cells.
- Wall collision based on MineGrid terrain passability.
- EditMode tests for core, mining, and procedural generation rules.

Not implemented yet:
- Mining input and runtime terrain updates.
- Automatic combat.
- Enemy spawning.
- Treasure pickup.
- Extraction and return flow.

Next:
- Add mining input that applies MiningRules to the current MineGrid and refreshes the Tilemap.