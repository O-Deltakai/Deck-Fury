using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireBomb : MonoBehaviour
{
    public bool objectIsPooled;
    
    [SerializeField] BoxCollider2D explosionCollider;
    [SerializeField] Animator bombExplosionAnimator;
    [SerializeField] AnimationClip vampireBombExpVFX;

    public float explosionDelay;
    
    public AttackPayload attackPayload;


    void Awake() 
    {
        StartCoroutine(ExplosionStart());
    }

    
    IEnumerator ExplosionStart(){
        yield return new WaitForSeconds(explosionDelay);
        bombExplosionAnimator.enabled=true;
        ActivateExplosionCollider();
        yield return new WaitForSeconds(vampireBombExpVFX.length);
        DisableObject();
    }

    void ActivateExplosionCollider()
    {
        StartCoroutine(ExplosionColliderDuration(vampireBombExpVFX.length/4));
        explosionCollider.enabled = true;
        int stageEntitiesLayer = LayerMask.NameToLayer("StageEntities");
        LayerMask stageEntitiesMask = 1 << stageEntitiesLayer;

        Collider2D[] hits = Physics2D.OverlapBoxAll(explosionCollider.transform.position, explosionCollider.size*gameObject.transform.localScale, 0, stageEntitiesMask);

        foreach(Collider2D collider2D in hits) 
        {
            StageEntity entityHit = collider2D.gameObject.GetComponent<StageEntity>();
            if(entityHit != null){
                if(entityHit.CompareTag("Player") || entityHit.CompareTag("EnvironmentalHazard"))
                {
                    entityHit.HurtEntity(attackPayload);
                }      
            }       
        }

        StartCoroutine(ExplosionColliderDuration(vampireBombExpVFX.length/2));
    }

    //disable explosion and object
    IEnumerator ExplosionColliderDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        explosionCollider.enabled = false;
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
