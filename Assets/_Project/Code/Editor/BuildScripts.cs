using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace BB.Editor
{
    /// <summary>
    /// CLI build entry points, invoked by scripts/build.sh:
    ///   Unity -batchmode -quit -projectPath . -executeMethod BB.Editor.BuildScripts.BuildLinuxServer
    /// Scene list comes from EditorBuildSettings (configured in-editor).
    /// </summary>
    public static class BuildScripts
    {
        public static void BuildLinuxClient() =>
            Build(BuildTarget.StandaloneLinux64, StandaloneBuildSubtarget.Player,
                  "Builds/LinuxClient/BackyardBattle.x86_64");

        public static void BuildWindowsClient() =>
            Build(BuildTarget.StandaloneWindows64, StandaloneBuildSubtarget.Player,
                  "Builds/WindowsClient/BackyardBattle.exe");

        public static void BuildLinuxServer() =>
            Build(BuildTarget.StandaloneLinux64, StandaloneBuildSubtarget.Server,
                  "Builds/LinuxServer/BackyardBattle.x86_64");

        static void Build(BuildTarget target, StandaloneBuildSubtarget subtarget, string outputPath)
        {
            string[] scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();
            if (scenes.Length == 0)
            {
                Debug.LogError("No scenes enabled in EditorBuildSettings — open the project and add Boot/MainMenu/Lobby/stages.");
                EditorApplication.Exit(1);
                return;
            }

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = target,
                subtarget = (int)subtarget,
                options = BuildOptions.None,
            };
            EditorUserBuildSettings.standaloneBuildSubtarget = subtarget;

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.LogError($"Build failed: {report.summary.result} ({report.summary.totalErrors} errors)");
                EditorApplication.Exit(1);
                return;
            }
            Debug.Log($"Build OK → {outputPath} ({report.summary.totalSize / (1024 * 1024)} MB)");
        }
    }
}
