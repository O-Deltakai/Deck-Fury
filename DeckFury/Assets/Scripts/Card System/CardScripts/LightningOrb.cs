using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningOrb : CardEffect
{
    [SerializeField] GameObject lightningOrbPrefab;

    public override void ActivateCardEffect()
    {
        LightningOrbProjectile lightningOrb = Instantiate(lightningOrbPrefab, player.currentTilePosition,
            new Quaternion(0, 0, (float)player.aimpoint.currentAimDirection, 0)).GetComponent<LightningOrbProjectile>();

        lightningOrb.attackPayload = attackPayload;
        lightningOrb.velocity = player.aimpoint.GetAimVector2Int();
        lightningOrb.transform.rotation = Quaternion.Euler(player.aimpoint.GetAimRotation());


    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield break;
    }
}
