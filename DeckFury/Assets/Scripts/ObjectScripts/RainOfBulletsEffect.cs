using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;


public class RainOfBulletsEffect : MonoBehaviour
{
    [SerializeField] AnimationEventIntermediary animationEventIntermediary;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] AnimationClip vfxAnimation;
    [SerializeField] GameObject splashLight;

    public AttackPayload attackPayload;

    [Header("SFX")]
    [SerializeField] EventReference chargeSFX;
    EventInstance effectSFXInstance;

    [SerializeField] EventReference shotSFX;



    [Header("Crosshair Settings")]
    [SerializeField] GameObject crosshairObject;
    Tween crosshairRotateTween;
    Tween crosshairScaleTween;
    Tween crosshairColorTween;

    [Header("Camera Shake Settings")]
    [SerializeField] Vector3 cameraShakeVelocity;
    [SerializeField] float cameraShakeDuration = 0.1f;


    BoxCollider2D hitboxCollider;
    CinemachineImpulseSourceHelper impulseSourceHelper;
    
    void Awake()
    {
        impulseSourceHelper = GetComponent<CinemachineImpulseSourceHelper>();
        hitboxCollider = GetComponent<BoxCollider2D>();
        splashLight.SetActive(false);
        animationEventIntermediary.OnAnimationEvent += TriggerDamage;
    }

    void Start()
    {
        RuntimeManager.PlayOneShot(chargeSFX, transform.position);
        AnimateCrosshair();
        StartCoroutine(SelfDestruct());
    }

    void TriggerDamage()
    {
        RuntimeManager.PlayOneShot(shotSFX, transform.position);
        StartCoroutine(TriggerLight());
        impulseSourceHelper.ShakeCameraRandomCircle(cameraShakeVelocity * SettingsManager.GlobalCameraShakeMultiplier, cameraShakeDuration, 1f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxCollider.transform.position, hitboxCollider.size, 0, targetLayer);

        if (hits.Length == 0)
        {
            return;
        }

        foreach (Collider2D collider2D in hits)
        {
            if (!collider2D.gameObject.TryGetComponent<StageEntity>(out var entityHit))
            {
                print("collider did not have a StageEntity attached");
                continue;
            }

            if (entityHit.CompareTag(TagNames.Enemy.ToString()) || entityHit.CompareTag(TagNames.EnvironmentalHazard.ToString()))
            {
                entityHit.HurtEntity(attackPayload);                        
            }
        }

    }

    void AnimateCrosshair()
    {
        crosshairRotateTween = crosshairObject.transform.DORotate(new Vector3(0, 0, 270), 0.5f, RotateMode.FastBeyond360);
        crosshairScaleTween = crosshairObject.transform.DOScale(0.8f, 0.5f);
        crosshairColorTween = crosshairObject.GetComponent<SpriteRenderer>().DOColor(Color.red, 0.5f);
    }


    IEnumerator TriggerLight()
    {
        splashLight.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        splashLight.SetActive(false);
        
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(vfxAnimation.length);  
        Destroy(gameObject);
    }


}
