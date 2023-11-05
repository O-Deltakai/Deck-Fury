using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Teleport : CardEffect
{

    [SerializeField] float teleportSpeed = 0.15f;

    public override void ActivateCardEffect()
    {
        AimpointController aimpoint = player.aimpoint;

        int x = aimpoint.GetAimVector3Int().x * (int)cardSO.QuantifiableEffects[0].GetValueDynamic();
        int y = aimpoint.GetAimVector3Int().y * (int)cardSO.QuantifiableEffects[0].GetValueDynamic();

        Vector3Int teleportDestination = new Vector3Int(player.currentTilePosition.x + x, player.currentTilePosition.y + y, 0); 

        if(!stageManager.CheckValidTile(teleportDestination))
        {
            //First check if the tile ahead that is in the same direction of the leap is free and go there first.
            if(stageManager.CheckValidTile(teleportDestination + aimpoint.GetAimVector3Int()))
            {
                Vector3Int validPosition = teleportDestination + aimpoint.GetAimVector3Int();
                Vector3Int moveDistance = validPosition - player.currentTilePosition;

                StartCoroutine(TeleportIntoObstacleDamageTimer());
                StartCoroutine(player.TweenMove(moveDistance.x, moveDistance.y, teleportSpeed, Ease.OutBounce));
                StartCoroutine(DisableEffectPrefab());

                return;                        
            }else
            if(stageManager.CheckValidTile(teleportDestination - aimpoint.GetAimVector3Int()))
            {//Secondly check if the tile behind that is in the same direction of the leap is free and go there second.
                Vector3Int validPosition = teleportDestination - aimpoint.GetAimVector3Int();
                Vector3Int moveDistance = validPosition - player.currentTilePosition;
 
                StartCoroutine(TeleportIntoObstacleDamageTimer());
                StartCoroutine(player.TweenMove(moveDistance.x, moveDistance.y, teleportSpeed, Ease.OutBounce));
                StartCoroutine(DisableEffectPrefab());

                return; 
            }

            //If the tile the first and seonc check fails, iterate through all adjacent tiles to find a valid landing point.
            foreach(Vector3Int direction in VectorDirections.Vector3IntAll)
            {
                if(stageManager.CheckValidTile(teleportDestination + direction))
                {
                    Vector3Int validPosition = teleportDestination + direction;
                    Vector3Int moveDistance = validPosition - player.currentTilePosition;

                    StartCoroutine(TeleportIntoObstacleDamageTimer());
                    StartCoroutine(player.TweenMove(moveDistance.x, moveDistance.y, teleportSpeed, Ease.OutBounce));
                    StartCoroutine(DisableEffectPrefab());
                    
                    return;
                }
            }
        }else
        {
            StartCoroutine(player.TweenMove(x, y, teleportSpeed, Ease.OutCubic));
            StartCoroutine(DisableHitboxTimer());
            StartCoroutine(DisableEffectPrefab());
        }

    }

    void MoveToLocation(Vector3Int location)
    {

    }


    IEnumerator TeleportIntoObstacleDamageTimer()
    {
        player.playerCollider.enabled = false;
        yield return new WaitForSeconds(teleportSpeed);
        player.playerCollider.enabled = true;
        AttackPayload damage = new AttackPayload(15)
        {
            attacker = player.gameObject,
            attackerSprite = player.GetComponent<SpriteRenderer>().sprite,
            causeOfDeathNote = "A bit of a bruise"
            
        };
        player.HurtEntity(damage);

    }

    IEnumerator DisableHitboxTimer()
    {
        player.playerCollider.enabled = false;
        yield return new WaitForSeconds(teleportSpeed);
        player.playerCollider.enabled = true;        
    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(teleportSpeed + 0.05f);
        gameObject.SetActive(false);
    }
}
