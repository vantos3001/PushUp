using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Infrastructure.Editor
{
    public static class AndroidBuildScript
    {
        private const string BuildPath = "Builds/PushUpArena.apk";

        [MenuItem("Tools/PushUp/Build Android APK")]
        public static void Build()
        {
            Directory.CreateDirectory("Builds");

            var options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Game.unity" },
                locationPathName = BuildPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);

            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Android build done: {BuildPath} ({report.summary.totalSize / (1024 * 1024)} MB)");
                EditorUtility.RevealInFinder(BuildPath);
                return;
            }

            Debug.LogError($"Android build failed: {report.summary.result}");
        }
    }
}
