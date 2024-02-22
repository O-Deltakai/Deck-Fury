using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct NPCSpawnData
{
    [field:SerializeField] public GameObject NPCPrefab{get; private set;}
    [field:SerializeField, Range(1, 50)] public int SpawnCount{get; set;}
    [field:SerializeField] public SpawnType SpawnZone{get; private set;}

    [Tooltip("SpawnLocation will only be used if SpawnType is set to SpecificPoint, make sure that this value is set to a valid tile on the map " 
    + "you want to spawn the enemy on. Don't set SpawnType to SpecificTile unless you intend to use the SpawnTable for one specific map!")]
    [SerializeField] Vector3Int _specificSpawnLocation;
    public readonly Vector3Int SpawnLocation{get { return _specificSpawnLocation; }}


}

[System.Serializable]
public class WaveTable
{
    [field:SerializeField] public string WaveName{get; private set;}
    [field:SerializeField] public List<NPCSpawnData> NPCSpawns{get; private set;}
}


[CreateAssetMenu(fileName = "Spawn Table", menuName = "New Spawn Table", order = 0)]
public class SpawnTableSO : ScriptableObject
{

    [Tooltip("The difficulty variable for this spawn table which dictates when and where this spawn table will be placed on the stage select " +
    "as well as other elements like randomly generated hazards, the quality of rewards etc.")]
    [Range(0, 10)]
    [SerializeField] int _difficultyTier;
    public int DifficultyTier{get { return _difficultyTier; }}

    public int DifficultyScore{ get; private set; }

    [SerializeField] bool _isElite;
    public bool IsElite => _isElite;

    [field:SerializeField] public List<WaveTable> WaveList {get; private set;}

    /// <summary>
    /// Retrieves all unique NPC spawn data from the wave list.
    /// </summary>
    /// <returns>A list of NPCSpawnData objects containing the aggregated spawn data.</returns>
    public List<NPCSpawnData> GetAllNPCSpawnData()
    {
        List<NPCSpawnData> allNPCSpawnData = new List<NPCSpawnData>();

        Dictionary<GameObject, NPCSpawnData> uniqueNPCSpawnData = new Dictionary<GameObject, NPCSpawnData>();

        foreach (var wave in WaveList)
        {
            foreach (var npcSpawn in wave.NPCSpawns)
            {
                if (uniqueNPCSpawnData.ContainsKey(npcSpawn.NPCPrefab))
                {
                    var tempSpawnData = uniqueNPCSpawnData[npcSpawn.NPCPrefab];
                    tempSpawnData.SpawnCount += npcSpawn.SpawnCount;
                    uniqueNPCSpawnData[npcSpawn.NPCPrefab] = tempSpawnData;
                }
                else
                {
                    uniqueNPCSpawnData.Add(npcSpawn.NPCPrefab, npcSpawn);
                }
            }
        }

        allNPCSpawnData.AddRange(uniqueNPCSpawnData.Values);

        return allNPCSpawnData;
    }

    public int CalculateDifficultyScore()
    {
        DifficultyScore = 0;

        // Placeholder for now - will be replaced with a more sophisticated algorithm
        int maxEnemies = 20;

        foreach(var wave in WaveList)
        {
            int waveScore = 0;

            foreach(var npcSpawn in wave.NPCSpawns)
            {
                if(npcSpawn.NPCPrefab.TryGetComponent<EntityWrapper>(out var entityWrapper))
                {
                    NPC npcComponent = (NPC)entityWrapper.Entity;
                    waveScore += npcComponent.spawnPointCost * npcSpawn.SpawnCount;
                }
            }

            // Normalize the number of enemies in the wave to a range between 0 and 1
            double normalizedEnemies = (double)wave.NPCSpawns.Count / maxEnemies;

            // Apply the exponential function and scale the result to the range between 1 and 3
            double multiplier = 1 + 1 * Math.Pow(normalizedEnemies, 1.5);

            DifficultyScore += (int)(waveScore * multiplier);
        }

        return DifficultyScore;

    }



}
