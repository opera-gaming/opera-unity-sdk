using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Opera
{
    public sealed class UnityBuilder : IBuilder
    {
        public bool Build(string buildDirectory)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();
            buildPlayerOptions.locationPathName = buildDirectory;
            buildPlayerOptions.target = BuildTarget.WebGL;
            buildPlayerOptions.options = BuildOptions.None;

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                return true;
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
            return false;
        }
    }
}
