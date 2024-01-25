using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExplosionOnKillItem : ItemBase
{
    [SerializeField] GameObject fireExplosionPrefab;



    public override void Initialize()
    {
        base.Initialize();

        player.OnKillSpecificEnemy += ProcEvent;


    }

    void ProcEvent(NPC enemy)
    {

        MarkedFireExplosion fireExplosion = Instantiate(fireExplosionPrefab, enemy.worldTransform.position, Quaternion.identity)
                                                        .GetComponent<MarkedFireExplosion>();
        AttackPayload explosionPayload = new AttackPayload
        {
            damage = itemSO.QuantifiableEffects[0].IntegerQuantity,
            attackElement = AttackElement.Fire,
            canTriggerMark = false
        };
        fireExplosion.Trigger(explosionPayload);

        Proc();


    }




}
