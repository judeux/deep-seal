using System;
using DeepSeal.Core;
using DeepSeal.Mining;
using DeepSeal.ProceduralGeneration;
using DeepSeal.UnityAdapters.Tilemaps;
using UnityEngine;
using UnityEngine.Serialization;

namespace DeepSeal.UnityAdapters.Prototype
{
    /// <summary>
    /// 프로토타입 씬에서 MineGridGenerator를 호출해 시드 기반 MineGrid를 만들고 Tilemap에 표시한다.
    /// 현재 생성 결과를 명시적 Inspector 참조를 통해 다른 프로토타입 컴포넌트가 읽을 수 있게 한다.
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
        [SerializeField] private MineGenerationShapeMode shapeMode = MineGenerationShapeMode.ConnectedCavern;

        [FormerlySerializedAs("randomFloorPercent")]
        [Range(0, 100)]
        [SerializeField] private int targetFloorPercent = 45;
        [Range(0, 100)]
        [SerializeField] private int internalWallPercent = 8;
        [Range(0, 100)]
        [SerializeField] private int internalUnmineableWallPercent = 25;
        [Range(0, MineGenerationSettings.MaxEdgeMineableWallThickness)]
        [SerializeField] private int edgeMineableWallThickness = 1;

        private MineGenerationResult currentResult;
        private bool hasCurrentResult;

        public bool HasCurrentResult => hasCurrentResult;

        public MineGenerationResult CurrentResult => currentResult;

        public MineGrid CurrentGrid => hasCurrentResult ? currentResult.Grid : null;

        private void Start()
        {
            if (generateOnStart && !hasCurrentResult)
            {
                GenerateAndRender();
            }
        }

        [ContextMenu("Generate And Render")]
        public void GenerateAndRender()
        {
            _ = TryGenerateAndRender();
        }

        public bool TryGenerateAndRender()
        {
            ClearCurrentResult();

            if (mineGridRenderer == null)
            {
                Debug.LogError(
                    "Cannot generate prototype mine grid because Mine Grid Renderer is not assigned.",
                    this);
                return false;
            }

            if (!TryCreateSettings(out MineGenerationSettings settings))
            {
                return false;
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
                return false;
            }

            MineGridValidationResult validationResult = MineGridValidator.Validate(generationResult);

            if (!validationResult.IsValid)
            {
                Debug.LogError(
                    $"Generated mine grid is invalid. Issue={validationResult.Issue}, Position={FormatPosition(validationResult)}.",
                    this);
                return false;
            }

            currentResult = generationResult;
            hasCurrentResult = true;

            mineGridRenderer.Render(generationResult.Grid);
            return true;
        }

        public bool TryGetCurrentResult(out MineGenerationResult result)
        {
            result = currentResult;
            return hasCurrentResult;
        }

        public bool TryGetCurrentGrid(out MineGrid grid)
        {
            grid = hasCurrentResult ? currentResult.Grid : null;
            return grid != null;
        }

        public bool TryGetStartPosition(out GridPosition startPosition)
        {
            if (hasCurrentResult)
            {
                startPosition = currentResult.StartPosition;
                return true;
            }

            if (TryCreateSettings(out MineGenerationSettings settings))
            {
                startPosition = settings.StartPosition;
                return true;
            }

            startPosition = default;
            return false;
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
                    targetFloorPercent,
                    shapeMode,
                    internalWallPercent,
                    internalUnmineableWallPercent,
                    edgeMineableWallThickness);

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

        private void ClearCurrentResult()
        {
            currentResult = default;
            hasCurrentResult = false;
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
            targetFloorPercent = Mathf.Clamp(targetFloorPercent, 0, 100);
            internalWallPercent = Mathf.Clamp(internalWallPercent, 0, 100);
            internalUnmineableWallPercent = Mathf.Clamp(internalUnmineableWallPercent, 0, 100);
            edgeMineableWallThickness = Mathf.Clamp(
                edgeMineableWallThickness,
                0,
                MineGenerationSettings.MaxEdgeMineableWallThickness);

            int carveInset = MineGenerationSettings.GetCarveInset(
                shapeMode,
                edgeMineableWallThickness);
            int minStartX = carveInset + startClearRadius;
            int maxStartX = width - 1 - carveInset - startClearRadius;
            int minStartY = carveInset + startClearRadius;
            int maxStartY = height - 1 - carveInset - startClearRadius;

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