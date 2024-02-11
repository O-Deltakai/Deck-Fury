using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainOfBullets : CardEffect
{
    [SerializeField] GameObject rainOfBulletsPrefab;

    public override void ActivateCardEffect()
    {
        RainOfBulletsEffect rainOfBulletsEffect = Instantiate(rainOfBulletsPrefab, player.currentTilePosition + player.aimpoint.GetAimVector3Int() * 3, Quaternion.identity)
                                                    .GetComponent<RainOfBulletsEffect>();
        rainOfBulletsEffect.attackPayload = attackPayload;


    }



    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(0.01f);
        gameObject.SetActive(false);
    }
}
