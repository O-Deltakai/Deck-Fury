using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTrailDamageItem : ItemBase
{


    [SerializeField] AttackPayload dashAttackPayload;

    [SerializeField] LayerMask targetLayer;


    protected override void Awake()
    {
        base.Awake();
    }

    void FixedUpdate()
    {
        transform.position = player.worldTransform.position;
    }


    public override void Initialize()
    {
        base.Initialize();
        stageManager = StageManager.Instance;
        player.DashController.OnDash += Proc;
    }


    public override void Proc()
    {
        base.Proc();

        StartCoroutine(DashHitboxTimer(player.DashController.DashSpeed));
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
