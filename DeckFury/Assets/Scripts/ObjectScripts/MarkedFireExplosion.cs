using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class MarkedFireExplosion : MonoBehaviour
{
    [SerializeField] GameObject explosionParent;
    [SerializeField] List<BoxCollider2D> explosionColliders;
    [SerializeField] LayerMask targetLayer;

    public AttackPayload attackPayload;

    [SerializeField] float lifetime;

    bool _triggered = false;

    [SerializeField] EventReference explosionSFX;

    void Awake()
    {
        if(explosionColliders.Count == 0)
        {
            Debug.LogWarning("Explosion colliders for marked fire explosion is empty, attempting to find them in children.", this);
            foreach (var boxCollider in GetComponentsInChildren<BoxCollider2D>())
            {
                explosionColliders.Add(boxCollider);
            }
        }


    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Trigger(AttackPayload attackPayload)
    {
        if(_triggered) { return; }//Prevent triggering the explosion more than once
        _triggered = true;

        this.attackPayload = attackPayload;

        RuntimeManager.PlayOneShotAttached(explosionSFX, gameObject);

        EnableExplosions();
        ActivateExplosionColliders();
        StartCoroutine(SelfDestructTimer(lifetime));
    }

    void EnableExplosions()
    {
        explosionParent.SetActive(true);
    }

    void ActivateExplosionColliders()
    {

        foreach (var boxCollider in explosionColliders)
        {

            Collider2D[] hits = Physics2D.OverlapBoxAll(boxCollider.transform.position, boxCollider.size, 0, targetLayer);

            foreach(Collider2D collider2D in hits) 
            {
                StageEntity entityHit = collider2D.gameObject.GetComponent<StageEntity>();

                if(entityHit == null)
                {
                    print("collider did not have a StageEntity attached");
                    continue;
                }


                if(entityHit.CompareTag(TagNames.Enemy.ToString()) || entityHit.CompareTag(TagNames.EnvironmentalHazard.ToString()))
                {
                    entityHit.HurtEntity(attackPayload);
                }             
            }
        }

    }

    IEnumerator SelfDestructTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }


}
