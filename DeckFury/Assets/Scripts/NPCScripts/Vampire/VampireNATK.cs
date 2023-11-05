using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireNATK : MonoBehaviour
{
    public bool objectIsPooled;
    
    [SerializeField] BoxCollider2D attackCollider;
    [SerializeField] Animator attackAnimator;
    [SerializeField] AnimationClip vampireNATKVFX;

    public StageEntity masterEntity;

    public float attackDelay;
    
    public AttackPayload attackPayload;


    void Awake() 
    {
        StartCoroutine(ATKStart());
    }

    
    IEnumerator ATKStart(){
        yield return new WaitForSeconds(attackDelay);
        attackAnimator.enabled=true;
        ActivateCollider();
        yield return new WaitForSeconds(vampireNATKVFX.length);
        DisableObject();
    }

    void ActivateCollider()
    {
        attackCollider.enabled = true;
        int stageEntitiesLayer = LayerMask.NameToLayer("StageEntities");
        LayerMask stageEntitiesMask = 1 << stageEntitiesLayer;

        Collider2D[] hits = Physics2D.OverlapBoxAll(attackCollider.transform.position, attackCollider.size*gameObject.transform.localScale, 0, stageEntitiesMask);

        foreach(Collider2D collider2D in hits) 
        {
            StageEntity entityHit = collider2D.gameObject.GetComponent<StageEntity>();
            if(entityHit != null){
                if(entityHit.CompareTag("Player") || entityHit.CompareTag("EnvironmentalHazard"))
                {
                    entityHit.HurtEntity(attackPayload);
                    masterEntity.CurrentHP+=(int)attackPayload.damage/2;
                }       
            }      
        }

        StartCoroutine(ColliderDuration(0.15f));
    }

    //disable explosion and object
    IEnumerator ColliderDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        attackCollider.enabled = false;
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


}
