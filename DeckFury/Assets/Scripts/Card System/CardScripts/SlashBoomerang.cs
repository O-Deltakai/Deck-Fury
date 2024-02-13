using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashBoomerang : CardEffect
{

    public override void ActivateCardEffect()
    {
        if(!cardSO.ObjectSummonsArePooled)
        {
            Boomerang boomerang = Instantiate(cardSO.ObjectSummonList[0], player.currentTilePosition,
            new Quaternion(0, 0, (float)player.aimpoint.currentAimDirection, 0)).GetComponent<Boomerang>();
            boomerang.objectIsPooled = false;

            boomerang.maxBounces = (int)cardSO.QuantifiableEffects[0].GetValueDynamic();
            boomerang.damageBonusPerBounce = (int)cardSO.QuantifiableEffects[1].GetValueDynamic();

            AssignVariable(boomerang);

        }else
        {
            Boomerang boomerang = ObjectSummonList[0].GetComponent<Boomerang>();
            boomerang.objectIsPooled = true;
            AssignVariable(boomerang);
        }

        StartCoroutine(DisableEffectPrefab());
    }

    //set position and rotation for object
    protected void AssignVariable(Boomerang boomerang)
    {
        boomerang.transform.position = player.currentTilePosition;
        boomerang.attackPayload = attackPayload;

        switch (player.aimpoint.currentAimDirection) 
        {
            case AimDirection.Up:
                boomerang.velocity.x = 0 ;
                boomerang.velocity.y = 1;
                break;

            case AimDirection.Down:
                boomerang.velocity.x = 0 ;
                boomerang.velocity.y = -1;
                break; 
            case AimDirection.Left:
                boomerang.velocity.x = -1;
                boomerang.velocity.y = 0 ;
                break;

            case AimDirection.Right:
                boomerang.velocity.x = 1;
                boomerang.velocity.y = 0 ;
                break;
        }
        
        boomerang.gameObject.SetActive(true);
    }


    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(cardSO.PlayerAnimation.length);
        gameObject.SetActive(false);

    }




}
