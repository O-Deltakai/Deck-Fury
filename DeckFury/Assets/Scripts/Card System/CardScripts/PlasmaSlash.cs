using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaSlash : CardEffect
{
    [SerializeField] GameObject slashEffectPrefab;

    public override void ActivateCardEffect()
    {
        PlasmaSlashEffect plasmaSlashEffect = Instantiate(slashEffectPrefab, player.currentTilePosition,
            Quaternion.Euler(player.aimpoint.GetAimRotation())).GetComponent<PlasmaSlashEffect>();

        plasmaSlashEffect.attackPayload = attackPayload;
    }


    protected override IEnumerator DisableEffectPrefab()
    {
        yield return null;
    }
}
