using System;
using DeepSeal.Core;
using DeepSeal.ProceduralGeneration;
using DeepSeal.UnityAdapters.Tilemaps;
using UnityEngine;

namespace DeepSeal.UnityAdapters.Prototype
{
    /// <summary>
    /// 프로토타입 씬에서 MineGridGenerator를 호출해 시드 기반 MineGrid를 만들고, MineGridTileMapRenderer에 넘긴다.
    /// "보이게 하기"용 최소 부트스트랩 역할.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PrototypeMineGridBootstrap : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MineGridTilemapRenderer mineGridRenderer;

        [Header("Generation")]
        [SerializeField] private bool generateOnStart = true;
        [SerializeField] private int seed = 12345;
        [SerializeField] private int width = 24;
        [SerializeField] private int height = 16;
        [SerializeField] private int startX = 12;
        [SerializeField] private int startY = 8;
        [SerializeField] private int startClearRadius = 1;
        [SerializeField] private int wallDurability = 3;
        [Range(0, 100)]
        [SerializeField] private int randomFloorPercent = 35;

        private void Start()
        {
            if(generateOnStart)
            {
                GenerateAndRender();
            }
        }

        [ContextMenu("Generate And Render")]
        public void GenerateAndRender()
        {
            if (mineGridRenderer == null)
            {
                Debug.LogError(
                    "Cannot generate prototype mine grid because Mine Grid Renderer is not assigned.",
                    this);
                return;
            }

            if (!TryCreateSettings(out MineGenerationSettings settings))
            {
                return;
            }

            MineGenerationResult generationResult;

            try
            {
                generationResult = MineGridGenerator.Generate(settings);
            }
            catch (ArgumentException exception)
            {
                Debug.LogError(
                    $"Failed to generate prototype mine grid. {exception.Message}",
                    this);
                return;
            }

            MineGridValidationResult validationResult = MineGridValidator.Validate(generationResult);

            if (!validationResult.IsValid)
            {
                Debug.LogError(
                    $"Generated mine grid is invalid. Issue={validationResult.Issue}, Position={FormatPosition(validationResult)}.",
                    this);
                return;
            }

            mineGridRenderer.Render(generationResult.Grid);
        }

        private bool TryCreateSettings(out MineGenerationSettings settings)
        {
            try
            {
                settings = new MineGenerationSettings(
                    width,
                    height,
                    seed,
                    new GridPosition(startX, startY),
                    startClearRadius,
                    wallDurability,
                    randomFloorPercent);

                return true;
            }
            catch (ArgumentException exception)
            {
                settings = default;

                Debug.LogError(
                    $"Invalid prototype mine generation settings. {exception.Message}",
                    this);

                return false;
            }
        }

        private static string FormatPosition(MineGridValidationResult validationResult)
        {
            return validationResult.HasPosition
                ? validationResult.Position.ToString()
                : "none";
        }

        private void Reset()
        {
            mineGridRenderer = GetComponent<MineGridTilemapRenderer>();

            if (mineGridRenderer == null)
            {
                mineGridRenderer = GetComponentInChildren<MineGridTilemapRenderer>();
            }
        }

        private void OnValidate()
        {
            width = Mathf.Max(3, width);
            height = Mathf.Max(3, height);
            startClearRadius = Mathf.Max(0, startClearRadius);
            wallDurability = Mathf.Max(1, wallDurability);
            randomFloorPercent = Mathf.Clamp(randomFloorPercent, 0, 100);

            int minStartX = 1 + startClearRadius;
            int maxStartX = width - 2 - startClearRadius;
            int minStartY = 1 + startClearRadius;
            int maxStartY = height - 2 - startClearRadius;

            if (minStartX <= maxStartX)
            {
                startX = Mathf.Clamp(startX, minStartX, maxStartX);
            }

            if (minStartY <= maxStartY)
            {
                startY = Mathf.Clamp(startY, minStartY, maxStartY);
            }
        }

    }
}
