using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Reaper : CardEffect
{

    //SpawnManager spawnManager;
    List<StageEntity> currentActiveNPCS = new List<StageEntity>();
    [SerializeField] float teleportSpeed = 0.15f;
    
    [SerializeField] AnimationClip VFXAnimationClip;

    public override void ActivateCardEffect()
    {

        
        //spawnManager = FindObjectOfType<SpawnManager>();
        currentActiveNPCS = SpawnManager.GetActiveStageEntities();
        int enemyCount = currentActiveNPCS.Count;
        //run if there is enemy
        if (enemyCount>0){
            int targetIndex = Random.Range(0, enemyCount);
            Vector3Int? targetTiles = GetRandomValidTile(GetTilesAdjacentToEnemy(Vector3Int.FloorToInt( currentActiveNPCS[targetIndex].gameObject.transform.position)));
            //Vector3Int? targetTiles = Vector3Int.FloorToInt( currentActiveNPCS[targetIndex].gameObject.transform.position);
            if (targetTiles.HasValue)
                {
                    var xpos = targetTiles.Value.x;
                    var ypos = targetTiles.Value.y;
                    Vector3Int destination = new Vector3Int(xpos, ypos, 0);
                    Vector3Int moveDistance = destination - player.currentTilePosition;
                    StartCoroutine(player.TweenMove(moveDistance.x, moveDistance.y, teleportSpeed, Ease.OutBounce));
                    StartCoroutine(DisableHitboxTimer());
                    CreateObject();
                }
                StartCoroutine(DisableEffectPrefab());

                return;   
        }

    }

    List<Vector3Int> GetTilesAdjacentToEnemy(Vector3Int enemyPosition)
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>();
        foreach(var direction in VectorDirections.Vector3IntAll)
        {
            Vector3Int adj = enemyPosition + direction;
            adjacentTiles.Add(adj);
        }
        return adjacentTiles;
    }

    
    Vector3Int? GetRandomValidTile(List<Vector3Int> adjacentTiles)
    {
        List<Vector3Int> validTiles = new List<Vector3Int>();
        foreach(var tile in adjacentTiles)
        {
            if (stageManager.CheckValidTile(tile))
            {
                validTiles.Add(tile);
            }
        }
        if (validTiles.Count == 0)
        {
            return null; 
        }
        else if (validTiles.Count == 1)
        {
            return validTiles[0]; 
        }
        else
        {
            int randomIndex = Random.Range(0, validTiles.Count);
            return validTiles[randomIndex]; 
        }

    }

    void CreateObject(){
        if(!cardSO.ObjectSummonsArePooled)
        {
            Wheel wheel = Instantiate(cardSO.ObjectSummonList[0], player.currentTilePosition,
            new Quaternion(0, 0, (float)player.aimpoint.currentAimDirection, 0)).GetComponent<Wheel>();
            wheel.objectIsPooled = false;

            AssignVariable(wheel);

        }else
        {
            Wheel wheel = ObjectSummonList[0].GetComponent<Wheel>();
            wheel.objectIsPooled = true;
            AssignVariable(wheel);
        }

    }

        //set position and rotation for object
    protected void AssignVariable(Wheel wheel){
            wheel.transform.position = player.currentTilePosition;
            wheel.attackPayload = attackPayload;            
            wheel.gameObject.SetActive(true);
    }

    IEnumerator DisableHitboxTimer()
    {
        player.playerCollider.enabled = false;
        yield return new WaitForSeconds(teleportSpeed);
        player.playerCollider.enabled = true;        
    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(teleportSpeed + VFXAnimationClip.length);
        gameObject.SetActive(false);
    }
}
