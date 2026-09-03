# ADR scope

이 파일은 `docs/adr/`의 architecture decision record에 적용된다.

- Architecture, compatibility, workflow authority, persistent identity, package/schema와
  중대한 design 방향은 사용자 승인 전 `Accepted`로 기록하지 않는다.
- 승인 전 record는 `Proposed`, 기각한 record는 `Rejected`, 대체된 record는
  `Superseded`로 표시한다.
- 새 번호는 현재 최고 번호 다음 값을 사용하고 삭제된 번호를 재사용하지 않는다.
- Accepted ADR의 결론을 새 현실에 맞게 소급 편집하지 않는다. 새 ADR을 만들고
  `Superseded by ADR-NNNN` 관계를 기록한다.
- Context, considered options, decision, rationale, consequences, compatibility/migration,
  verification/follow-up을 실제 scope에 맞게 작성한다.
- 단순 구현 세부, 일시적 TODO, numeric tuning과 작은 refactor는 ADR로 만들지 않는다.
- 관련 GDD, architecture, roadmap와 code path를 link하고 중복 설명을 최소화한다.
- Owner agent가 filename, index, link와 status를 함께 갱신한다.
