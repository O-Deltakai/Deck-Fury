using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;

public class DamageBufferOnFullEnergy : ItemBase
{
    [SerializeField] GameObject bufferVFXObject;
    Vector3 originalVFXScale;
    [SerializeField] SpriteRenderer bufferSpriteRenderer;


    DamageBuffer damageBuffer = new DamageBuffer();
    EnergyController energyController;

    bool bufferActive = false;

    [Header("VFX Properties")]
    [SerializeField] Vector3 dissipateScale = new Vector3(1.5f, 1.5f);
    [SerializeField] float dissipateSpeed = 0.25f;

    [Header("SFX")]
    [SerializeField] EventReference breakBufferSFX;
    [SerializeField] EventReference regenerateBufferSFX;

    Tween rotateVFXTween;

    void Update()
    {
        if(bufferActive)
        {
            transform.position = player.worldTransform.position;
        }
    }

    public override void Initialize()
    {
        originalVFXScale = bufferVFXObject.transform.localScale;

        energyController = EnergyController.Instance;

        damageBuffer.source = gameObject;
        damageBuffer.OnBufferRemoved += BreakBuffer;

        player.BufferList.Add(damageBuffer);
        bufferActive = true;

        energyController.OnFullCharge += Proc;
        ActivateBufferVFX();

        base.Initialize();
    }


    public override void Proc()
    {
        if(bufferActive){ return; }
        base.Proc();

        player.BufferList.Add(damageBuffer);
        bufferActive = true;
        ActivateBufferVFX();
        RuntimeManager.PlayOneShotAttached(regenerateBufferSFX, player.gameObject);

    }



    void ActivateBufferVFX()
    {
        bufferVFXObject.SetActive(true);
        bufferVFXObject.transform.DOScale(originalVFXScale, dissipateSpeed);
        bufferSpriteRenderer.DOFade(1, dissipateSpeed);

        if(!rotateVFXTween.IsActive())
        {
            rotateVFXTween = bufferVFXObject.transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        }
    }

    void BreakBuffer()
    {
        bufferVFXObject.transform.DOScale(dissipateScale, dissipateSpeed);
        bufferSpriteRenderer.DOFade(0, dissipateSpeed);
        bufferActive = false;
        RuntimeManager.PlayOneShotAttached(breakBufferSFX, player.gameObject);
    }

}
