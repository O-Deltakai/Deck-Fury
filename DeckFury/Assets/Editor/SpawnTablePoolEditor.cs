using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnTablePoolSO))]
public class SpawnTablePoolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Calculate and display the difficulty score
        SpawnTablePoolSO spawnTablePool = (SpawnTablePoolSO)target;
        if (GUILayout.Button("Calculate Spawn Table Difficulty Scores"))
        {
            Debug.Log("Calculated difficulty score");

            for (int i = 0; i < spawnTablePool.RegularSpawnTables.Count; i++)
            {
                spawnTablePool.RegularSpawnTables[i].CalculateDifficultyScore();
            }

            for (int i = 0; i < spawnTablePool.EliteSpawnTables.Count; i++)
            {
                spawnTablePool.EliteSpawnTables[i].CalculateDifficultyScore();
            }

            for (int i = 0; i < spawnTablePool.BossSpawnTables.Count; i++)
            {
                spawnTablePool.BossSpawnTables[i].CalculateDifficultyScore();
            }
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Regular Spawn Table Difficulty Scores", EditorStyles.boldLabel);
        for (int i = 0; i < spawnTablePool.RegularSpawnTables.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(spawnTablePool.RegularSpawnTables[i].name + ":");
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(spawnTablePool.RegularSpawnTables[i].DifficultyScore.ToString());
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Elite Spawn Table Difficulty Scores", EditorStyles.boldLabel);
        for (int i = 0; i < spawnTablePool.EliteSpawnTables.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(spawnTablePool.EliteSpawnTables[i].name + ":");
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(spawnTablePool.EliteSpawnTables[i].DifficultyScore.ToString());
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Boss Spawn Table Difficulty Scores", EditorStyles.boldLabel);
        for (int i = 0; i < spawnTablePool.BossSpawnTables.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(spawnTablePool.BossSpawnTables[i].name + ":");
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(spawnTablePool.BossSpawnTables[i].DifficultyScore.ToString());
            EditorGUILayout.EndHorizontal();
        }
    }
}
