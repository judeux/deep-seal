# 0001. Initial Project Direction

Date: 2026-06-15

## Decision

The project will be developed as a single-player expedition-management survival roguelite for Steam.

Core direction:
- Automatic combat similar to Vampire Survivors.
- Procedurally generated mining maps.
- Campaign progression through repeated expeditions.
- Sealstone accumulation across multiple expeditions.
- Miner failure states: injury, missing, rescue failure, and limited permanent death.
- 2D tile-based implementation with URP 2D visuals.
- No multiplayer in the initial scope.
- No complex fluid simulation in the initial scope.

## Rationale

The project is developed by a solo developer, so the initial scope must prioritize systems that are readable, testable, and expandable.

Mining, terrain destruction, automatic combat, expedition preparation, named encounters, and sub-dungeons are considered core.

Complex water or lava simulation is deferred because it is expensive to implement, difficult to balance, and may not be intuitive for players.

## Status

Accepted.