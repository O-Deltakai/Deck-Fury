using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeStageController : MonoBehaviour
{
    [SerializeField] GameObject trainingNPC;

    StageManager stageManager;
    SpawnManager spawnManager;


    [SerializeField] List<NPC> trainingNPCs = new List<NPC>();
    [SerializeField] List<Vector3Int> respawnPositions = new List<Vector3Int>();



    void Start()
    {

        stageManager = StageManager.Instance;
        spawnManager = SpawnManager.Instance;

        SubscribeNPCs();

    }

    void SubscribeNPCs()
    {
        foreach (var npc in trainingNPCs)
        {
            SubscribeNPCToRespawn(npc);
        }
    }

    void SubscribeNPCToRespawn(NPC npc)
    {
        npc.OnDestroyed -= CleanTrainingNPCs;
        npc.OnDestroyed -= RespawnNPC;

        npc.OnDestroyed += CleanTrainingNPCs;
        npc.OnDestroyed += RespawnNPC;
    }

    void CleanTrainingNPCs()
    {
        List<NPC> destroyedNPCs = new List<NPC>();
        foreach (var npc in trainingNPCs)
        {
            if(npc == null)
            {
                destroyedNPCs.Add(npc);
            }
        }

        for (int i = 0; i < destroyedNPCs.Count; i++)
        {
            trainingNPCs.Remove(destroyedNPCs[i]);
        }


    }


    void RespawnNPC()
    {
        StartCoroutine(RespawnNPCTimer());
    }

    IEnumerator RespawnNPCTimer()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        bool foundCondition = false;
        Vector3Int respawnPosition = new Vector3Int();


        while(!foundCondition)
        {
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
            
            respawnPosition = FindValidRespawnPosition(out foundCondition);
            if(foundCondition)
            {
                NPC spawnedNPC = (NPC)spawnManager.SpawnNPCPrefab(trainingNPC, respawnPosition);

                trainingNPCs.Add(spawnedNPC);
                SubscribeNPCToRespawn(spawnedNPC);
            }

            yield return new WaitForSeconds(1);
        }    



    }

    Vector3Int FindValidRespawnPosition(out bool condition)
    {
        foreach(Vector3Int position in respawnPositions)
        {
            if(stageManager.CheckValidTile(position))
            {
                print("postion valid at: " + position);
                condition = true;
                return position;
            }
        }

        condition = false;
        return Vector3Int.zero;

    }



}
