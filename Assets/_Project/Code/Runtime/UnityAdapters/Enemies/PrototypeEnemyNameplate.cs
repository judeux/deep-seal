using UnityEngine;

namespace DeepSeal.UnityAdapters.Enemies
{
    /// <summary>
    /// 명명 엘리트 머리 위에 떠 있는 임시 네임플레이트.
    /// 실제 UI 스택이 도입되기 전까지 빌트인 폰트 TextMesh로 이름을 보여준다.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypeEnemyNameplate : MonoBehaviour
    {
        private const float BobHeightCells = 0.12f;
        private const float BobSpeedCellsPerSecond = 2.2f;

        private Vector3 baseLocalPosition;

        private void Awake()
        {
            baseLocalPosition = transform.localPosition;
        }

        private void Update()
        {
            float bobOffset = Mathf.Sin(Time.time * BobSpeedCellsPerSecond) * BobHeightCells;
            transform.localPosition = baseLocalPosition + new Vector3(0f, bobOffset, 0f);
        }

        public static PrototypeEnemyNameplate Create(Transform parent, string displayName)
        {
            var nameplateObject = new GameObject("PrototypeEliteNameplate");
            nameplateObject.transform.SetParent(parent, false);
            nameplateObject.transform.localPosition = new Vector3(0f, 0.85f, 0f);

            var textMesh = nameplateObject.AddComponent<TextMesh>();
            textMesh.text = displayName;
            textMesh.fontSize = 32;
            textMesh.characterSize = 0.2f;
            textMesh.anchor = TextAnchor.LowerCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.color = new Color(1f, 0.8f, 0.25f, 1f);

            var meshRenderer = nameplateObject.GetComponent<MeshRenderer>();
            meshRenderer.sortingOrder = 15;

            return nameplateObject.AddComponent<PrototypeEnemyNameplate>();
        }
    }
}
