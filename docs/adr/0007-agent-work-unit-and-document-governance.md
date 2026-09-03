# ADR-0007: Agent Work-Unit and Document Governance

- Status: Accepted
- Date: 2026-09-03
- Decision owner: Project owner
- Scope: Codex/ZCode collaboration, execution authority, documentation lifecycle and Git integration

## Context

Deep Seal은 Codex와 ZCode를 함께 사용하지만 source와 Unity content는 프로젝트
소유자가 직접 적용하는 workflow를 유지한다. 두 agent가 같은 checkout이나 branch를
동시에 변경하면 unrelated change가 섞이고 documentation status, staging과 ownership이
불분명해질 수 있다.

기존 repository 지침은 prototype bootstrap 단계의 규칙, 현재 상태와 세부 asset
요구사항을 root에 함께 보관해 context가 커졌으며, 현재 구현 상태를 한곳에서 확인할
문서와 milestone archive lifecycle도 없었다.

## Decision

### Work-unit ownership

- 하나의 work unit에는 Codex 또는 ZCode 중 정확히 한 owner agent를 지정한다.
- Owner만 해당 work unit의 write와 Git 작업을 수행한다. 다른 agent는 read-only
  review만 수행한다.
- Codex와 ZCode는 별도 worktree와 owner prefix branch를 사용한다.
- 대상 path가 겹치는 work unit은 병렬 실행하지 않는다.

### Execution authority

- Source, tests, scenes, prefabs, imported assets, packages와 ProjectSettings는 사용자가
  해당 task에서 direct edit을 승인하지 않는 한 proposal-only다.
- 승인된 work unit의 repository 문서는 owner agent가 직접 관리한다.
- 게임 규칙, concept, architecture contract, compatibility, milestone scope와 중대한
  방향 변경은 문서 수정 전 사용자 승인이 필요하다.

### Documentation lifecycle

- `docs/project/current-state.md`가 현재 구현·검증 snapshot을 소유한다.
- `docs/roadmap/next-milestone.md`가 하나의 active milestone과 work-unit sequence를
  소유한다.
- 완료한 milestone은 `docs/roadmap/archive/`에 evidence와 함께 보존한다.
- 구조와 workflow 결정은 `docs/adr/`에 append-oriented record로 남긴다.
- Existing detailed prototype plan은 `docs/implementation/`에 역사적 evidence로
  유지하되 current status를 중복 소유하지 않는다.

### Verified Git automation

- Owner는 verified work unit의 명시적 path만 stage, review, commit과 push한다.
- 사용자 verification 보고 뒤에는 별도 재승인 없이 feature branch를 local `main`에
  fast-forward 통합하고 `origin/main`을 push할 수 있다.
- Divergence, conflict, unexpected change 또는 verification failure가 있으면 중단한다.
- Auto rebase, stash, conflict resolution, force push, history rewrite와 branch deletion으로
  우회하지 않는다.

## Rationale

Worktree와 single-owner 경계는 두 agent를 동시에 사용하면서도 파일 소유권과 Git
history를 추적 가능하게 만든다. Root에는 공통 불변 규칙만 두고 nearest scoped
`AGENTS.md`에서 세부 책임을 제공하면 불필요한 context를 줄이면서 경로별 안전 규칙을
강화할 수 있다.

Current-state, active roadmap, archive와 ADR을 분리하면 계획, 구현, 검증과 완료 주장을
서로 혼동하지 않고 자동 문서 관리의 갱신 시점을 결정할 수 있다.

## Consequences

- Work unit마다 owner, branch/worktree, scope와 verification gate를 먼저 확인해야 한다.
- Agent 전환 시 새 owner는 `origin/main` 기준의 새 work unit에서 시작한다.
- Document-only work도 link, status, diff와 Git 검증을 통과해야 한다.
- ADR-0002와 ADR-0003의 initial policy는 유지되지만, agent isolation, documentation
  authority와 Git automation의 더 구체적인 규칙은 이 ADR과 현재 scoped
  `AGENTS.md`가 소유한다.
- W0-B에서 문서 구조를 만들고 W0-C에서 실제 project snapshot을 채운다.

## Verification and follow-up

- Scoped instruction path와 precedence를 정적 검토한다.
- 모든 `Assets/` 아래 `AGENTS.md`에 `.meta`가 있고 GUID가 중복되지 않는지 확인한다.
- Documentation link, status, staged path와 `git diff --check`를 검증한다.
- W0-C 후 ZCode에 새 workflow와 current project state를 전달한다.
