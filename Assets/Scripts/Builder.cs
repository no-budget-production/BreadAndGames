using UnityEditor;
using UnityEngine;
using System.Diagnostics;

public class Builder
{
    [MenuItem("Builds/Win 64bits")]
    public static void BuildGame()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] levels = new string[] { "Assets/_Scenes/main.unity" };

        // Build player.
        UnityEditor.PlayerSettings.runInBackground = false;
        BuildPipeline.BuildPlayer(levels, path + "/BreadAndGames.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);

        // Copy a file from the project folder to the build folder, alongside the built game.
        FileUtil.CopyFileOrDirectory("Assets/Text/Readme.txt", path + "Readme.txt");

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = path + "BreadAndGames.exe";
        //proc.Start();
    }
}