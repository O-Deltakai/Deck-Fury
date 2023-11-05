using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class SceneNameGenerator
{

    private const string EnumName = "SceneNames";
    private static readonly string OutputPath = "Assets/Scripts/Enums/" + EnumName + ".cs";

    [MenuItem("Tools/Generate Scene Names Enum")]
    public static void GenerateSceneNames()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("public enum " + EnumName);
        sb.AppendLine("{");

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                //string enumValue = sceneName.Replace(" ", "_"); // Replace spaces with underscores
                sb.AppendLine($"    {sceneName},");
            }
        }

        sb.AppendLine("}");

        File.WriteAllText(OutputPath, sb.ToString());
        AssetDatabase.Refresh();
    }    


}
