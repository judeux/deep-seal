# Architecture Decision Records

ADR은 Deep Seal의 중요한 구조, compatibility, workflow와 장기 방향 결정의 이유를
보존한다. 현재 결정만 확인할 때는 `Accepted`이며 supersede되지 않은 record를 따른다.

## Index

| ADR | Status | Summary |
| --- | --- | --- |
| [ADR-0001](0001-initial-project-direction.md) | Accepted | Initial product and technical direction |
| [ADR-0002](0002-codex-working-rules.md) | Accepted | Initial repository-scoped agent rules and verification scripts |
| [ADR-0003](0003-manual-application-workflow.md) | Accepted | Proposal-only source and owner-applied workflow |
| [ADR-0004](0004-asset-request-specification.md) | Accepted | Structured asset request and licensing workflow |
| [ADR-0005](0005-prototype-domain-and-tilemap-bootstrap.md) | Accepted | Pure domain and Tilemap adapter foundation |
| [ADR-0006](0006-prototype-player-movement.md) | Accepted | Prototype movement as a Unity adapter |
| [ADR-0007](0007-agent-work-unit-and-document-governance.md) | Accepted | Codex/ZCode worktree ownership, document lifecycle and verified Git automation |

## Naming and status

Filename은 `NNNN-short-kebab-case-title.md`를 사용한다. 새 문서는
[`../templates/adr-template.md`](../templates/adr-template.md)를 복사한다.

허용 status:

- `Proposed`
- `Accepted`
- `Rejected`
- `Superseded`

Accepted record가 대체되면 기존 파일을 삭제하지 않고 replacement ADR을 양쪽에
link한다.

ADR-0001부터 ADR-0006은 이전 decision-record directory에서 이동한 legacy record라 기존의
`## Status` footer 형식을 유지한다. ADR-0007 이후의 새 record는 canonical template의
상단 metadata 형식을 사용한다.
