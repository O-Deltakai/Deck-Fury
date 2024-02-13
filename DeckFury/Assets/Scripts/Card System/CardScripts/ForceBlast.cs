using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;

public class ForceBlast : CardEffect
{
    CinemachineImpulseSource impulseSource;


    [SerializeField] AnimationClip VFXAnimationClip;
    [SerializeField] BoxCollider2D effectCollider;

    [SerializeField] LayerMask targetLayer;

    AimpointController aimpoint;

    protected override void Awake()
    {
        base.Awake();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public override void ActivateCardEffect()
    {
        StartCoroutine(DisableEffectPrefab());

        //StartCoroutine(HardDisableTimer());
        aimpoint = player.aimpoint;
        transform.position = player.worldTransform.position;

        if(impulseSource)
        {
            impulseSource.GenerateImpulseWithVelocity(impulseSource.m_DefaultVelocity * SettingsManager.GlobalCameraShakeMultiplier);
        }

        FaceTowardsAimpoint(aimpoint);

        Collider2D[] hits = Physics2D.OverlapBoxAll(effectCollider.transform.position, effectCollider.size, effectCollider.transform.eulerAngles.z, targetLayer);
        if(hits.Length == 0){return;}
        if(hits == null) { return; }

        var sortedEntities = hits.OrderBy(h => -Vector2.Distance(h.transform.position, transform.position)).ToList();
        

        foreach(var collider2D in sortedEntities) 
        {
            if(collider2D.TryGetComponent<IReflectable>(out IReflectable reflectable))
            {
                reflectable.Reflect(player.gameObject);   
            }

            if(collider2D.TryGetComponent<StageEntity>(out StageEntity entity))
            {
                if(entity.CompareTag(TagNames.Enemy.ToString()) || entity.CompareTag(TagNames.EnvironmentalHazard.ToString()))
                {
                    entity.HurtEntity(attackPayload);
                    Vector2Int shoveDirection = aimpoint.GetAimVector2Int() * (int)cardSO.QuantifiableEffects[0].GetValueDynamic();

                    entity.AttemptMovement(shoveDirection.x, shoveDirection.y, 0.15f, DG.Tweening.Ease.OutQuart, ForceMoveMode.Forward);         
                }
            }



        }

        

    }



    //Rotate the object towards the aimpoint so that it fires in the direction of the aimpoint
    void FaceTowardsAimpoint(AimpointController aimpoint)
    {
        switch (aimpoint.currentAimDirection) 
        {
            case AimDirection.Up:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;

            case AimDirection.Down:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break; 
            case AimDirection.Left:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;

            case AimDirection.Right:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
        }
    }


    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(VFXAnimationClip.length + 0.05f);
        gameObject.SetActive(false);
        
    }

    IEnumerator HardDisableTimer()
    {
        yield return new WaitForSecondsRealtime(0.75f);
        gameObject.SetActive(false);
    }

}
