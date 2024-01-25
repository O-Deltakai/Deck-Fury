using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class DashTrailDamageItem : ItemBase
{

    [SerializeField] AttackPayload dashAttackPayload;
    [SerializeField] LayerMask targetLayer;

    [SerializeField] Gradient dashTrailColorOverride;
    [SerializeField] EventReference dashSFXOverride;

    [SerializeField] GameObject auraVFX;

    bool vfxEnabled = false;

    protected override void Awake()
    {
        base.Awake();
        dashAttackPayload = new AttackPayload(itemSO.QuantifiableEffects[0].IntegerQuantity)
        {
            attackElement = AttackElement.Neutral,
            canTriggerMark = false
        };
        auraVFX.SetActive(false);
    }

    public override void Initialize()
    {
        base.Initialize();
        stageManager = StageManager.Instance;
        player.DashController.DashTrail.colorGradient = dashTrailColorOverride;
        player.DashController.dashSFX = dashSFXOverride;
        player.DashController.OnDash += Proc;
        dashAttackPayload.attacker = player.gameObject;
    }


    public override void Proc()
    {
        base.Proc();

        StartCoroutine(DashHitboxTimer(player.DashController.DashSpeed));
        StartCoroutine(AuraVFXTimer(player.DashController.DashSpeed));

    }

    void Update()
    {
        if(vfxEnabled)
        {
            transform.position = player.worldTransform.position;
        }
    }

    IEnumerator AuraVFXTimer(float duration)
    {
        vfxEnabled = true;
        auraVFX.SetActive(true);
        yield return new WaitForSeconds(duration);
        auraVFX.SetActive(false);
        vfxEnabled = false;

    }

    IEnumerator DashHitboxTimer(float duration)
    {
        Vector2 startPosition = player.DashController.StartPosition;
        
        Vector2 endPosition = player.DashController.EndPosition;
        yield return new WaitForSeconds(duration * 0.5f);

        

        RaycastHit2D[] hits = Physics2D.LinecastAll(startPosition, endPosition, targetLayer);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag(TagNames.Enemy.ToString()))
            {
                // Damage each enemy
                if (hit.collider.TryGetComponent<StageEntity>(out var enemy))
                {
                    enemy.HurtEntity(dashAttackPayload);
                }
            }
        }
    }


}
