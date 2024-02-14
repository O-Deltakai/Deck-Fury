using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Wheel : MonoBehaviour
{
    public bool objectIsPooled;
    [SerializeField] AnimationClip wheelVFX;
    [SerializeField] Animator wheelAnimator;
    [SerializeField] GameObject wheelSprite;
    
    Rigidbody2D rigidBody;

    public AttackPayload attackPayload;
    public EntityTeam team = EntityTeam.Player;


    void Awake() 
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        StartCoroutine(TimedDestruction());

    }


    private void OnCollisionEnter2D(Collision2D other)
    {

        if(team == EntityTeam.Player)
        {
            if(other.gameObject.CompareTag("Enemy"))
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(attackPayload);
                //DisableObject();
            }
        }else
        if(team == EntityTeam.Enemy)
        {
            if (other.gameObject.CompareTag("Player")) // Added this
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(attackPayload);
                //DisableObject();
            }
        }else
        {//Neutral team, can damage either player or enemy
            if (other.gameObject.CompareTag("Player")||other.gameObject.CompareTag("Enemy")) // Added this
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(attackPayload);
                //DisableObject();
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
        yield return new WaitForSeconds(wheelVFX.length + 0.05f);
        //Destroy(gameObject);
        
        DisableObject();
    }

}
