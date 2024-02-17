using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRespawner : MonoBehaviour
{
    [Serializable]
    public class EnemyRespawnData
    {
        public GameObject enemyPrefab;
        public Vector3Int? position;
        public Transform transform;
    }

    [SerializeField] List<EnemyRespawnData> enemySpawns = new();

    [SerializeField] float respawnTimer = 2f;

    [SerializeField] GameObject spawnPositionParent;

    Coroutine respawnCoroutine;

    void Start()
    {
        SpawnEnemies();
        foreach (var tr in spawnPositionParent.GetComponentsInChildren<Transform>())
        {
            tr.gameObject.SetActive(false);
        }
    }

    void SpawnEnemies()
    {
        foreach (var enemyRespawnData in enemySpawns)
        {
            SelfRespawningEnemy(enemyRespawnData);
        }
    }    

    /// <summary>
    /// Spawns a self-respawning enemy which will respawn at a set location after a set amount of time after it is killed.
    /// </summary>
    /// <param name="enemyRespawnData"></param>
    public void SelfRespawningEnemy(EnemyRespawnData enemyRespawnData)
    {
        Vector3Int spawnPosition = Vector3Int.FloorToInt(enemyRespawnData.transform.position);
        StageEntity entity = SpawnManager.Instance.TrySpawnNPCPrefab(enemyRespawnData.enemyPrefab, spawnPosition, out bool success);
        if(!success)
        {
            if(respawnCoroutine != null)
            {
                StopCoroutine(respawnCoroutine);
            }
            respawnCoroutine = StartCoroutine(RespawnTimer(respawnTimer * 0.5f, enemyRespawnData));
        }else
        {
            entity.OnDestructionEvent += (StageEntity destroyedEntity, Vector3Int deathPosition) =>
            {
                if(respawnCoroutine != null)
                {
                    StopCoroutine(respawnCoroutine);
                }
                respawnCoroutine = StartCoroutine(RespawnTimer(respawnTimer, enemyRespawnData));
            };
            
        }
    }

    IEnumerator RespawnTimer(float duration, EnemyRespawnData enemyRespawnData)
    {
        yield return new WaitForSeconds(duration);
        SelfRespawningEnemy(enemyRespawnData);
        respawnCoroutine = null;
    }

}
