using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bazooka : Bullet
{
    public bool objectIsPooled;
    
    [SerializeField] BoxCollider2D bazookaCollider;
    [SerializeField] BoxCollider2D explosionCollider;
    [SerializeField] Animator bombExplosionAnimator;
    [SerializeField] AnimationClip fireBombExplosionVFX;
    [SerializeField] GameObject bazookaBullet;
    [SerializeField] Transform bazookaTransform;

    bool impacted = false;

    //Check for collision with appropriate targets
    //amend position of gameobject on impact
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(!impacted){
            if(team == EntityTeam.Player)
            {
                if(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnvironmentalHazard"))
                {
                    StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                    entity.HurtEntity(attackPayload);
                    bazookaTransform.position = other.gameObject.transform.position;
                    ExplosionImpact();          
                }
            }else
            if(team == EntityTeam.Enemy)
            {
                if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("EnvironmentalHazard")) // Added this
                {
                    StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                    entity.HurtEntity(attackPayload);
                    bazookaTransform.position = other.gameObject.transform.position;
                    ExplosionImpact();
                }
            }
            if(other.gameObject.tag == "Wall")
            {
                ExplosionImpact();
            }
        }

    }

    //activate when rocket hit
    private void ExplosionImpact()
    {
        speed = 0.0f;
        impacted=true;
        //disable rocket itself and make explosion active
        bazookaCollider.enabled = false;
        bazookaBullet.SetActive(false);
        explosionCollider.enabled = true;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        bombExplosionAnimator.Play("FireBombExplosionVFX", 0);
        ActivateExplosionCollider();
    }

    void ActivateExplosionCollider()
    {
        int stageEntitiesLayer = LayerMask.NameToLayer("StageEntities");
        LayerMask stageEntitiesMask = 1 << stageEntitiesLayer;

        Collider2D[] hits = Physics2D.OverlapBoxAll(explosionCollider.transform.position, explosionCollider.size*gameObject.transform.localScale, 0, stageEntitiesMask);
        print("number of hits in bazooka explosion: " + hits.Length);

        foreach(Collider2D collider2D in hits) 
        {
            StageEntity entityHit = collider2D.gameObject.GetComponent<StageEntity>();

            if(entityHit == null)
            {
                print("collider did not have a StageEntity attached");
                continue;
            }


            if(entityHit.CompareTag("Enemy") || entityHit.CompareTag("EnvironmentalHazard"))
            {
                entityHit.HurtEntity(attackPayload);
            }             
        }

        StartCoroutine(ExplosionColliderDuration(0.1f));
    }

    //disable explosion and object
    IEnumerator ExplosionColliderDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        explosionCollider.enabled = false;
        yield return new WaitForSeconds(fireBombExplosionVFX.length - duration);
        DisableObject();
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
