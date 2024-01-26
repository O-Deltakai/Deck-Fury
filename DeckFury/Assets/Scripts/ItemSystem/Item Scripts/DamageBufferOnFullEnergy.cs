using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBufferOnFullEnergy : ItemBase
{
    [SerializeField] GameObject bufferVFXObject;
    Vector3 originalVFXScale;
    [SerializeField] SpriteRenderer bufferSpriteRenderer;


    DamageBuffer damageBuffer = new DamageBuffer();
    EnergyController energyController;

    bool bufferActive = false;


    public override void Initialize()
    {
        base.Initialize();
        originalVFXScale = bufferVFXObject.transform.localScale;

        energyController = EnergyController.Instance;

        damageBuffer.source = gameObject;
        damageBuffer.OnBufferRemoved += BreakBufferVFX;

        player.BufferList.Add(damageBuffer);
        bufferActive = true;

        energyController.OnFullCharge += Proc;

    }

    public override void Proc()
    {
        if(bufferActive){ return; }
        base.Proc();

        player.BufferList.Add(damageBuffer);
        bufferActive = true;
        ActivateBufferVFX();

    }

    void ActivateBufferVFX()
    {
        bufferVFXObject.SetActive(true);
    }

    void BreakBufferVFX()
    {

    }

}
