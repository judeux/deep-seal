# Art source scope

이 파일은 Unity import 전 original, reference, editable source와 generation prompt에
적용된다.

- Source를 final runtime asset으로 간주하지 않는다. Exact imported output path와
  conversion 관계를 asset 문서 또는 register에 기록한다.
- User-provided original은 명시적 편집·교체 요청 없이 덮어쓰지 않는다.
  Derived output은 별도 filename으로 보존한다.
- Prompt는 tool/model이 바뀌어도 재실행 가능한 수준으로 subject, dimensions,
  style, constraints, negative prompt와 acceptance criteria를 포함한다.
- Creator/tool, source relationship, generation/edit process, date, license/commercial use,
  attribution과 modification을 기록한다.
- 허가가 확인되지 않은 copyrighted external reference를 repository에 추가하지 않는다.
- Binary/high-resolution source는 먼저 filename, dimensions, format, size, hash와
  provenance만 확인하고 시각·청각 판단이 필요한 작업에서만 preview한다.
- Git LFS는 승인된 pattern에만 사용하고 일반 PNG/JPG/WAV에 임의로 확대하지 않는다.
