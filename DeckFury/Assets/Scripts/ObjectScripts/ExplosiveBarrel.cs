using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ExplosiveBarrel : StageEntity
{
    CinemachineImpulseSourceHelper impulseSourceHelper;


    [SerializeField] BoxCollider2D barrelCollider;

    [SerializeField] AttackPayload attackPayload;

    [SerializeField] GameObject shadow;
    [SerializeField] float fuseTimer = 0.5f;


    [SerializeField] Vector2 cameraShakeVelocity;

[Header("Explosion Settings")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] BoxCollider2D explosionCollider;
    [SerializeField] EventReference explosionSFX;
    [SerializeField] Light2D explosionLight;
 

    bool isExploding = false;

    protected override void Awake()
    {
        base.Awake();
        barrelCollider = GetComponent<BoxCollider2D>();
        explosionCollider.enabled = false;
        explosionLight.enabled = false;
        OnHPChanged += ExplodeBarrel;

        impulseSourceHelper = GetComponent<CinemachineImpulseSourceHelper>();
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SetPositionTimer());
    }

    IEnumerator SetPositionTimer()
    {
        yield return new WaitForEndOfFrame();
        _stageManager = StageManager.Instance;

        currentTilePosition.Set((int)worldTransform.position.x, (int)worldTransform.position.y, 0);
        _stageManager.SetTileEntity(this, currentTilePosition);
    }


    void ExplodeBarrel(int oldValue, int newValue)
    {
        barrelCollider.enabled = false;
        StartCoroutine(_statusEffectManager.FlashColor(Color.red, fuseTimer, 0.075f));
        StartCoroutine(ExplosionTimer());

    }

    void TriggerExplosionHitbox()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(explosionCollider.transform.position, explosionCollider.size, 0, targetLayer);

        foreach(Collider2D collider2D in hits) 
        {
            if(!collider2D.gameObject.TryGetComponent<StageEntity>(out var entityHit))
            {
                print("collider did not have a StageEntity attached");
                continue;
            }

            entityHit.HurtEntity(attackPayload);                        
        }

    }

    IEnumerator ExplosionTimer()
    {
        yield return new WaitForSeconds(fuseTimer + 0.05f);

        RuntimeManager.PlayOneShot(explosionSFX, transform.position);

        impulseSourceHelper.ShakeCameraRandomCircle(cameraShakeVelocity * SettingsManager.GlobalCameraShakeMultiplier, 0.3f, 1.05f);
        shadow.SetActive(false);
        explosionLight.enabled = true;
        DOTween.To(() => explosionLight.intensity, x => explosionLight.intensity = x, 0, 0.25f).SetUpdate(true).SetEase(Ease.InQuad);
        TriggerExplosionHitbox();
        _stageManager.SetTileEntity(null, currentTilePosition);
        StartCoroutine(DestroyEntity());

    }




}
