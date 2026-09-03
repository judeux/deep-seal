# Repository tool scope

이 파일은 repository-local PowerShell script, validator와 build/test wrapper에 적용된다.
Root의 proposal-only source-code 권한을 유지한다.

- Global tool을 가정하지 않고 repository-local wrapper를 우선한다.
- Validator는 기본 read-only, deterministic, actionable error, nonzero failure exit와
  명시적 target list를 사용한다.
- Broad recursive scan이 필요하지 않으면 current work-unit path만 검사한다.
- Script 변경은 parser/syntax, 최소 fixture, 필요한 경우 full scan 순서로 검증한다.
- Host-specific absolute path, user profile, secret 또는 generated cache를 고정 contract에
  넣지 않는다.
- PowerShell은 repository가 지원하는 Windows/PowerShell 환경에서 parse되어야 하며
  불필요한 optional module을 전체 script의 parser dependency로 만들지 않는다.
- Script가 생성하는 log/result/build는 documented ignored path에 두고 success/failure를
  exit code로 반환한다.
