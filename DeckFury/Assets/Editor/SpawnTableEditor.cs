using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(SpawnTableSO))]
public class SpawnTableEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Calculate and display the difficulty score
        SpawnTableSO spawnTable = (SpawnTableSO)target;
        if (GUILayout.Button("Calculate Difficulty Score"))
        {
            Debug.Log("Calculated difficulty score");
            spawnTable.CalculateDifficultyScore();
        }

        EditorGUILayout.LabelField("Difficulty Score:", spawnTable.DifficultyScore.ToString());

    }

}
