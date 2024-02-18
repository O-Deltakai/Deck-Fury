using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorSlash : CardEffect
{
    [SerializeField] GameObject scissorSlashPrefab;

    public override void ActivateCardEffect()
    {
        ScissorSlashEffect scissorSlashEffect = Instantiate(scissorSlashPrefab, player.currentTilePosition + player.aimpoint.GetAimVector3Int() * 2,
            Quaternion.Euler(0, 0, (float)player.aimpoint.currentAimDirection)).GetComponent<ScissorSlashEffect>();

        scissorSlashEffect.attackPayload = attackPayload;
    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield return null;
    }

}
