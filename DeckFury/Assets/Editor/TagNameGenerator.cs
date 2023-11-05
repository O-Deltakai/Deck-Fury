using UnityEditor;
using System.IO;
using System.Text;

public class TagNameGenerator
{
    private const string EnumName = "TagNames";
    private static readonly string OutputPath = "Assets/Scripts/Enums/" + EnumName + ".cs";

    [MenuItem("Tools/Generate Tag Names Enum")]
    public static void GenerateTagNamesEnum()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("public enum " + EnumName);
        sb.AppendLine("{");

        foreach (string tag in UnityEditorInternal.InternalEditorUtility.tags)
        {
            sb.AppendLine($"    {tag},");
        }

        sb.AppendLine("}");

        File.WriteAllText(OutputPath, sb.ToString());
        AssetDatabase.Refresh();
    }
}