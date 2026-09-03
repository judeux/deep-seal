# Deep Seal Documentation Map

이 문서는 repository 문서의 navigation entry다. 각 정보는 한 primary 문서가
소유하며 다른 문서는 상세 내용을 복제하지 않고 link한다.

## Read first

| Need | Primary document |
| --- | --- |
| 현재 구현·검증 상태 | [`project/current-state.md`](project/current-state.md) |
| 현재 milestone과 다음 work unit | [`roadmap/next-milestone.md`](roadmap/next-milestone.md) |
| 게임 규칙과 player experience | [`gdd/GDD_MASTER.md`](gdd/GDD_MASTER.md) 및 개별 GDD |
| Code boundary와 package placement | [`architecture/CODE_STRUCTURE.md`](architecture/CODE_STRUCTURE.md) |
| 구조·workflow 결정의 이유 | [`adr/README.md`](adr/README.md) |
| Local verification | [`testing/LOCAL_VERIFICATION.md`](testing/LOCAL_VERIFICATION.md) |
| Asset pipeline과 요청 형식 | [`art/asset-pipeline.md`](art/asset-pipeline.md) |
| Asset provenance/license | [`licenses/ASSET_REGISTER.md`](licenses/ASSET_REGISTER.md) |

## Directory ownership

- `project/`: 현재 사실을 짧게 보여주는 volatile snapshot
- `roadmap/`: 하나의 active milestone과 completed milestone archive
- `gdd/`: 승인 상태가 표시된 design source of truth
- `architecture/`: code responsibility와 dependency
- `adr/`: 중요한 결정과 rationale의 append-oriented record
- `implementation/`: 기존 prototype의 상세 plan/review 자료와 역사적 step index
- `testing/`: verification procedure와 environment
- `art/`: asset specification, generation, import와 replacement pipeline
- `licenses/`: external/AI asset provenance와 commercial-use record
- `templates/`: 새 work unit, ADR와 milestone archive의 canonical template

## Lifecycle

1. `next-milestone.md`에서 work unit을 제안하고 사용자 승인을 받는다.
2. 필요한 구조적 결정은 구현 전에 ADR로 기록한다.
3. Owner agent가 승인된 문서를 관리하고 proposal-only 변경은 사용자가 적용한다.
4. 적용 결과와 verification evidence를 owner가 검토한다.
5. `current-state.md`를 실제 증거에 맞춰 갱신한다.
6. Milestone gate를 모두 통과하면 archive를 만들고 다음 milestone을 연다.

Status와 증거 규칙은 [`AGENTS.md`](AGENTS.md)를 따른다.
