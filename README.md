# Deep Seal

Deep Seal은 자동 전투, 셀 단위 채굴과 절차 생성 지형을 결합한 single-player
expedition-management survival roguelite prototype이다.

## Project status

- Unity: `6000.3.17f1` (Unity 6.3 LTS), URP 2D
- Primary target: Windows PC / Steam
- Current implementation and verification: [`docs/project/current-state.md`](docs/project/current-state.md)
- Current milestone and next work unit: [`docs/roadmap/next-milestone.md`](docs/roadmap/next-milestone.md)
- Documentation map: [`docs/README.md`](docs/README.md)

현재 기능 목록을 이 README에 중복해서 유지하지 않는다. 위 current-state 문서가
code, tests와 verification evidence에 맞춰 갱신되는 canonical snapshot이다.

## Development workflow

- Repository 공통 규칙: [`AGENTS.md`](AGENTS.md)
- Source/test와 Unity content는 task-level direct-edit 승인이 없으면 proposal-only다.
- 승인된 work unit의 문서는 owner agent가 직접 관리한다.
- Codex와 ZCode는 별도 worktree/branch를 사용하고 work unit마다 한 owner만 둔다.
- Local verification: [`docs/testing/LOCAL_VERIFICATION.md`](docs/testing/LOCAL_VERIFICATION.md)

Generated Unity directories such as `Library`, `Temp`, `Logs`, `TestResults` and `Builds`
are local-only and must not be committed.
