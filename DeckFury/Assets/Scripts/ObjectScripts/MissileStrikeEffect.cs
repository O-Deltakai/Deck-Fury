using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class MissileStrikeEffect : MonoBehaviour
{
    [Header("Missile Settings")]
    [SerializeField] GameObject _missileObject;
    [SerializeField] Vector3 _missileSpawnOffset;
    public float _missileSpeed;
    [SerializeField] Ease _missileTravelEase;


    [Header("VFX Settings")]
    [SerializeField] GameObject _primaryExplosionVFX;
    [SerializeField] AnimationClip _primaryExplosionClip;
    [SerializeField] AnimationEventIntermediary _primaryExplosionVFXIntermediary;
    [SerializeField] GameObject _secondaryExplosionVFX;
    [SerializeField] AnimationClip _secondaryExplosionClip;
    [SerializeField] AnimationEventIntermediary _secondaryExplosionVFXIntermediary;

    [Header("Hitbox Settings")]
    [SerializeField] BoxCollider2D _primaryHitbox;
    [SerializeField] BoxCollider2D _secondaryHitbox;
    [SerializeField] LayerMask _targetLayer;

    [Header("Reticle Settings")]
    [SerializeField] GameObject _primaryReticle;
    [SerializeField] GameObject _secondaryReticle;
    [SerializeField] float _reticleScaleDuration;
    [SerializeField] float _reticleEndScale;
    [SerializeField] Ease _reticleScaleEase;
    float _originalSecondaryReticleScale;
    Tween secondaryReticleScaleTween;

    [Header("Light Settings")]
    [SerializeField] Light2D _explosionLight;
    float _originalLightIntensity;

    [Header("Camera Shake Settings")]
    [SerializeField] float _shakeDuration;
    [SerializeField] Vector3 _shakeVelocity;
    CinemachineImpulseSourceHelper _impulseSourceHelper;

    [Header("SFX Settings")]
    [SerializeField] EventReference missileLaunchSFX;
    EventInstance missileLaunchInstance;
    [SerializeField] EventReference missileExplosionSFX;

    public AttackPayload attackPayload;

    void Awake()
    {
        _impulseSourceHelper = GetComponent<CinemachineImpulseSourceHelper>();
        missileLaunchInstance = RuntimeManager.CreateInstance(missileLaunchSFX);
        RuntimeManager.AttachInstanceToGameObject(missileLaunchInstance, _missileObject.transform);

        _originalSecondaryReticleScale = _secondaryReticle.transform.localScale.x;
        _originalLightIntensity = _explosionLight.intensity;

        _primaryExplosionVFX.SetActive(false);
        _secondaryExplosionVFX.SetActive(false);

        _primaryExplosionVFXIntermediary.OnAnimationEvent += TriggerPrimaryHitbox;
        _secondaryExplosionVFXIntermediary.OnAnimationEvent += TriggerSecondaryHitbox;
    }

    void Start()
    {
    }

    void OnEnable()
    {
        missileLaunchInstance.start();
        _explosionLight.enabled = false;
        _explosionLight.intensity = _originalLightIntensity;
        _secondaryReticle.SetActive(true);


        _secondaryReticle.transform.localScale = _originalSecondaryReticleScale * Vector3.one;
        _missileObject.transform.localPosition = _missileSpawnOffset;
        _missileObject.SetActive(true);


        ScaleSecondaryReticle();
        MoveMissileToTarget();
    }

    void OnDisable()
    {
        _primaryExplosionVFX.SetActive(false);
        _secondaryExplosionVFX.SetActive(false);
    }

    void ScaleSecondaryReticle()
    {
        if(secondaryReticleScaleTween.IsActive())
        {
            secondaryReticleScaleTween.Kill();
        }

        secondaryReticleScaleTween = _secondaryReticle.transform.DOScale(_reticleEndScale, _missileSpeed).SetEase(_reticleScaleEase);
    }

    void MoveMissileToTarget()
    {

        _missileObject.transform.DOLocalMoveY(0, _missileSpeed).SetEase(_missileTravelEase).OnComplete(() => ExplodeMissile());
    }

    void ExplodeMissile()
    {
        missileLaunchInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _missileObject.SetActive(false);
        _secondaryReticle.SetActive(false);

        _primaryExplosionVFX.SetActive(true);
        _secondaryExplosionVFX.SetActive(true);

        StartCoroutine(SelfDestruct(_primaryExplosionClip.length));
    }

    void TriggerPrimaryHitbox()
    {
        RuntimeManager.PlayOneShot(missileExplosionSFX, transform.position);
        if(_impulseSourceHelper) _impulseSourceHelper.ShakeCameraRandomCircle(_shakeVelocity * SettingsManager.GlobalCameraShakeMultiplier, _shakeDuration, 1);

        _explosionLight.enabled = true;
        DOTween.To(() => _explosionLight.intensity, x => _explosionLight.intensity = x, 0, 0.25f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(_primaryHitbox.transform.position, _primaryHitbox.size, 0, _targetLayer);
        if(hits.Length == 0) { return; }

        foreach (Collider2D hit in hits)
        {
            if(hit.TryGetComponent(out StageEntity stageEntity))
            {
                if(hit.CompareTag(TagNames.Enemy.ToString()) || hit.CompareTag(TagNames.EnvironmentalHazard.ToString()))
                {
                    stageEntity.HurtEntity(attackPayload);
                }

            }
        }
    }

    void TriggerSecondaryHitbox()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(_secondaryHitbox.transform.position, _secondaryHitbox.size, 0, _targetLayer);
        if(hits.Length == 0) { return; }

        foreach (Collider2D hit in hits)
        {
            if(hit.TryGetComponent(out StageEntity stageEntity))
            {
                if(hit.CompareTag(TagNames.Enemy.ToString()) || hit.CompareTag(TagNames.EnvironmentalHazard.ToString()))
                {
                    stageEntity.HurtEntity(attackPayload);
                }

            }
        }
    }

    IEnumerator SelfDestruct(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }


}
