using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace DeepSeal.EditorBuild
{
    public static class BuildPlayer
    {
        private const string BuildPathArgument = "-deepSealBuildPath";
        private const string DefaultBuildPath = "Builds/Windows/DeepSeal.exe";
        private const string SceneSearchRoot = "Assets/_Project/Scenes";

        public static void PerformWindowsDevelopmentBuild()
        {
            string buildPath = GetCommandLineValue(BuildPathArgument, DefaultBuildPath);
            string fullBuildPath = Path.GetFullPath(buildPath);
            string? buildDirectory = Path.GetDirectoryName(fullBuildPath);

            if (string.IsNullOrWhiteSpace(buildDirectory))
            {
                throw new InvalidOperationException($"Invalid build path: {fullBuildPath}");
            }

            Directory.CreateDirectory(buildDirectory);

            string[] scenes = GetBuildScenes();

            if (scenes.Length == 0)
            {
                throw new InvalidOperationException(
                    $"No scenes found. Add a scene to Build Settings or place a scene under '{SceneSearchRoot}'.");
            }

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = fullBuildPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development | BuildOptions.AllowDebugging
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Build failed. Result={summary.result}, Errors={summary.totalErrors}, Warnings={summary.totalWarnings}");
            }

            UnityEngine.Debug.Log(
                $"Build succeeded: {summary.outputPath}, Size={summary.totalSize} bytes, Warnings={summary.totalWarnings}");
        }

        private static string[] GetBuildScenes()
        {
            string[] enabledBuildSettingsScenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .ToArray();

            if (enabledBuildSettingsScenes.Length > 0)
            {
                return enabledBuildSettingsScenes;
            }

            if (!AssetDatabase.IsValidFolder(SceneSearchRoot))
            {
                return Array.Empty<string>();
            }

            return AssetDatabase.FindAssets("t:Scene", new[] { SceneSearchRoot })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string GetCommandLineValue(string key, string fallback)
        {
            string[] args = Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], key, StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1];
                }
            }

            return fallback;
        }
    }
}
