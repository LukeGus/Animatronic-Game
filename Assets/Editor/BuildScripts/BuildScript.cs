using UnityEditor;
using System.Collections.Generic;

public class BuildScript
{
    static string[] GetEnabledScenes()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        List<string> enabledScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in scenes)
        {
            if (scene.enabled)
            {
                enabledScenes.Add(scene.path);
            }
        }
        return enabledScenes.ToArray();
    }

    [MenuItem("Build/Build/Windows 64-bit")]
    static void BuildWindows64()
    {
        string projectName = "AnimatronicGame"; // Set the name of your project here
        string outputPath = $"Builds/Windows64/{projectName}64Windows.exe";
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        
        buildPlayerOptions.scenes = GetEnabledScenes();
        buildPlayerOptions.locationPathName = outputPath;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Player;

        EditorUserBuildSettings.development = false;

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}
