using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazookaLauncher : CardEffect
{

    public override void ActivateCardEffect()
    {
        if(!cardSO.ObjectSummonsArePooled)
        {
            Bazooka bazooka = Instantiate(cardSO.ObjectSummonList[0], player.currentTilePosition,
            new Quaternion(0, 0, (float)player.aimpoint.currentAimDirection, 0)).GetComponent<Bazooka>();
            bazooka.objectIsPooled = false;
            AssignVariable(bazooka);

        }else
        {
            Bazooka bazooka = ObjectSummonList[0].GetComponent<Bazooka>();
            bazooka.objectIsPooled = true;
            AssignVariable(bazooka);
        }

        StartCoroutine(DisableEffectPrefab());
    }


    //set position and rotation for object
    protected void AssignVariable(Bazooka bazooka)
    {
        bazooka.transform.position = player.currentTilePosition + player.aimpoint.GetAimVector3Int();
        bazooka.attackPayload = attackPayload;
        bazooka.impactPayload = attackPayload;
        bazooka.impactPayload.canTriggerMark = false;
        bazooka.impactPayload.attackElement = AttackElement.Fire;
        bazooka.impactPayload.statusEffects = new List<StatusEffect>();     
        //bazooka.speed = 10;

        switch (player.aimpoint.currentAimDirection) 
        {
            case AimDirection.Up:
                bazooka.transform.rotation = Quaternion.Euler(0, 0, -90);
                bazooka.velocity.x = 0 ;
                bazooka.velocity.y = 1;
                break;

            case AimDirection.Down:
                bazooka.transform.rotation = Quaternion.Euler(0, 0, 90);
                bazooka.velocity.x = 0 ;
                bazooka.velocity.y = -1;
                break; 
            case AimDirection.Left:
                bazooka.transform.rotation = Quaternion.Euler(0, 0, 0);
                bazooka.velocity.x = -1;
                bazooka.velocity.y = 0 ;
                break;

            case AimDirection.Right:
                bazooka.transform.rotation = Quaternion.Euler(0, 0, 180);
                bazooka.velocity.x = 1;
                bazooka.velocity.y = 0 ;
                break;
        }
        
        bazooka.gameObject.SetActive(true);
    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(cardSO.PlayerAnimation.length);
        gameObject.SetActive(false);

    }




}
