using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonBeacon : CardEffect
{
    public override void ActivateCardEffect()
    {
        AimpointController aimpoint = player.aimpoint;
 
        Vector3Int position = player.currentTilePosition + aimpoint.GetAimVector3Int();

        if(!stageManager.CheckValidTile(position))
        {
            foreach(Vector3Int direction in VectorDirections.Vector3IntAll)
            {
                if(stageManager.CheckValidTile(position + direction))
                {
                    CreateBeacon(position + direction);
                    return;
                }
            }
        }else
        {
            CreateBeacon(position);
        }

        
    
    }

    void CreateBeacon(Vector3Int position)
    {
        ShieldBeacon beacon = Instantiate(cardSO.ObjectSummonList[0], position, Quaternion.identity).GetComponent<ShieldBeacon>();
        beacon.restorePoint = cardSO.GetBaseDamage();
        beacon.LifeTime = cardSO.QuantifiableEffects[0].FloatQuantity;
        beacon.restoreRate = cardSO.QuantifiableEffects[1].FloatQuantity;
        beacon.maxPoint = cardSO.QuantifiableEffects[2].IntegerQuantity;
        beacon.player = player;
        beacon.objectIsPooled = false;
        beacon.gameObject.SetActive(true);
    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(cardSO.PlayerAnimation.length);
        gameObject.SetActive(false);
    }





}
