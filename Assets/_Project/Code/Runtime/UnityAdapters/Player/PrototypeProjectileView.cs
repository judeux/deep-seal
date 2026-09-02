using System;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Player
{
    /// <summary>
    /// Prototype-only traveling visual for the projectile attack pattern.
    /// ProjectileAttackRules already decided the flight at fire time; this view reenacts it
    /// and reports arrival so damage stays grid-deterministic.
    /// Uses a code-generated white square sprite as an explicit temporary placeholder.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypeProjectileView : MonoBehaviour
    {
        [SerializeField] private float moveSpeedCellsPerSecond = 12f;
        [SerializeField] private Color tint = new Color(0.35f, 0.85f, 1f, 1f);

        private static Texture2D sharedPlaceholderTexture;

        private Vector3 targetWorldPosition;
        private Action<PrototypeProjectileView> arrivedCallback;

        public static PrototypeProjectileView Create(Vector3 originWorldPosition, Transform parent)
        {
            var projectileObject = new GameObject("PrototypeProjectile");
            projectileObject.transform.SetParent(parent, false);
            projectileObject.transform.position = originWorldPosition;

            var view = projectileObject.AddComponent<PrototypeProjectileView>();

            var spriteRenderer = projectileObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateSharedPlaceholderSprite();
            spriteRenderer.color = view.tint;
            spriteRenderer.sortingOrder = 10;

            return view;
        }

        public void Begin(Vector3 targetWorldPosition, Action<PrototypeProjectileView> arrivedCallback)
        {
            this.targetWorldPosition = targetWorldPosition;
            this.arrivedCallback = arrivedCallback;
        }

        /// <summary>
        /// 발사체 색상을 변경한다. 플레이어 발사체와 적 발사체를 구분하는 표현용이다.
        /// </summary>
        public void SetTint(Color color)
        {
            tint = color;

            if (TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                spriteRenderer.color = color;
            }
        }

        private void Update()
        {
            float maxStep = moveSpeedCellsPerSecond * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, maxStep);

            if ((transform.position - targetWorldPosition).sqrMagnitude > 0.0001f)
            {
                return;
            }

            Action<PrototypeProjectileView> callback = arrivedCallback;
            arrivedCallback = null;
            callback?.Invoke(this);
            Destroy(gameObject);
        }

        private static Sprite CreateSharedPlaceholderSprite()
        {
            if (sharedPlaceholderTexture == null)
            {
                sharedPlaceholderTexture = new Texture2D(4, 4, TextureFormat.RGBA32, false)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };

                Color32 white = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                Color32[] pixels = new Color32[16];

                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = white;
                }

                sharedPlaceholderTexture.SetPixels32(pixels);
                sharedPlaceholderTexture.Apply(false, true);
            }

            return Sprite.Create(
                sharedPlaceholderTexture,
                new Rect(0f, 0f, 4f, 4f),
                new Vector2(0.5f, 0.5f),
                4f);
        }
    }
}
