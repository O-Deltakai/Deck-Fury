using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class ShieldBash : CardEffect
{
    [SerializeField] AnimationClip ShieldBashVFS; //Set in inspector
    [SerializeField] BoxCollider2D attackHitbox;
    [SerializeField] Animator bashAnimator;
    AimpointController aimpoint;
    int extraDamage = 0;

    CinemachineImpulseSourceHelper impulseSourceHelper;
    [Header("Camera Shake Settings")]
    [SerializeField] float _shakeDuration;
    [SerializeField] Vector3 _shakeVelocity;

    [Header("SFX")]
    [SerializeField] EventReference onHitSFX;


    protected override void Awake()
    {
        base.Awake();
        impulseSourceHelper = GetComponent<CinemachineImpulseSourceHelper>();
    }

    public override void ActivateCardEffect()
    {

        aimpoint = player.aimpoint;
        FaceTowardsAimpoint(aimpoint);
        //if shield point more than consume
        if(player.ShieldHP >= (int)cardSO.QuantifiableEffects[0].GetValueDynamic())
        {
            player.ShieldHP -= (int)cardSO.QuantifiableEffects[0].GetValueDynamic();
            extraDamage = (int)cardSO.QuantifiableEffects[1].GetValueDynamic() * (int)cardSO.QuantifiableEffects[0].GetValueDynamic();
        }
        else
        {
            extraDamage = player.ShieldHP * (int)cardSO.QuantifiableEffects[1].GetValueDynamic();
            player.ShieldHP = 0;
        }
        attackPayload.damage += extraDamage;

        StartCoroutine(DisableEffectPrefab());
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(!other.gameObject.TryGetComponent<StageEntity>(out var entity)) {return;}
        if(other.gameObject.CompareTag(TagNames.Enemy.ToString()) || other.gameObject.CompareTag(TagNames.EnvironmentalHazard.ToString()))
        {
            impulseSourceHelper.ShakeCameraRandomCircle(_shakeVelocity * SettingsManager.GlobalCameraShakeMultiplier, _shakeDuration, 1);
            RuntimeManager.PlayOneShot(onHitSFX, transform.position);
            entity.HurtEntity(attackPayload);
            Vector2Int shoveDirection = aimpoint.GetAimVector2Int();
            entity.AttemptMovement(shoveDirection.x*2, shoveDirection.y*2, 0.15f, DG.Tweening.Ease.OutQuart, ForceMoveMode.Forward);
            attackHitbox.enabled = false;
        }

    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(cardSO.PlayerAnimation.length);
        gameObject.SetActive(false);
        attackHitbox.enabled = true;

        attackPayload = new AttackPayload();

        InitializeAttackPayload();
        
    }
}
