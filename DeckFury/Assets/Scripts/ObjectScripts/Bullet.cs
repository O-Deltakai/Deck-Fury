using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bullet : MonoBehaviour
{

    public EntityTeam team = EntityTeam.Player;
    Rigidbody2D rigidBody;
    public float speed = 50;
    public Vector2 velocity = new Vector2();

    public AttackPayload attackPayload;

    public TrailRenderer trailRenderer;

    bool canGoThroughWalls = true;

    [SerializeField] Color bulletColor;
    Light2D bulletLight;

    protected virtual void Awake() 
    {
        rigidBody = GetComponent<Rigidbody2D>();    
        trailRenderer = GetComponent<TrailRenderer>();
        bulletLight = GetComponent<Light2D>();
  
        bulletLight.color = bulletColor;
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

    void Update()
    {
        //rigidBody.MovePosition(rigidBody.position + velocity * Time.deltaTime * speed);    

    }

    public void ChangeTrailRendererColor(Color color)
    {
        trailRenderer.startColor = color;
      
        trailRenderer.endColor = Color.white;
        bulletLight.color = color;
    }


    //Check for collision with appropriate targets
    private void OnCollisionEnter2D(Collision2D other)
    {

        if(team == EntityTeam.Player)
        {
            if(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnvironmentalHazard"))
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(attackPayload);
                Destroy(gameObject);            
            }
        }else
        if(team == EntityTeam.Enemy)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("EnvironmentalHazard")) // Added this
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(attackPayload);
                Destroy(gameObject);            

            }
        }else
        {//Neutral team, can damage either player or enemy
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("EnvironmentalHazard")) // Added this
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(attackPayload);
                Destroy(gameObject);            

            }
            if(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnvironmentalHazard"))
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(attackPayload);
                Destroy(gameObject);          

            }                        
        }



        if(other.gameObject.tag == "Wall" && !canGoThroughWalls)
        {
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
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }


}
