using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBomb : CardEffect
{

    public override void ActivateCardEffect()
    {
        if(!cardSO.ObjectSummonsArePooled)
        {
            FireBombController fireBomb = Instantiate(cardSO.ObjectSummonList[0], player.currentTilePosition,
            new Quaternion(0, 0, (float)player.aimpoint.currentAimDirection, 0)).GetComponent<FireBombController>();

            fireBomb.transform.position = player.currentTilePosition;
            fireBomb.attackPayload = attackPayload;
            fireBomb.aimDirection = player.aimpoint.currentAimDirection;
            fireBomb.objectIsPooled = false;

            fireBomb.gameObject.SetActive(true);

        }else
        {
            FireBombController fireBomb = ObjectSummonList[0].GetComponent<FireBombController>();

            fireBomb.transform.position = player.currentTilePosition;
            fireBomb.attackPayload = attackPayload;
            fireBomb.aimDirection = player.aimpoint.currentAimDirection;
            fireBomb.objectIsPooled = true;

            fireBomb.gameObject.SetActive(true);
        }


        StartCoroutine(DisableEffectPrefab());
    }




    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(cardSO.PlayerAnimation.length);
        gameObject.SetActive(false);

    }




}
