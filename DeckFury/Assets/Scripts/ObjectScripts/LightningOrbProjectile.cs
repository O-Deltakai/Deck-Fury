using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class LightningOrbProjectile : Bullet
{
    [SerializeField] GameObject VFX;
    [SerializeField] EventReference impactSFX;

    [SerializeField] AnimationCurve accelerationCurve;
    [SerializeField] float accelerationDuration = 1f;

    protected override void Awake()
    {
        base.Awake();

        OnImpact += Impact;
        
    }

    protected override void Start()
    {
        base.Start();
        AccelerateOverTime();
    }

    void AccelerateOverTime()
    {
        StartCoroutine(Accelerate());
    }

    IEnumerator Accelerate()
    {
        float timer = 0;
        float startSpeed = speed;
        while(timer < accelerationDuration)
        {
            speed = accelerationCurve.Evaluate(timer / accelerationDuration) * startSpeed;
            timer += Time.deltaTime;
            yield return null;
        }
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


    public override void Reflect(GameObject reflector)
    {
        base.Reflect(reflector);
    }


}
