using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace DeepSeal.EditorTools
{
    /// <summary>
    /// Editor menu that runs the EditMode test suite inside the open editor
    /// and logs a summary to the console, so iteration does not require
    /// closing the editor for the batch-mode tool scripts.
    /// </summary>
    [InitializeOnLoad]
    public static class InEditorEditModeRunner
    {
        private const string MenuPath = "DeepSeal/Verification/Run EditMode Tests (In Editor)";

        static InEditorEditModeRunner()
        {
            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(new ResultLogger());
        }

        [MenuItem(MenuPath)]
        public static void RunEditModeTests()
        {
            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.Execute(new ExecutionSettings(new Filter
            {
                testMode = TestMode.EditMode
            }));
        }

        private sealed class ResultLogger : ICallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun)
            {
            }

            public void TestStarted(ITestAdaptor test)
            {
            }

            public void TestFinished(ITestResultAdaptor result)
            {
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                if (result.TestStatus == TestStatus.Passed)
                {
                    Debug.Log($"[EditMode] Passed. Total={result.PassCount + result.FailCount + result.SkipCount}");
                    return;
                }

                Debug.LogError($"[EditMode] Result={result.TestStatus} Pass={result.PassCount} Fail={result.FailCount} Skip={result.SkipCount}");
            }
        }
    }
}