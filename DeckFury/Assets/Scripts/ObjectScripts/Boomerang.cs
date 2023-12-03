using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boomerang : MonoBehaviour
{
    public bool objectIsPooled;
    [SerializeField] AnimationClip boomerangVFX;
    [SerializeField] Animator boomerangAnimator;
    [SerializeField] GameObject boomerangSprite;
    
    Rigidbody2D rigidBody;
    public float speed;
    public Vector2 velocity = new Vector2();

    public AttackPayload attackPayload;
    public EntityTeam team = EntityTeam.Player;

    bool turningBack = false;
    public int damageMultiplier=1;

    void Awake() 
    {
        rigidBody = GetComponent<Rigidbody2D>();
        turningBack = false;
        boomerangAnimator.Play(boomerangVFX.name, 0);
    }

    void Start()
    {
        StartCoroutine(TimedDestruction());

    }

    
    private void OnEnable() 
    {
        //Spin the boomrang sprite
        //boomerangSprite.transform.DOLocalRotate(new Vector3(0, 0, 360), 0.25f, RotateMode.FastBeyond360).SetLoops(20, LoopType.Restart).SetEase(Ease.Linear).SetUpdate(false);   
    }

    
    private void FixedUpdate() 
    {
        //Move object with given velocity
        rigidBody.MovePosition(rigidBody.position + velocity * Time.fixedDeltaTime * speed);    
    }


    private void OnCollisionEnter2D(Collision2D other)
    {

        if(team == EntityTeam.Player)
        {
            if(other.gameObject.CompareTag("Enemy"))
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(attackPayload);
                DisableObject();
            }
        }else
        if(team == EntityTeam.Enemy)
        {
            if (other.gameObject.CompareTag("Player")) // Added this
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(attackPayload);
                DisableObject();
            }
        }else
        {//Neutral team, can damage either player or enemy
            if (other.gameObject.CompareTag("Player")||other.gameObject.CompareTag("Enemy")) // Added this
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(attackPayload);
                DisableObject();
            }                     
        }

        //turning back gimmick when reach wall or obstacle
        if(other.gameObject.tag == "Wall" || other.gameObject.CompareTag("EnvironmentalHazard"))
        {
            if(turningBack){
                DisableObject();
            }
            else{
                turningBack=true;
                velocity.x*=-1;
                velocity.y*=-1;
                attackPayload.damage *=damageMultiplier;
                speed*=1.5f;
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
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

}
