using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class LightningOrbProjectile : Bullet
{
    [SerializeField] GameObject VFX;
    [SerializeField] EventReference impactSFX;

    protected override void Awake()
    {
        base.Awake();

        OnImpact += Impact;
        
    }

    void Impact()
    {
        if(VFX)
        {
            Instantiate(VFX, transform.position, Quaternion.identity);
        }

        if(!impactSFX.IsNull)
        {
            RuntimeManager.PlayOneShot(impactSFX, transform.position);
        }

    }


}
