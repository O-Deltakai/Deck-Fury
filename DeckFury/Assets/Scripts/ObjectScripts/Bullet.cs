using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityUtils;

public class Bullet : MonoBehaviour, IReflectable
{

    public Action OnImpact;

    public EntityTeam team = EntityTeam.Player;
    Rigidbody2D rigidBody;
    public float speed = 50;
    public Vector2 velocity = new Vector2();

    public AttackPayload attackPayload;

    public TrailRenderer trailRenderer;

    bool canGoThroughWalls = true;

    [SerializeField] Color bulletColor;
    [SerializeField] Light2D bulletLight;

    public bool IsReflected { get; set; } = false;

    protected virtual void Awake() 
    {
        rigidBody = GetComponent<Rigidbody2D>();    
        trailRenderer = GetComponent<TrailRenderer>();
        if(!bulletLight)
        {
            bulletLight = GetComponent<Light2D>();
        }
  
        if(bulletLight)
        {
            bulletLight.color = bulletColor;
        }
    }

    void Start()
    {
        StartCoroutine(GoThroughWallsTimer()); 
        StartCoroutine(TimedDestruction());

    }

    private void FixedUpdate() 
    {
        //Move bullet with given velocity
        rigidBody.MovePosition(rigidBody.position + speed * Time.fixedDeltaTime * velocity);    
    }


    public void ChangeTrailRendererColor(Color color)
    {
        trailRenderer.startColor = color;
      
        trailRenderer.endColor = Color.white;
        if(bulletLight) bulletLight.color = color;
    }


    //Check for collision with appropriate targets
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        

        if(team == EntityTeam.Player)
        {
            if(other.gameObject.CompareTag(TagNames.Enemy) || other.gameObject.CompareTag(TagNames.EnvironmentalHazard))
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(attackPayload);
                OnImpact?.Invoke();
                Destroy(gameObject);            
            }
        }else
        if(team == EntityTeam.Enemy)
        {
            if (other.gameObject.CompareTag(TagNames.Player) || other.gameObject.CompareTag(TagNames.EnvironmentalHazard)) // Added this
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(attackPayload);
                OnImpact?.Invoke();
                Destroy(gameObject);            

            }
        }else
        {//Neutral team, can damage either player or enemy
            if (other.gameObject.CompareTag(TagNames.Player) || other.gameObject.CompareTag(TagNames.EnvironmentalHazard)) // Added this
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(attackPayload);
                OnImpact?.Invoke();
                Destroy(gameObject);            

            }
            if(other.gameObject.CompareTag(TagNames.Enemy) || other.gameObject.CompareTag(TagNames.EnvironmentalHazard))
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(attackPayload);
                OnImpact?.Invoke();
                Destroy(gameObject);          

            }                        
        }



        if(other.gameObject.CompareTag(TagNames.Wall) && !canGoThroughWalls)
        {
            OnImpact?.Invoke();
            Destroy(gameObject);
        }

    }

    //Timer such that if a bullet spawns on top of a wall, it doesnt get immediately destroyed
    IEnumerator GoThroughWallsTimer()
    {
        yield return new WaitForEndOfFrame();
        canGoThroughWalls = false;
        

    }


    //Fail-safe for if the projectile does not collide with an appropriate target for too long,
    //destroys the game object after some time.
    private IEnumerator TimedDestruction()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }

    public virtual void Reflect(GameObject reflector)
    {
        if(IsReflected || team == EntityTeam.Player)
        {
            return;
        }

        IsReflected = true;
        velocity = -velocity * 2.5f;
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 180);

        attackPayload.damage *= 5;
        attackPayload.attacker = reflector;

        attackPayload.reflected = true;
        team = EntityTeam.Player;
        ChangeTrailRendererColor(Color.cyan);
    }
}
