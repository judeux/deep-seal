using UnityEngine;

namespace DeepSeal.UnityAdapters.Tilemaps
{
    /// <summary>
    /// 프로토타입 바이옴 이름을 타일맵 색조로 바꾸는 어댑터 데이터.
    /// 바이옴 전용 타일셋이 도입되기 전까지 임시 표현으로 사용한다.
    /// </summary>
    public static class PrototypeBiomeTintLibrary
    {
        public static bool TryGetTint(string biomeName, out Color tint)
        {
            switch (biomeName)
            {
                case "rubble-cavern":
                    tint = Color.white;
                    return true;
                case "dense-rock":
                    tint = new Color(0.82f, 0.74f, 0.62f, 1f);
                    return true;
                case "hollow-cavern":
                    tint = new Color(0.70f, 0.82f, 1f, 1f);
                    return true;
                case "vein-field":
                    tint = new Color(1f, 0.86f, 0.58f, 1f);
                    return true;
                default:
                    tint = Color.white;
                    return false;
            }
        }
    }
}
