using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShieldSteal : CardEffect
{


    [SerializeField] AnimationClip VFXAnimation; //Set in inspector
    [SerializeField] BoxCollider2D attackHitbox;
    [SerializeField] Light2D attackLight;
    AimpointController aimpoint;

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
        attackLight.intensity = 0.8f;
        DOTween.To(() => attackLight.intensity, x => attackLight.intensity = x, 0, 0.25f);
        aimpoint = player.aimpoint;
        FaceTowardsAimpoint(aimpoint);

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


            if(!other.gameObject.CompareTag(TagNames.EnvironmentalHazard.ToString()))
            {
                player.ShieldHP+=(int)cardSO.QuantifiableEffects[0].GetValueDynamic();
            }

            entity.HurtEntity(attackPayload);

            attackHitbox.enabled = false;
        }

    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(cardSO.PlayerAnimation.length);
        gameObject.SetActive(false);
        attackHitbox.enabled = true;
    }
}
