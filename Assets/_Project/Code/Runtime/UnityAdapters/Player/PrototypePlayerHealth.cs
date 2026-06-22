using System;
using DeepSeal.Combat;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Player
{
    /// <summary>
    /// Prototype-only player health adapter.
    /// Uses pure Combat health rules and handles temporary defeat behavior in the scene.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypePlayerHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private int maxHitPoints = 10;
        [SerializeField] private bool resetOnAwake = true;

        [Header("Runtime Debug")]
        [SerializeField] private int currentHitPoints;

        [Header("Defeat")]
        [SerializeField] private bool reenableOnReset = true;
        [SerializeField] private bool tintOnDefeat = true;
        [SerializeField] private Color aliveTint = Color.white;
        [SerializeField] private Color defeatedTint = new Color(0.4f, 0.4f, 0.4f, 1f);
        [SerializeField] private SpriteRenderer[] tintRenderers;
        [SerializeField] private Behaviour[] disableOnDefeat;

        [Header("Debug")]
        [SerializeField] private bool logHealthChanges;

        private HitPointState hitPoints;
        private bool defeatHandled;

        public int MaxHitPoints => hitPoints.IsInitialized ? hitPoints.MaxHitPoints : maxHitPoints;

        public int CurrentHitPoints => hitPoints.IsInitialized ? hitPoints.CurrentHitPoints : currentHitPoints;

        public bool IsDefeated => hitPoints.IsInitialized && hitPoints.IsDefeated;

        public HitPointState CurrentState => hitPoints;

        private void Awake()
        {
            if (resetOnAwake)
            {
                ResetHealthToFull();
                return;
            }

            InitializeFromInspectorValues();
        }

        private void Start()
        {
            EnsureInitialized();
            ApplyVisualState();
        }

        public bool TryApplyDamage(int damage)
        {
            EnsureInitialized();

            DamageResult result;

            try
            {
                result = HealthRules.ApplyDamage(hitPoints, damage);
            }
            catch (ArgumentException exception)
            {
                Debug.LogError(
                    $"Prototype player damage failed because damage settings are invalid. {exception.Message}",
                    this);
                return false;
            }

            hitPoints = result.Current;
            currentHitPoints = hitPoints.CurrentHitPoints;

            ApplyVisualState();

            if (logHealthChanges)
            {
                Debug.Log(
                    $"Player health changed: Damage={result.Damage}, Previous={result.Previous}, Current={result.Current}, DefeatedThisHit={result.DefeatedThisHit}.",
                    this);
            }

            if (result.DefeatedThisHit)
            {
                HandleDefeat();
            }

            return result.Changed;
        }

        [ContextMenu("Reset Health To Full")]
        public void ResetHealthToFull()
        {
            hitPoints = HitPointState.Full(maxHitPoints);
            currentHitPoints = hitPoints.CurrentHitPoints;
            defeatHandled = false;

            if (reenableOnReset)
            {
                SetDefeatBehavioursEnabled(true);
            }

            ApplyVisualState();
        }

        private void InitializeFromInspectorValues()
        {
            int safeMaxHitPoints = Mathf.Max(1, maxHitPoints);
            int safeCurrentHitPoints = Mathf.Clamp(currentHitPoints, 0, safeMaxHitPoints);

            hitPoints = new HitPointState(safeMaxHitPoints, safeCurrentHitPoints);
            currentHitPoints = hitPoints.CurrentHitPoints;
            defeatHandled = hitPoints.IsDefeated;

            ApplyVisualState();
        }

        private void EnsureInitialized()
        {
            if (hitPoints.IsInitialized)
            {
                return;
            }

            if (currentHitPoints > 0)
            {
                InitializeFromInspectorValues();
                return;
            }

            ResetHealthToFull();
        }

        private void HandleDefeat()
        {
            if (defeatHandled)
            {
                return;
            }

            defeatHandled = true;

            if (logHealthChanges)
            {
                Debug.Log("Prototype player defeated.", this);
            }

            SetDefeatBehavioursEnabled(false);
        }

        private void SetDefeatBehavioursEnabled(bool enabledValue)
        {
            if (disableOnDefeat == null)
            {
                return;
            }

            for (int i = 0; i < disableOnDefeat.Length; i++)
            {
                Behaviour behaviour = disableOnDefeat[i];

                if (behaviour == null || behaviour == this)
                {
                    continue;
                }

                behaviour.enabled = enabledValue;
            }
        }

        private void ApplyVisualState()
        {
            if (!tintOnDefeat)
            {
                return;
            }

            ApplyTint(IsDefeated ? defeatedTint : aliveTint);
        }

        private void ApplyTint(Color tint)
        {
            if (tintRenderers == null)
            {
                return;
            }

            for (int i = 0; i < tintRenderers.Length; i++)
            {
                SpriteRenderer spriteRenderer = tintRenderers[i];

                if (spriteRenderer != null)
                {
                    spriteRenderer.color = tint;
                }
            }
        }

        private void Reset()
        {
            maxHitPoints = 10;
            currentHitPoints = maxHitPoints;
            resetOnAwake = true;
            reenableOnReset = true;
            tintOnDefeat = true;
            aliveTint = Color.white;
            defeatedTint = new Color(0.4f, 0.4f, 0.4f, 1f);
            tintRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        private void OnValidate()
        {
            maxHitPoints = Mathf.Max(1, maxHitPoints);
            currentHitPoints = Mathf.Clamp(currentHitPoints, 0, maxHitPoints);
        }
    }
}