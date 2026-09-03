using System.Collections.Generic;

namespace DeepSeal.ProceduralGeneration
{
    /// <summary>
    /// 프로토타입 단계에서 코드로 작성된 바이옴 목록을 제공한다.
    /// 3-B-2에서 씬 표현(타일 틴트, HUD 표시)이 붙기 전까지 데이터 소스 역할만 한다.
    /// </summary>
    public static class MineBiomeLibrary
    {
        public static IReadOnlyList<MineBiome> CreatePrototypeBiomes()
        {
                        return new List<MineBiome>
            {
                // 기본 암반 동굴. 3-E 스케일 실험으로 폭/높이를 약 두 배로 늘렸다.
                new MineBiome(
                    "rubble-cavern",
                    minWidth: 56, maxWidth: 72,
                    minHeight: 40, maxHeight: 48,
                    startClearRadius: 1,
                    wallDurability: 3,
                    targetFloorPercent: 45,
                    internalWallPercent: 10,
                    internalUnmineableWallPercent: 25,
                    edgeMineableWallThickness: 1,
                    presetPlacementCount: 4,
                    presetPlacementAttempts: 120),
                // 치밀한 암층. 내부 벽과 채굴 불가 벽이 많아 채굴 빌드의 가치가 커진다.
                new MineBiome(
                    "dense-rock",
                    minWidth: 60, maxWidth: 76,
                    minHeight: 40, maxHeight: 48,
                    startClearRadius: 1,
                    wallDurability: 3,
                    targetFloorPercent: 40,
                    internalWallPercent: 18,
                    internalUnmineableWallPercent: 40,
                    edgeMineableWallThickness: 2,
                    presetPlacementCount: 2,
                    presetPlacementAttempts: 120),
                // 공동 굴. 장애물이 적고 트인 공간이 넓어 원거리·범위 공격이 유리하다.
                new MineBiome(
                    "hollow-cavern",
                    minWidth: 52, maxWidth: 68,
                    minHeight: 36, maxHeight: 44,
                    startClearRadius: 1,
                    wallDurability: 3,
                    targetFloorPercent: 40,
                    internalWallPercent: 4,
                    internalUnmineableWallPercent: 10,
                    edgeMineableWallThickness: 1,
                    presetPlacementCount: 0,
                    presetPlacementAttempts: 0),
                // 광맥 지대. 프리셋 구조물이 많이 배치된다.
                new MineBiome(
                    "vein-field",
                    minWidth: 56, maxWidth: 72,
                    minHeight: 40, maxHeight: 48,
                    startClearRadius: 1,
                    wallDurability: 3,
                    targetFloorPercent: 45,
                    internalWallPercent: 8,
                    internalUnmineableWallPercent: 20,
                    edgeMineableWallThickness: 1,
                    presetPlacementCount: 8,
                    presetPlacementAttempts: 200)
            };
        }
    }
}
