# Unity ProjectSettings scope

이 파일은 `ProjectSettings/` 변경에 적용된다.

- ProjectSettings 변경은 proposal-only이며 build, input, rendering, serialization,
  import pipeline과 compatibility 영향 및 alternative를 설명하고 승인받는다.
- Exact Unity version `6000.3.17f1`의 Editor UI와 serialized format을 기준으로 한다.
- Visible Meta Files, Force Text, URP 2D와 Windows x86_64 기준을 승인 없이 바꾸지 않는다.
- 요구사항 없이 broad reserialization, platform switch, quality/render pipeline 또는
  package backend setting을 변경하지 않는다.
- Proposal은 가능한 경우 Unity Editor menu/path, exact field와 expected before/after를
  제공한다. YAML direct edit는 작고 형식이 명확한 승인된 예외에서만 검토한다.
- 변경 후 target diff, Unity restart/resolve, script compilation, Console과 affected
  editor/runtime smoke를 risk에 맞게 검증한다.
