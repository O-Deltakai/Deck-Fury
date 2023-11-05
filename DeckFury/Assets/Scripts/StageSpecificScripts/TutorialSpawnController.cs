using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSpawnController : MonoBehaviour
{
    [SerializeField] StageManager stageManager;

    [SerializeField] GameObject trainingRobotPrefab;
    [SerializeField] GameObject explosiveBarrelPrefab;

    [SerializeField] List<Transform> explosiveBarrelSpawnPoints;
    [SerializeField] List<Transform> trainingRobotSpawnPointsWeak;
    [SerializeField] Transform trainingRobotSpawnPointStrong;

    [SerializeField] List<StageEntity> phase3StageEntities;

    // Start is called before the first frame update
    void Start()
    {
        stageManager = StageManager.Instance;
        foreach(StageEntity entity in phase3StageEntities)
        {
            entity.OnDestructionEvent += StartTimerForRespawn;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RespawnPrefab(GameObject prefab, Vector3Int spawnPosition)
    {

        GameObject spawnedPrefab = Instantiate(prefab, spawnPosition, Quaternion.identity);
        StageEntity prefabEntity = spawnedPrefab.GetComponent<EntityWrapper>().stageEntity;
        prefabEntity.OnDestructionEvent += StartTimerForRespawn;

    }

    void StartTimerForRespawn(StageEntity entity, Vector3Int deathPosition)
    {
        StartCoroutine(TimerForRespawn(entity, deathPosition));
    }
    IEnumerator TimerForRespawn(StageEntity entity, Vector3Int deathPosition)
    {
        GameObject prefabToSpawn;


        if(entity is ExplosiveBarrel)
        {   
            prefabToSpawn = explosiveBarrelPrefab;
        }else
        {
            prefabToSpawn = trainingRobotPrefab;
        }

        yield return new WaitForSeconds(3f);

        while(!stageManager.CheckValidTile(deathPosition))
        {
            yield return new WaitForSeconds(1);
        }

        RespawnPrefab(prefabToSpawn, deathPosition);


    }


}
