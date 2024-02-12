using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSentry : CardEffect
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
                    CreateSentryGun(spawnTilePosition + direction);
                    return;
                }
            }
        }else
        {
            CreateSentryGun(spawnTilePosition);
        }

        
    
    }

    void CreateSentryGun(Vector3Int position)
    {
        SentryGun sentryGun = Instantiate(cardSO.ObjectSummonList[0], position, Quaternion.identity).GetComponent<SentryGun>();
        sentryGun.sentryDamage = cardSO.GetBaseDamage();
        sentryGun.sentryLifeTime = cardSO.QuantifiableEffects[0].FloatQuantity;
        sentryGun.sentryFireRate = cardSO.QuantifiableEffects[1].FloatQuantity;
    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(cardSO.PlayerAnimation.length);
        gameObject.SetActive(false);
    }





}
