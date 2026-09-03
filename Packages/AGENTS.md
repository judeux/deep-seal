# Unity package scope

이 파일은 `Packages/manifest.json`, `packages-lock.json`과 package-related configuration에
적용된다.

- Package 추가·삭제·version 변경은 proposal-only이며 affected assembly, transitive
  dependency, lockfile, Unity compatibility, migration과 alternative를 제안하고 승인받는다.
- Unity가 관리하는 manifest/lock 형식을 유지하고 승인된 package와 관련된 entry만
  변경한다.
- Existing Unity/package 기능으로 해결할 수 있는 문제에 중복 framework를 추가하지
  않는다.
- Addressables, localization, networking, Steamworks, DI/async framework는 별도 승인된
  milestone과 decision 없이 추가하지 않는다.
- 변경 후 manifest/lock 일치, Unity resolve, script compilation과 Console을 검증한다.
