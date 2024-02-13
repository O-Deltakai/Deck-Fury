using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingShot : CardEffect
{
    
    //[SerializeField] GameObject VFXPrefabToUse; //Set in inspector

    public override void ActivateCardEffect()
    {
        if(!cardSO.ObjectSummonsArePooled)
        {
            Bullet arrow = Instantiate(cardSO.ObjectSummonList[0], player.currentTilePosition,
            new Quaternion(0, 0, (float)player.aimpoint.currentAimDirection, 0)).GetComponent<Bullet>();

            //arrow.objectIsPooled = false;
            AssignVariable(arrow);

        }else
        {
            Bullet arrow = ObjectSummonList[0].GetComponent<Bullet>();
            AssignVariable(arrow);
            //arrow.objectIsPooled = true;
        }

        StartCoroutine(DisableEffectPrefab());
    }

    //design for 3s way and 5 ways
    protected void AssignVariable(Bullet arrow){
            arrow.transform.position = player.currentTilePosition;
            arrow.attackPayload = attackPayload;
            //arrow.speed = 20;

            switch (player.aimpoint.currentAimDirection) 
            {
                case AimDirection.Up:
                    arrow.transform.rotation = Quaternion.Euler(0, 0, 180);
                    arrow.velocity.x = 0;
                    arrow.velocity.y = 1;
                    break;

                case AimDirection.Down:
                    arrow.transform.rotation = Quaternion.Euler(0, 0, 0);
                    arrow.velocity.x = 0;
                    arrow.velocity.y = -1;
                    break; 
                case AimDirection.Left:
                    arrow.transform.rotation = Quaternion.Euler(0, 0, -90);
                    arrow.velocity.x = -1;
                    arrow.velocity.y = 0;
                    break;

                case AimDirection.Right:
                    arrow.transform.rotation = Quaternion.Euler(0, 0, 90);
                    arrow.velocity.x = 1;
                    arrow.velocity.y = 0;
                    break;
            }
            arrow.gameObject.SetActive(true);
    }


    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(cardSO.PlayerAnimation.length);
        gameObject.SetActive(false);

    }




}