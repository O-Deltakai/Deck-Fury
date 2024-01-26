using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExplosionOnKillItem : ItemBase
{
    [SerializeField] GameObject fireExplosionPrefab;



    public override void Initialize()
    {
        base.Initialize();

        player.OnKillEnemyWithPayload += ProcEvent;


    }

    void ProcEvent(NPC enemy, AttackPayload payload)
    {
        if(payload.attackElement != AttackElement.Fire) { return; }

        Vector3 explosionPosition = enemy.worldTransform.position;

        StartCoroutine(ProcTimer(enemy, explosionPosition));


    }

    IEnumerator ProcTimer(NPC enemy, Vector3 worldPosition)
    {
        yield return new WaitForSeconds(0.15f);
        MarkedFireExplosion fireExplosion = Instantiate(fireExplosionPrefab, worldPosition, Quaternion.identity)
                                                        .GetComponent<MarkedFireExplosion>();
        AttackPayload explosionPayload = new AttackPayload
        {
            damage = itemSO.QuantifiableEffects[0].IntegerQuantity,
            attackElement = AttackElement.Fire,
            attacker = player.gameObject,
            canTriggerMark = false
        };
        fireExplosion.Trigger(explosionPayload);

        Proc();
    }



}
