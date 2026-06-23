# Prototype Tuning Pass

Date: 2026-06-23
Status: Draft
Reviewed Scene: `Assets/_Project/Scenes/Prototype/PrototypeMineGrid.unity`
Scope: 1-P Prototype Tuning Pass

## Purpose

Tune the first playable prototype loop after the prototype HUD and wall mining feedback became visible during play.

This pass changes Inspector values only. It does not add new systems, content, assets, or gameplay rules.

## Explicit Exclusions

This pass does not add:

- New runtime systems.
- New enemy types.
- New treasure types.
- Upgrade selection.
- Campaign systems.
- Full expedition result screen.
- Final UI art.
- New art, audio, VFX, or animation assets.
- Enemy pathfinding improvements.

## Tuning Goals

| Area | Goal |
|---|---|
| Map | Keep the map readable and small enough for short prototype runs. |
| Mining | Make wall breaking feel intentional but not slow. |
| Movement | Keep movement responsive while leaving time for mining/combat decisions. |
| Enemies | Add pressure without immediately ending the first loop. |
| Auto attack | Let combat happen automatically without removing all pressure too early. |
| Contact damage | Make nearby enemies feel dangerous but not instantly fatal. |
| Treasure | Require at least a short collection decision before extraction. |
| Extraction | Make return objective clear after treasure collection. |

## Starting Values

Record the values before tuning.

| Setting | Before |
|---|---:|
| Mine seed |  |
| Width |  |
| Height |  |
| Start X |  |
| Start Y |  |
| Start clear radius |  |
| Wall durability |  |
| Random floor percent |  |
| Player move speed |  |
| Mining interval seconds |  |
| Mining damage |  |
| Enemy spawn offsets |  |
| Enemy max hit points |  |
| Enemy move interval seconds |  |
| Attack interval seconds |  |
| Attack range cells |  |
| Attack damage |  |
| Contact range cells |  |
| Contact damage |  |
| Contact damage interval seconds |  |
| Treasure spawn offsets and values |  |
| Required treasure value |  |
| Extraction marker offset |  |

## Proposed Tuned Values

| Setting | Tuned Value |
|---|---:|
| Mine seed | 1203 |
| Width | 40 |
| Height | 26 |
| Start X | 20 |
| Start Y | 13 |
| Start clear radius | 2 |
| Wall durability | 3 |
| Random floor percent | 55 |
| Player move speed | 3.5 |
| Mining interval seconds | 0.22 |
| Mining damage | 1 |
| Enemy spawn offsets | `(2,0)`, `(0,2)`, `(-2,0)`, `(0,-2)` |
| Enemy max hit points | 4 |
| Enemy move interval seconds | 0.45 |
| Attack interval seconds | 0.6 |
| Attack range cells | 3 |
| Attack damage | 1 |
| Contact range cells | 1 |
| Contact damage | 1 |
| Contact damage interval seconds | 1.0 |
| Treasure spawn offsets and values | `(2,2)=1`, `(-2,2)=1`, `(3,-2)=2` |
| Required treasure value | 2 |
| Extraction marker offset | `(-2,-2)` |

## Playtest Runs

Run at least three short sessions.

| Run | Seed/Settings | Result | Notes |
|---:|---|---|---|
| 1 | Proposed tuned values | Pass |  |
| 2 | Same values, intentional mining route | Pass |  |
| 3 | Same values, enemy pressure check | Pass |  |

Use one of:

- `Pass`
- `Fail`
- `Partial`
- `Not Tested`

## Evaluation Checklist

| Check | Result | Notes |
|---|---|---|
| Scene opens without Console compile errors | Pass |  |
| Player can complete treasure pickup and extraction | Pass |  |
| Mining one wall feels readable with durability overlay | Pass |  |
| Mining does not feel too slow for a short prototype loop | Pass |  |
| Player movement remains responsive | Pass |  |
| Enemies create pressure before being defeated | Pass |  |
| Auto attack still defeats enemies without manual aiming | Pass |  |
| Contact damage is readable through HUD HP changes | Pass |  |
| Player can survive at least one mistake | Pass |  |
| Required treasure value creates a short objective | Pass |  |
| Extraction remains reachable after collecting treasure | Pass |  |
| The loop remains playable end-to-end | Pass |  |

## Observed Problems

| Severity | Problem | Reproduction | Suggested Follow-up |
|---|---|---|---|
| P1 | Enemies stop moving when a non-passable wall blocks the direct line toward the player. | Place or observe an enemy with a wall between it and the player. The enemy can stall instead of routing around terrain. | Add a grid-based enemy pathfinding rule in pure C# and connect it through the Unity enemy adapter. |
| P2 | Enemy movement feels too uniform because enemies currently share similar movement cadence and decision behavior. | Observe multiple enemies approaching the player during a short loop. They tend to read as one repeated behavior. | Add small prototype differences such as movement interval, chase priority, or per-enemy movement settings. |
| P2 | Enemy pressure is front-loaded and static because enemies are placed at scene start rather than spawned over time or by population rules. | Continue a run after defeating initial enemies. The combat pressure can drop too much. | Add a prototype runtime spawner that can spawn enemies on a timer or maintain a minimum active enemy count. |
| P2 | Mining and enemy defeats do not yet create immediate reward feedback. | Mine walls or defeat enemies during a loop. The action changes navigation/combat state but does not produce a collectible reward. | Add prototype reward drops and short-range pickup after enemy defeat and selected mining actions. |

Severity values:

- `P0`: blocks Play Mode or loop completion.
- `P1`: breaks a core loop behavior.
- `P2`: weak or confusing but playable.
- `P3`: polish or later tuning.

## Tuning Decision

| Decision Item | Value |
|---|---|
| Loop verdict after tuning | Stable enough for next prototype step |
| Values to keep | Tuned scene and prefab values are acceptable as the current prototype baseline. |
| Values to revert or adjust | No immediate revert required. Future values may change after enemy navigation and runtime spawning are added. |
| Next implementation direction | 1-Q Enemy Navigation and Spawn Pressure Pass |
| Deferred issues | Prototype reward drops should follow after enemy movement/spawn pressure is reliable. Full upgrade selection, campaign rewards, and polished combat feedback remain deferred. |

Suggested verdict values:

- `Stable enough for next prototype step`
- `Needs another tuning pass`
- `Needs focused fix before tuning continues`

## Recommended Next Step

### 1-Q. Enemy Navigation and Spawn Pressure Pass

Goal:

- Improve enemy pressure now that the first playable loop is visible and tunable.
- Prevent enemies from stalling when walls block direct movement toward the player.
- Add small movement differences so enemies do not all feel identical.
- Add prototype runtime spawning based on time interval or minimum active enemy count.

Why this comes next:

- The tuning pass showed that enemy pressure is currently limited more by movement and spawning structure than by numeric balance.
- Better enemy navigation should be solved before adding reward drops, because reward pacing depends on how often enemies meaningfully reach combat range.

Explicit exclusions:

- No final enemy AI.
- No enemy animation polish.
- No named elites or bosses.
- No campaign reward settlement.
- No upgrade selection.

## Verification Log

| Command | Result | Notes |
|---|---|---|
| `git diff --check` | Pass |  |
| `.\tools\verify-project.ps1` | Pass |  |
| `.\tools\test-editmode.ps1` | Pass |  |

## Final Notes

What improved:

- The prototype loop is now easier to evaluate because HP, treasure, extraction state, loop result, and wall mining progress are visible.
- Tuned values are good enough to serve as a temporary baseline for the next prototype step.

What still feels weak:

- Enemies can stall when terrain blocks direct movement.
- Enemy behavior reads as too uniform.
- Combat pressure can drop after the initial enemies are defeated.
- Mining and enemy defeat need immediate reward feedback.

What should happen next:

- Implement a focused enemy navigation and spawn pressure pass.
- Keep pathfinding and spawn rules testable in pure C# where possible.
- Use Unity adapters only for scene spawning, transforms, prefabs, and runtime wiring.

What should stay deferred:

- Final combat balance.
- Full upgrade selection.
- Campaign reward settlement.
- Polished VFX, audio, animation, and production UI.