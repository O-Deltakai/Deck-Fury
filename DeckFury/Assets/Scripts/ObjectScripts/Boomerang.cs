using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Boomerang : MonoBehaviour
{
    public bool objectIsPooled;

    [SerializeField] GameObject boomerangSprite;
    
    Rigidbody2D rigidBody;
    public float speed;
    public Vector2 velocity = new Vector2();

    public AttackPayload attackPayload;
    public EntityTeam team = EntityTeam.Player;


    [SerializeField] CircleCollider2D searchCollider;
    [SerializeField] LayerMask targetLayer;

    [Header("Bounce Settings")]
    [Min(1)] public int maxBounces = 2;
    public int damageBonusPerBounce = 25;


    int bounces = 0;

    void Awake() 
    {
        rigidBody = GetComponent<Rigidbody2D>();

    }

    void Start()
    {
        StartCoroutine(TimedDestruction());

    }

    
    private void FixedUpdate() 
    {
        //Move object with given velocity
        rigidBody.MovePosition(rigidBody.position + speed * Time.fixedDeltaTime * velocity);    
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(TagNames.Wall.ToString()) 
            || other.gameObject.CompareTag(TagNames.EnvironmentalHazard.ToString()) 
            || other.gameObject.CompareTag(TagNames.Enemy.ToString()))
        {
            if (other.gameObject.TryGetComponent<StageEntity>(out StageEntity entity))
            {
                entity.HurtEntity(attackPayload);
            }

            if (FindNearestTarget())
            {
                if (bounces < maxBounces)
                {
                    bounces++;
                    attackPayload.damage += damageBonusPerBounce;
                    speed *= 1.1f;
                }
                else
                {
                    DisableObject();
                }
            }
            else
            {
                if (bounces < maxBounces)
                {
                    bounces++;
                    attackPayload.damage += damageBonusPerBounce;

                    // Calculate the new bounce angle
                    Vector2 normal = other.GetContact(0).normal;
                    Vector2 reflection = Vector2.Reflect(velocity, normal);
                    velocity = reflection.normalized;

                    speed *= 1.1f;
                }
                else
                {
                    DisableObject();
                }
            }
        }
    }

    bool FindNearestTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(searchCollider.transform.position, searchCollider.radius, targetLayer);
        if (hits.Length == 0) { return false; }
        if (hits == null) { return false; }

        var sortedEntities = hits.OrderBy(h => -Vector2.Distance(h.transform.position, transform.position)).ToList();

        foreach (var collider2D in sortedEntities)
        {
            if (collider2D.TryGetComponent<StageEntity>(out StageEntity entity))
            {
                if (entity.CompareTag(TagNames.Enemy.ToString()) || entity.CompareTag(TagNames.EnvironmentalHazard.ToString()))
                {
                    Vector2 direction = (collider2D.transform.position - transform.position).normalized;
                    velocity = direction;
                    return true;
                }
            }
        }

        return false;
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
