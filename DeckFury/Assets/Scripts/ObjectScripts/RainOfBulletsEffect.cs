using System.Collections;
using System.Collections.Generic;
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

    BoxCollider2D hitboxCollider;


    void Awake()
    {

        hitboxCollider = GetComponent<BoxCollider2D>();
        splashLight.SetActive(false);
        animationEventIntermediary.OnAnimationEvent += TriggerDamage;
    }

    void Start()
    {
        RuntimeManager.PlayOneShot(chargeSFX, transform.position);
        
        StartCoroutine(SelfDestruct());
    }

    void TriggerDamage()
    {
        RuntimeManager.PlayOneShot(shotSFX, transform.position);
        StartCoroutine(TriggerLight());
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
