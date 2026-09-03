# AGENTS.md — Deep Seal Repository Rules

이 파일은 repository 전체에 항상 적용하는 최소 공통 규칙을 정의한다.
세부 규칙은 작업 경로에서 가장 가까운 scoped `AGENTS.md`를 추가로 따른다.

## Project identity

- Product: `Deep Seal` (working title)
- Unity: `6000.3.17f1` (Unity 6.3 LTS), URP 2D
- Target: Windows PC / Steam
- Mode: single-player, offline-first
- Root C# namespace: `DeepSeal`
- Structure: 2D tile-based destructive mine exploration
- Initial product boundary: expedition survival roguelite with automatic combat,
  mining, procedural maps, treasure and extraction

Multiplayer, a custom backend and physically simulated flowing water or lava are
outside the approved initial scope.

## Instruction precedence

1. 사용자의 현재 명시적 요청
2. 작업 경로에서 가장 가까운 scoped `AGENTS.md`
3. 이 repository-level `AGENTS.md`
4. 현재 roadmap과 승인된 GDD, architecture, decision record
5. 구현 증거인 code, tests, serialized asset과 verification result

출처가 충돌하면 임의로 선택하지 않는다. 충돌한 내용, 확인된 사실, 필요한
결정을 사용자에게 보고한다. 확인할 수 없는 repository 사실은 만들어내지 않고
가정 또는 검증 필요 항목으로 표시한다.

## Language and communication

- 사용자가 다른 언어를 요청하지 않으면 설명, 제안, 검토와 새 문서 본문은
  한국어를 기본으로 한다.
- 기존 영어 문서는 이 규칙만을 이유로 일괄 번역하지 않는다.
- C# identifier, namespace, API, file/path, command, setting key와 원본 log/error는
  정확한 원문 표기를 유지한다.
- 확정된 문제, 가능성이 높은 위험, runtime 검증 필요 항목을 구분한다.
- 작업 종료 보고에는 변경 이유, 변경 파일, 검증 수준, 남은 위험과 다음 조건을
  포함한다.

## Work-unit ownership and isolation

- 하나의 work unit에는 Codex 또는 ZCode 중 정확히 한 owner agent만 둔다.
- Owner만 해당 work unit의 파일 수정과 Git write를 수행한다. 다른 agent는
  read-only review를 할 수 있다.
- Codex와 ZCode는 별도 worktree와 별도 branch를 사용한다. 같은 branch 또는
  worktree를 동시에 수정하지 않는다.
- 기본 branch prefix는 owner에 따라 `codex/` 또는 `zcode/`를 사용한다.
- Work unit 시작 전에 owner, branch, worktree, 대상 path, 목표, 제외 범위와
  verification gate를 확인한다.
- 대상 path가 겹치는 work unit은 병렬 실행하지 않는다.
- 사용자 또는 다른 agent의 unrelated change를 이동, 정리, stage 또는 덮어쓰지
  않는다.

## Execution authority

### Source and test code: proposal only

사용자가 해당 작업에서 직접 수정을 명시적으로 승인하지 않으면 source와 test
code는 read-only다.

- 새 파일은 exact path와 전체 내용으로 제안한다.
- 기존 파일은 적용 위치가 분명한 unified diff 또는 bounded replacement로 제안한다.
- Import, namespace, constructor argument, serialized field, assembly reference, test와
  관련 문서를 생략하지 않는다.
- `...`, `existing code`, `relevant method`, `add somewhere` 같은 불명확한
  placeholder를 사용하지 않는다.
- 제안은 사용자가 적용하고 실제 compile/test 증거가 나오기 전까지 implemented,
  passed 또는 verified로 표시하지 않는다.
- 사용자가 적용한 뒤 현재 파일과 diff를 다시 읽고 proposal 일치 여부를 검토한다.
- 기계적 오타 교정도 사용자의 direct-edit 승인이 없으면 proposal-only다.

### Unity project content: proposal only

Scene, prefab, imported asset, ScriptableObject instance, animation, material,
Input Action, package와 `ProjectSettings/` 변경은 기본적으로 사용자 주도다.

- Exact hierarchy, component/property, value, asset path, reference와 Editor 적용 순서를
  제공한다.
- Unity YAML을 직접 수정하거나 Unity를 자동으로 열어 reserialize하지 않는다.
- `.meta`는 asset과 함께 보존하고 GUID를 임의로 재생성하지 않는다.
- 예외적으로 직접 변경하려면 해당 작업에서 사용자의 명시적 승인을 먼저 받는다.

### Documentation: agent-managed within approved scope

현재 승인된 work unit에 필요한 repository 문서는 owner agent가 직접 생성, 수정,
이동 및 정리한다.

- 문서 권한은 code, test, Unity asset, package 또는 project setting 권한으로 확장되지
  않는다.
- 게임 규칙, concept, architecture contract, compatibility, milestone scope 또는 중대한
  방향을 바꾸는 문서 수정은 적용 전에 사용자 승인을 받는다.
- 증거 없이 `Implemented`, `Verified`, `Completed` 또는 archived 상태를 기록하지
  않는다.
- 세부 규칙은 `docs/AGENTS.md`를 따른다.

## Work-unit workflow

1. 현재 roadmap, 관련 문서, code, test와 asset을 최소 범위로 확인한다.
2. 목표, owner, 변경 파일, 변경 범위, 명시적 제외와 verification gate를 제안한다.
3. Design, architecture, compatibility 또는 scope 결정은 사용자 승인을 받는다.
4. Owner가 승인된 문서를 관리하고 branch/worktree를 준비한다. Proposal-only 대상은
   사용자가 적용할 수 있도록 정확한 내용을 제공한다.
5. 사용자가 적용 결과와 verification 결과를 보고하면 owner가 실제 diff, code,
   Console, test log와 asset reference를 검토한다.
6. 문제는 수정 제안 후 다시 적용·검증한다. 모든 필수 증거가 충족된 경우에만
   work unit을 verified로 판정한다.
7. Owner가 필수 문서를 동기화하고 검토된 path만 stage, commit, push한다.
8. 사용자 verification 보고 뒤에는 별도 재승인 없이 current feature branch를 local
   `main`에 fast-forward 통합하고 `origin/main`을 push할 수 있다.

여러 미검증 work unit을 하나의 큰 구현이나 commit으로 합치지 않는다.

## Current source of truth

- Game rule과 player experience: `docs/gdd/`
- Current milestone and work-unit sequence: `docs/roadmap/next-milestone.md`
- Project implementation snapshot: `docs/project/current-state.md`
- Historical prototype step index: `docs/implementation/PROTOTYPE_ROADMAP.md`
- Architecture boundary와 current structure: `docs/architecture/CODE_STRUCTURE.md`
- Structural and workflow decisions: `docs/adr/`
- Verification procedure: `docs/testing/LOCAL_VERIFICATION.md`
- Asset provenance and licensing: `docs/licenses/ASSET_REGISTER.md`
- Implemented behavior: current code, tests, Unity assets와 verification evidence

## Architecture invariants

| Area | Responsibility | Dependency rule |
| --- | --- | --- |
| `DeepSeal.Core` | Grid primitive와 넓게 공유되는 pure value | Unity에 의존하지 않음 |
| `DeepSeal.Mining` | Terrain, mine grid와 mining rule | Unity에 의존하지 않음 |
| `DeepSeal.ProceduralGeneration` | Seeded map generation과 validation | Pure domain에만 의존 |
| `DeepSeal.Combat` | Combat, enemy, attack와 health rule | Pure domain에만 의존 |
| `DeepSeal.Expedition` | Treasure, reward, extraction와 expedition placement rule | Pure domain에만 의존 |
| `DeepSeal.Upgrades` | Prototype upgrade definition과 purchase rule | Pure domain에만 의존 |
| `DeepSeal.UnityAdapters.*` | Scene, lifecycle, rendering, input과 Unity integration | Domain을 사용할 수 있으나 역방향 금지 |
| `DeepSeal.EditorBuild` / `DeepSeal.EditorTools` | Build와 editor tooling | Runtime에서 reference 금지 |

- `UnityEngine`, scene, prefab, `MonoBehaviour`, `ScriptableObject`와 Unity lifecycle을
  pure domain에 유입하지 않는다.
- Reusable gameplay rule을 MonoBehaviour가 다시 계산하지 않는다.
- Circular dependency, global mutable singleton, broad service locator와 global event bus를
  승인 없이 추가하지 않는다.
- 새 class는 책임 owner와 기존 folder/namespace를 먼저 확인해 배치한다.
  `Manager`, `Helper`, `Misc`, `Common`, `Util`을 dumping area로 만들지 않는다.
- Persistent/content identity가 필요해지면 display text, enum ordinal, list index 또는
  asset filename에 의존하지 않는 explicit stable ID를 사용한다.
- Stable ID, serialized schema, assembly boundary, package dependency, Input Action contract
  또는 authored data schema 변경은 compatibility 영향과 migration을 설명하고 승인받는다.

## Context and inspection efficiency

- 탐색은 `rg`, `rg --files`, targeted `git status`와 `git diff`로 시작한다.
- 현재 work unit에 필요한 가장 가까운 `AGENTS.md`와 관련 문서 section만 우선 읽는다.
- 파일이 바뀌었거나 상태가 불명확할 때만 이전 검증을 반복한다.
- 성공한 read-only 검사와 긴 log는 결과만 요약하고 문제와 필요한 증거만 자세히
  출력한다.
- Unchanged `.meta`, generated project file, large Unity YAML, font/atlas payload와 binary를
  전문으로 읽지 않는다. GUID, importer, target object/property, size, format, hash 등
  필요한 field부터 확인한다.
- Repository work에는 sub-agent를 자동으로 사용하지 않는다. 사용자가 명시적으로
  요청한 경우에도 bounded read-only subtask에만 사용하고 write, final review와 Git은
  work-unit owner가 유지한다.

## Scoped instruction routing

| Target | Additional instruction |
| --- | --- |
| Repository documents | `docs/AGENTS.md` |
| Asset specification or generation prompt | `docs/art/AGENTS.md` |
| Source/reference art | `ArtSource/AGENTS.md` |
| Unity project content | `Assets/_Project/AGENTS.md` and nearest scoped file |
| Source and tests | `Assets/_Project/Code/AGENTS.md` and nearest scoped file |
| Imported art | `Assets/_Project/Art/AGENTS.md` |
| Imported audio | `Assets/_Project/Audio/AGENTS.md` |
| Prefabs | `Assets/_Project/Prefabs/AGENTS.md` |
| Scenes | `Assets/_Project/Scenes/AGENTS.md` |
| Repository scripts | `tools/AGENTS.md` |
| Package manifest and lock | `Packages/AGENTS.md` |
| Unity project settings | `ProjectSettings/AGENTS.md` |

여러 경로를 함께 다루면 각 경로의 scoped 지침을 모두 적용한다.

## Git safety and verified automation

- 시작 전에 branch, worktree, remote, local/remote base와 staged/unstaged diff를
  확인한다.
- Owner branch의 verified work unit만 의도한 explicit path로 stage한다.
- `git diff --cached`, `git diff --cached --check`와 staged path를 commit 전에 검토한다.
- Commit message는 변경 의도, behavior와 risk scope를 설명한다.
- Integration 순서는 `git fetch origin` -> local `main`의
  `git pull --ff-only origin main` -> `git merge --ff-only <feature-branch>` ->
  `git push origin main`이다.
- Remote divergence, non-fast-forward, conflict, branch protection/auth failure, unexpected
  worktree change 또는 verification failure가 있으면 즉시 중단하고 보고한다.
- Auto rebase, stash, conflict resolution, force push, history rewrite와 branch deletion으로
  우회하지 않는다.
- `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`, `Build/`, `Builds/`,
  `TestResults/`, `CoverageResults/`를 commit하지 않는다.
- Unity asset과 `.meta`는 함께 commit하고 text-serialized Unity asset은 Git LFS에 넣지
  않는다.
- 완료 후 local/remote tip과 clean worktree를 확인한다. Remote feature branch는 사용자
  요청 없이 삭제하지 않는다.

## Repository prohibitions

현재 milestone에서 명시적으로 승인하기 전에는 다음을 추가하지 않는다.

- Multiplayer, networking package, custom server, database 또는 authentication
- Current roadmap 밖 gameplay와 campaign-scale system
- DOTS/ECS, third-party DI/async framework 또는 broad pooling infrastructure
- Steamworks, analytics/live-ops SDK 또는 mobile SDK
- General asset loading framework, Addressables 또는 localization system
- Production-complete로 표시한 prototype placeholder

현재 milestone을 만족하는 가장 작고 유지보수 가능한 구조를 선택한다.
