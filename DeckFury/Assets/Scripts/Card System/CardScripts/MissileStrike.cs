using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileStrike : CardEffect
{
    [SerializeField] GameObject missilePrefab;
    TargetClosestTile targetClosestTile;
    EventBinding<RelayGameObjectEvent> targetingSystemRelayBinding;

    [SerializeField] bool useTargetingAlgorithm = false;


    protected override void Awake()
    {
        base.Awake();
        targetingSystemRelayBinding = new EventBinding<RelayGameObjectEvent>(HandleEventData);
        EventBus<RelayGameObjectEvent>.Register(targetingSystemRelayBinding);

    }

    void OnDestroy()
    {
        EventBus<RelayGameObjectEvent>.Deregister(targetingSystemRelayBinding);
    }

    void HandleEventData(RelayGameObjectEvent data)
    {
        if (data.gameObject.TryGetComponent(out TargetClosestTile targetSystem))
        {
            targetClosestTile = targetSystem;
        }
    }

    public override void ActivateCardEffect()
    {
        MissileStrikeEffect missileStrikeEffect;

        if(useTargetingAlgorithm && targetClosestTile != null)
        {
            missileStrikeEffect = Instantiate(missilePrefab, targetClosestTile.TargetPosition, Quaternion.identity)
            .GetComponent<MissileStrikeEffect>();                
        }else
        {
            missileStrikeEffect = Instantiate(missilePrefab, player.currentTilePosition + player.aimpoint.GetAimVector3Int() * 4, Quaternion.identity)
            .GetComponent<MissileStrikeEffect>();
        }
        missileStrikeEffect.attackPayload = attackPayload;        
        missileStrikeEffect._missileSpeed = (float)cardSO.QuantifiableEffects[0].GetValueDynamic();
    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield return null;
    }

}
