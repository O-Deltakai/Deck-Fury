using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezingMine : CardEffect
{
    public override void ActivateCardEffect()
    {
        AimpointController aimpoint = player.aimpoint;
 
        Vector3Int spawnTilePosition = player.currentTilePosition + aimpoint.GetAimVector3Int();

        if(!stageManager.CheckValidTile(spawnTilePosition))
        {
            foreach(Vector3Int direction in VectorDirections.Vector3IntAll)
            {
                if(stageManager.CheckValidTile(spawnTilePosition + direction))
                {
                    CreateMine(spawnTilePosition + direction);
                    return;
                }
            }
        }else
        {
            CreateMine(spawnTilePosition);
        }

        
    
    }

    void CreateMine(Vector3Int position)
    {
        FreezeMine mine = Instantiate(cardSO.ObjectSummonList[0], position, Quaternion.identity).GetComponent<FreezeMine>();
        mine.attackPayload = attackPayload;
        mine.gameObject.SetActive(true);
    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(cardSO.PlayerAnimation.length);
        gameObject.SetActive(false);
    }





}
