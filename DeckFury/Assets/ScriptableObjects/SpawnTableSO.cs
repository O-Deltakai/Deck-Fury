using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NPCSpawnData
{
    [field:SerializeField] public GameObject NPCPrefab{get; private set;}
    [field:SerializeField] public int SpawnCount{get; private set;}
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


    [field:SerializeField] public List<WaveTable> WaveList {get; private set;}


    public int CalculateDifficultyScore()
    {
        DifficultyScore = 0;

        foreach(var wave in WaveList)
        {
            foreach(var npcSpawn in wave.NPCSpawns)
            {
                if(npcSpawn.NPCPrefab.TryGetComponent<EntityWrapper>(out var entityWrapper))
                {
                    NPC npcComponent = (NPC)entityWrapper.stageEntity;

                    DifficultyScore += npcComponent.spawnPointCost * npcSpawn.SpawnCount;
                }
            }
        }

        return DifficultyScore;

    }



}
