# Roadmap scope

이 파일은 active milestone과 completed milestone archive에 적용된다.

- `next-milestone.md`는 정확히 하나의 active milestone만 소유한다.
- Work unit마다 owner, status, goal, affected area, explicit exclusion, dependency,
  approval, verification gate와 completion condition을 기록한다.
- Current step 밖 기능을 조용히 추가하거나 다음 step을 앞당기지 않는다.
- User approval 전에는 `Approved`, 적용 evidence 전에는 `Implemented`, 필수 verification과
  owner review 전에는 `Verified`로 표시하지 않는다.
- Milestone archive는 모든 required gate와 Git integration이 완료된 뒤 만든다.
- Archive는 이후 새 계획에 맞춰 내용을 바꾸지 않는다. 오류 정정은 명시적 correction
  note 또는 후속 ADR로 남긴다.
- 상세 구현 history를 next milestone에 계속 누적하지 않고 completed work는 current
  state 또는 archive로 이동한다.
- Owner agent가 current state, ADR와 roadmap status의 일치를 검토한다.
