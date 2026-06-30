# 11. Scope, MVP, and Roadmap

## 1. 개발 원칙

1인 개발에서는 콘텐츠 수보다 핵심 루프 검증을 우선한다.

검증 순서:

1. 자동 전투 중 채굴이 재미있는가
2. 채굴한 지형이 전투 판단을 바꾸는가
3. 준비와 상성이 체감 난도를 바꾸는가
4. 후퇴와 손실이 캠페인 결정을 만드는가
5. 여러 원정의 누적이 최종 목표로 이어지는가

## 2. 1차 플레이어블 프로토타입

### 포함

- 광부 1명
- 직업 1개
- 지층 1개
- 자동 무기 4개
- 채굴 연계 강화 4개
- 일반 적 4종
- 엘리트 1종
- 보스 1종
- 15분 원정
- 채굴, 성장, 보물, 탈출

### 제외

- 원정대 관리
- 부상과 실종
- 봉인석 캠페인
- 복잡한 거점
- 다수 서브 던전

### 성공 기준

- 채굴이 이동 지연이 아니라 적극적 선택으로 느껴짐
- 같은 전투도 지형을 다르게 파면 결과가 달라짐
- 15분 세션을 반복할 동기가 있음

## 3. 2차 캠페인 프로토타입

### 포함

- 광부 4명
- 직업 2개
- 특성 10종
- 부상 6종
- 거점 시설 3개
- 일반·구조·토벌 원정
- 네임드 2종
- 서브 던전 2종
- 봉인석 1개

### 성공 기준

- 광부 선택과 장비 준비가 원정 결과에 영향
- 실종과 구조가 의미 있는 다음 행동을 만듦
- 같은 지역 반복이 단순 반복으로 느껴지지 않음

## 4. 버티컬 슬라이스

### 포함 후보

- 지층 2개
- 직업 3개
- 광부 보유 8명
- 일반 적 8~10종
- 네임드 4종
- 보스 2종
- 서브 던전 4종
- 봉인석 2개
- 보물 30개
- 약 2시간의 축약 캠페인
- 임시 최종 원정

## 5. Early Access 후보 범위 — DEFERRED

- 지역 4개 이상
- 직업 5개 내외
- 봉인석 5개 이상
- 최종 원정 완성
- 반복 캠페인 변형
- Steam 업적과 클라우드 세이브

## 6. 명시적 범위 제외

- 멀티플레이
- 완전 3D 복셀 지형
- 복잡한 유체 시뮬레이션
- 대규모 거점 건설
- 수십 종류 제작 재료
- 완전 절차적 보스 패턴 생성

## 7. 기능 도입 게이트

새 기능은 다음 조건을 만족할 때만 본 제작에 포함한다.

- 핵심 루프를 직접 강화
- 기존 시스템 재사용 가능
- 전용 아트 요구량이 감당 가능
- 자동 테스트 또는 데이터 검증 가능
- 제거해도 저장 데이터가 치명적으로 깨지지 않는 구조

## 8. 현재 프로토타입 구현 상태

현재 구현 완료:

- Core grid primitives
  - 정수 그리드 좌표.
  - 4방향 그리드 방향.
- Mining domain
  - 바닥/벽 셀 구분.
  - 벽 내구도.
  - 안전한 MineGrid 셀 접근.
  - 채굴 피해 누적과 벽 파괴 후 바닥 전환.
- Procedural generation
  - seed 기반 테스트용 광산 맵 생성.
  - 외곽 벽 유지.
  - 시작 위치 주변 통과 가능 영역 보장.
  - 생성 결과 검증.
  - Seed-based mine generation now supports irregular visible mine silhouettes using non-rendered outside-footprint cells.
  - The main passable mine area remains connected.
  - Connected mine layouts can include mineable internal wall obstacles that preserve overall floor connectivity.
  - Hand-authored prototype terrain presets can be blended into generated maps.
  - Preset placement is seed-stable and preserves the connected main passable area.
  - Presets currently use pure C# pattern data, not Unity Prefabs or Tilemap chunks.
- Terrain wall semantics
  - 채굴 가능한 벽.
  - 채굴 불가능한 내부 벽.
  - 광산 footprint 경계 벽.
  - 맵 바깥을 나타내는 Void 셀.
- Unity Tilemap prototype display
  - MineGrid 데이터를 Unity Tilemap에 표시.
  - 프로토타입 씬에서 seed/settings 기반 광산 맵 확인 가능.
- Prototype player movement
  - WASD/방향키 기반 임시 이동.
  - MineGrid의 통과 가능 셀 기준으로 이동 가능 여부 판정.
  - 벽 셀과 맵 밖으로 이동 불가.
- Prototype mining input
  - 현재 방향 기준 인접 셀 채굴.
  - 런타임 MineGrid 변경.
  - 채굴 후 Tilemap refresh.
- Prototype enemy and combat foundation
  - 순수 C# 적 상태와 MineGrid 기반 이동 규칙.
  - Unity 적 스폰/표시/이동 어댑터.
  - 플레이어 주변 적 자동 타겟팅.
  - 임시 적 체력과 처치/제거 흐름.
- Prototype player damage and health loop
  - 플레이어 임시 체력.
  - 적 접촉 피해.
  - 체력 0 도달 시 프로토타입 패배 처리.
- Prototype treasure pickup
  - 프로토타입 보물 표시.
  - 플레이어 GridPosition 기준 보물 획득.
  - 획득 개수와 총 가치 추적.
- Prototype extraction marker
  - 보물 획득 후 도달할 수 있는 프로토타입 귀환 마커.
  - 플레이어 GridPosition 기준 탈출 완료 판정.
  - 완료 후 프로토타입 입력/전투 컴포넌트 정지.
  - Treasure and extraction marker placement can fall back to valid reachable floor cells on irregular generated maps.
- Prototype loop feedback and tuning
  - 체력, 보물, 탈출 상태, 루프 결과, 벽 내구도 피드백을 기준으로 1차 튜닝 진행.
  - 현재 튜닝 값은 다음 프로토타입 단계의 임시 기준선으로 사용.
  - 튜닝 결과, 적 이동/스폰/보상 피드백이 다음 병목으로 확인됨.
- Prototype enemy navigation and spawn pressure
  - 적이 단순 직선 이동에서 막힐 때 통과 가능한 경로를 찾는 그리드 기반 경로 탐색 추가.
  - 적별 이동 간격과 체력 차이를 통해 임시적인 움직임 다양성 추가.
  - 시간 간격과 최소 활성 적 수 기준의 런타임 적 스폰 추가.
  - Enemy spawn tests explicitly cover void and non-passable terrain semantics.
- Prototype reward drops
  - 적 처치와 일부 채굴 행동에서 프로토타입 보상 드랍 생성.
  - 플레이어가 일정 거리 안으로 접근하면 자동 획득.
  - 현재는 획득 개수와 가치만 추적하며, 정식 인벤토리나 캠페인 정산은 없음.
  - Reward drop spawn validation uses the same passable terrain semantics as expedition object placement.
- Prototype upgrade selection
  - reward drop value를 사용한 임시 업그레이드 선택 추가.
  - 자동공격 피해, 자동공격 사거리, 채굴 속도, 이동 속도에 직접 연결.
  - 현재는 런 중 임시 효과이며, 영구 성장이나 저장 데이터는 없음.

아직 미구현:

- 정식 업그레이드 트리.
- 영구 성장과 캠페인 업그레이드.
- 저장 데이터와 보상 정산.
- Wall durability/material variants.
- Wall material variants and final terrain art.
- Asset-authored terrain presets, vault chunks, or room authoring tools.
- Minimap or exploration map UI after procedural generation, terrain semantics, and discovery rules are more stable.
- Final spawn tables, biome-specific spawn weighting, authored spawn zones, and campaign reward settlement.


다음 구현 후보:

1. Generation spawn rule review pass
   - Review treasure, extraction, enemy, and reward placement against irregular generated maps with terrain presets.
   - Move fragile spawn assumptions into pure domain placement rules if needed.

2. Asset-authored preset or vault authoring
   - DEFERRED until code-authored preset placement proves useful.

3. Minimap or exploration map UI
   - DEFERRED until procedural generation, terrain semantics, preset placement, and discovery rules are more stable.
