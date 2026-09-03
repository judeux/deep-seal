# Documentation scope

이 파일은 `docs/` 아래 repository 문서에 적용된다.

## Default execution mode: owner-agent managed

사용자가 proposal-only 또는 read-only review를 명시하지 않으면 현재 승인된 work
unit의 owner agent가 필요한 문서를 직접 생성, 수정, 이동 및 정리한다.

- 현재 source of truth와 user change를 먼저 확인하고 unrelated change를 보존한다.
- 문서 변경 후 diff, link, status, 날짜와 구현·검증 주장의 근거를 검토한다.
- 문서 권한으로 source/test code, Unity asset, package 또는 project setting을 변경하지
  않는다.
- 게임 규칙, concept, architecture contract, compatibility, milestone scope 또는 중대한
  방향 변경은 사용자 승인 전에 문서에 확정 사항으로 반영하지 않는다.
- 승인 전 아이디어는 `Proposed` 또는 `DRAFT`로 명확히 표시한다.

## Documentation sources of truth

- `docs/gdd/`: 승인된 game rule, player experience와 design scope
- `docs/architecture/`: code boundary, dependency와 implemented structure
- `docs/project/current-state.md`: 현재 구현·검증·active work snapshot
- `docs/roadmap/next-milestone.md`: 현재 milestone, work-unit 순서와 completion gate
- `docs/roadmap/archive/`: 완료된 milestone의 불변 completion record
- `docs/implementation/PROTOTYPE_ROADMAP.md`: 기존 prototype step의 역사적 상세 색인
- `docs/adr/`: 구조·workflow·호환성 결정과 rationale
- `docs/testing/`: local verification procedure와 evidence expectation
- `docs/licenses/ASSET_REGISTER.md`: external/AI asset provenance와 license

## Status vocabulary and evidence

Work unit에는 필요한 경우 다음 상태만 사용한다.

- `Proposed`: 방향 제안, 승인 전
- `Awaiting Approval`: 사용자 결정 필요
- `Approved`: 범위와 계약 승인, 구현 전
- `In Progress`: 승인 범위에서 작업 중
- `Applied — Awaiting Verification`: 사용자 적용 완료, 증거 대기
- `Review Required`: 적용 또는 검증 문제 확인
- `Verified`: 필수 검증과 owner review 통과
- `Completed`: 문서와 Git 통합까지 완료
- `Deferred`: 현재 순서에서 연기
- `Blocked`: 외부 결정이나 환경 없이는 진행 불가
- `Superseded`: 더 최신 결정이나 계획으로 대체

`Designed`, `Implemented`, `Verified`, `Completed`와 archive는 서로 다른 주장이다.
Code, asset, test와 verification evidence를 직접 확인한 범위만 기록한다. Application
실행을 compile/test 통과와 같은 증거로 취급하지 않는다.

## Required maintenance events

- 사용자 승인: 범위, 제외, owner와 verification gate 기록
- 작업 시작: active work unit, branch/worktree와 대상 path 기록
- Proposal 전달: 적용 파일, 핵심 contract, 적용 순서와 예상 결과 기록
- 사용자 적용 보고: 실제 diff를 확인한 뒤 implemented 범위와 차이 기록
- Verification 보고: 날짜, Unity version, 수준, command/Editor route, pass/fail 수,
  log path, 미검증 범위와 blocker 기록
- Review 완료: confirmed problem, accepted risk와 follow-up 기록
- Work unit 완료: 관련 roadmap/architecture/decision/license 문서를 동기화
- Milestone 완료: 완료 범위, 제외, verification evidence, commit과 다음 milestone을
  archive에 기록

이미 정확하게 기록된 history를 여러 문서에 복제하지 않는다. 상세 source-of-truth
문서로 link하고 현재 상태만 요약한다.

## GDD and design changes

- `LOCKED`: 사용자 승인을 받은 현재 기준
- `DRAFT`: 유용하지만 미확정
- `DEFERRED`: 현재 milestone 밖으로 연기
- `OUT`: 의도적으로 제외

큰 design idea를 조용히 삭제하지 않는다. 연기 또는 제외 이유와 영향을 기록한다.
GDD의 LOCKED rule과 code가 충돌하면 어느 쪽도 임의로 고치지 않고 사용자 결정을
요청한다.

## Decision records

`docs/adr/NNNN-short-kebab-case-title.md` 형식을 사용한다.

Decision record에는 최소한 다음을 포함한다.

```markdown
# ADR-NNNN: Title

Date: YYYY-MM-DD
Status: Proposed | Accepted | Superseded

## Context
## Decision
## Rationale
## Consequences
## Verification or follow-up
```

구조, dependency, compatibility, workflow authority, persistent identity 또는 승인된
design 방향을 바꾸는 결정에 사용한다. 이미 accepted인 record를 새 결론에 맞게
소급 수정하지 말고 새 record로 supersede한다.

## Roadmap maintenance

`docs/roadmap/next-milestone.md`는 work unit 완료, 분할, 병합, 연기 또는 재범위화 시
owner agent가 갱신한다. `docs/project/current-state.md`는 실제 구현·검증 snapshot이
바뀔 때 함께 갱신한다.

- Current step, completed behavior, explicit exclusion, verification evidence와 next step을
  일치시킨다.
- 사용자 verification과 owner review 전에는 `Done`으로 표시하지 않는다.
- Current step 밖 기능을 조용히 끼워 넣지 않는다.
- Roadmap 순서를 바꿀 필요가 있으면 이유와 대안을 먼저 제안하고 승인받는다.
- Milestone completion gate를 모두 통과하면 `docs/roadmap/archive/`에 completion
  record를 만들고 그 다음에만 `next-milestone.md`를 다음 계획으로 교체한다.

## Documentation quality

- UTF-8과 repository의 Markdown style을 유지한다.
- Link는 repository-relative path를 사용하고 rename/move 후 target을 검증한다.
- 표의 상태, 날짜, file path와 count가 본문과 일치해야 한다.
- 긴 command output을 복사하기보다 재현 command, 핵심 결과와 log path를 기록한다.
- 관련 heading과 link target만 먼저 읽고 충돌이나 전체 context가 필요할 때 확장한다.
