using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerPrefsManager))]
public class PlayerPrefsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerPrefsManager yourComponent = (PlayerPrefsManager)target;
        if (GUILayout.Button("Clear PlayerPrefs"))
        {
            PlayerPrefsManager.ClearPlayerPrefs();
        }
    }
}