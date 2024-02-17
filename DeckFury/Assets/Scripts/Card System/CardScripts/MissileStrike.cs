using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileStrike : CardEffect
{
    [SerializeField] GameObject missilePrefab;

    public override void ActivateCardEffect()
    {
        MissileStrikeEffect missileStrikeEffect = Instantiate(missilePrefab, player.currentTilePosition + player.aimpoint.GetAimVector3Int() * 4, Quaternion.identity)
                                                    .GetComponent<MissileStrikeEffect>();
        missileStrikeEffect.attackPayload = attackPayload;        
        missileStrikeEffect._missileSpeed = (float)cardSO.QuantifiableEffects[0].GetValueDynamic();
    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield return null;
    }

}
