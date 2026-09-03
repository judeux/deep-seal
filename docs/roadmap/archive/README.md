# Milestone Archive

현재 실행 계획은 [`../next-milestone.md`](../next-milestone.md)에만 둔다. 완료 milestone은
이 directory에 evidence-backed completion record로 보존한다.

## Archive rule

Milestone의 모든 completion gate가 적용, 검토, 검증되고 Git integration까지 끝난 후:

1. `docs/roadmap/archive/<milestone-id>-<short-name>.md`를 만든다.
2. 아래 index에 completion date, verification과 archive link를 추가한다.
3. `next-milestone.md`를 다음 승인 상태가 정확한 milestone으로 교체한다.
4. `current-state.md`의 active work와 main baseline을 갱신한다.
5. 구현 의도와 실제 evidence를 구분한다.

새 archive는 [`../../templates/milestone-archive-template.md`](../../templates/milestone-archive-template.md)를
사용한다.

## Completed milestones

새 lifecycle로 archive한 milestone은 아직 없다. 기존 완료 step의 상세 이력은
[`../../implementation/PROTOTYPE_ROADMAP.md`](../../implementation/PROTOTYPE_ROADMAP.md)에
보존되어 있으며, 검증 근거를 재확인하지 않고 소급 archive를 만들지 않는다.

## Required evidence

- Completion date, integrated commit과 exact Unity version
- Automated command/Editor route, pass/fail count와 result/log path
- Manual smoke procedure와 observed result
- Implemented and excluded scope
- Accepted warning, blocker와 untested area
- Decision/ADR link와 follow-up milestone
