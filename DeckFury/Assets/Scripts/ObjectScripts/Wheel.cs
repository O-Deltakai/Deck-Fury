using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FMODUnity;

public class Wheel : MonoBehaviour
{
    public bool objectIsPooled;
    [SerializeField] AnimationClip wheelVFX;
    [SerializeField] GameObject slashVFXParent;
    
    [SerializeField] BoxCollider2D hitbox;
    [SerializeField] LayerMask targetLayer;


    [SerializeField] AnimationEventIntermediary wheelAnimEventRelay;

    public int numberOfSlashes = 2;

    public AttackPayload attackPayload;
    public EntityTeam team = EntityTeam.Player;

    [Header("SFX")]
    [SerializeField] EventReference slashSFX;


    void Awake() 
    {
        wheelAnimEventRelay.OnAnimationEvent += TriggerHitbox;
        slashVFXParent.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(TimedDestruction());
        StartCoroutine(SlashTimer());
    }

    IEnumerator SlashTimer()
    {
        for(int i = 0; i < numberOfSlashes; i++)
        {
            slashVFXParent.transform.localScale = new Vector3(-slashVFXParent.transform.localScale.x , 1 , 1);
            slashVFXParent.SetActive(true);
            yield return new WaitForSeconds(wheelVFX.length);
            slashVFXParent.SetActive(false);
        }
    }

    void TriggerHitbox()
    {
        RuntimeManager.PlayOneShot(slashSFX, transform.position);
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitbox.transform.position, hitbox.size, 0, targetLayer);

        if(hits.Length == 0){return;}
        if(hits == null) { return; }

        foreach(var collider2D in hits) 
        {
            if(collider2D.TryGetComponent<StageEntity>(out StageEntity entity))
            {
                if(entity.CompareTag(TagNames.Enemy.ToString()) || entity.CompareTag(TagNames.EnvironmentalHazard.ToString()))
                {
                    entity.HurtEntity(attackPayload);
                }
            }
        }
    }

    void DisableObject()
    {
        
        if(objectIsPooled)
        {
            gameObject.SetActive(false);
        }else
        {
            Destroy(gameObject);
        }

    }
    
    //Fail-safe for if the projectile does not collide with an appropriate target for too long,
    //destroys the game object after some time.
    private IEnumerator TimedDestruction()
    {
        yield return new WaitForSeconds(wheelVFX.length * numberOfSlashes + 0.05f);
        
        DisableObject();
    }

}
