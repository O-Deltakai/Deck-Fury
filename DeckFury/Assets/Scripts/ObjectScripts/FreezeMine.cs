using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FreezeMine : MonoBehaviour
{
    BoxCollider2D spikesCollider;

    public AttackPayload attackPayload;
    public bool objectIsPooled;

    private void Awake() 
    {
        spikesCollider = GetComponent<BoxCollider2D>();

    }

    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //What happens if an entity walks into the collider of the trap?
    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.CompareTag("Enemy")){
            StageEntity entityHit = other.gameObject.GetComponent<StageEntity>();

            if (entityHit != null)
            {
                entityHit.HurtEntity(attackPayload);
                DisableObject();
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

}
