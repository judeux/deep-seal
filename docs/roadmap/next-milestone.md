# Next Milestone: Prototype Pacing and Optional Sub-Dungeon

- Status: In Progress — 3-E-1 verified; remaining 3-E is `Awaiting Proposal`
- Current gameplay owner: ZCode, based on the project owner's 2026-09-03 report
- Target Unity version: `6000.3.17f1`
- Historical step index: [`../implementation/PROTOTYPE_ROADMAP.md`](../implementation/PROTOTYPE_ROADMAP.md)
- Current implementation snapshot: [`../project/current-state.md`](../project/current-state.md)

## Goal

현재 prototype의 map scale, movement pacing과 route choice를 개선한 뒤 한 개의 optional
sub-dungeon으로 biome exploration 선택을 검증한다.

이 milestone은 기존 `3-E. Movement Pacing and Map Openness Pass`와 그 다음으로 승인된
`3-F. Sub-Dungeon Prototype`의 순서를 보존한다.

## Work units

### 3-E-1. Map scale and initial pacing experiment

- Status: Verified
- Owner at completion: ZCode
- Implementation commit: `ab3344a`
- Main integration commit: `0cb910f`
- Documentation commit: `6aec973`

Verified scope recorded in the historical roadmap:

- Generated map width and height scale-up
- Relative character-size reduction through camera framing
- Approximately halved base movement pacing
- Early spawn grace period
- Larger pathfinding budgets
- Owner playtest found no spawn-tick frame drop in the tested sessions

Known findings:

- Camera framing reveals too much of the map and needs a later local-view adjustment.
- Enemy spawn style needs a dedicated later review.
- Longer and harsher performance runs have not been verified.
- Treasure placement and balance are deferred to a later tuning pass.

### Remaining 3-E work

- Status: Awaiting Proposal
- Owner: ZCode

The exact next 3-E work unit is not present in repository documentation. Do not infer or
implement it from the interim findings alone. The owner must propose the next bounded unit,
explicit exclusions and verification gate for user approval.

Before that proposal becomes an implementation task, ZCode must use a dedicated clean worktree
and a new `zcode/<short-scope>` branch based on current `origin/main`.

### 3-F. Sub-Dungeon Prototype

- Status: Planned — starts only after 3-E completion is reviewed

Goal:

- Add one optional sub-dungeon prototype.
- Diversify biome flavor beyond the existing biome-selection pass.

Explicit exclusions:

- No full sub-dungeon rotation from the GDD.
- No counter-play item reward system.
- No campaign-scale sub-dungeon persistence.

## Milestone exclusions

- New dash, teleport or unrelated player movement mechanic
- Fog-of-war or a production vision system
- Final enemy spawn tables or final treasure balance
- Campaign management, injury/missing state or sealstone progression
- Production art, final UI, localization framework or save system

## Completion gates

- Remaining 3-E scope is explicitly approved, applied, reviewed and verified.
- 3-E completion decision records unresolved camera/spawn/treasure findings as completed,
  accepted risk or deferred follow-up.
- 3-F has its own approved bounded proposal before implementation.
- Required Unity compilation/tests/manual smoke for changed behavior pass with evidence.
- `docs/project/current-state.md`, architecture and relevant GDD/ADR are synchronized.
- Verified paths are committed, pushed and fast-forward integrated to `origin/main`.
- A milestone completion record is created from the archive template.
