using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryGun : NPC
{
    const string SENTRY_MUZZLE_FLASH = "SentryMuzzleFlash";

    //If object is pooled, then the sentry gun will disable itself after some time. Otherwise, it will destroy itself instead.
    public bool ObjectIsPooled = false;
    [SerializeField] GameObject sentryTurret;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject muzzleFlashVFX;
    Animator muzzleFlashAnimator;
    [SerializeField] AnimationClip muzzleFlashAnimation;
    [SerializeField] Transform firepoint;
    public float sentryLifeTime = 10;
    [Range(0.1f, 5)]
    public float sentryFireRate = 1;
    public int sentryDamage = 10;
    [SerializeField] bool CanFire;
    [SerializeField] List<StageEntity> TargetsInRange = new List<StageEntity>();


    protected override void Start()
    {
        base.Start();
        if(!ObjectIsPooled)
        {
            StartCoroutine(LifeTimeCounter());
        }

    }


    private void Update() 
    {
        if(TargetsInRange.Count > 0)
        {
            AimAtClosestTarget();
        }
    }

    private void OnEnable() 
    {
        if(ObjectIsPooled)
        {
            StartCoroutine(LifeTimeCounter());
        }
    }


    void AimAtClosestTarget()
    {
        //Currently just rotates the sentry turret towards the first target that enters its range and doesnt take into account distance
        sentryTurret.transform.right = TargetsInRange[0].currentTilePosition - transform.position;
        
        FireBullet(sentryTurret.transform.right.normalized);
    }

    void FireBullet(Vector3 directionVector)
    {
        if(!CanFire){return;}

        Bullet bullet = Instantiate(projectile, firepoint.position, sentryTurret.transform.rotation).GetComponent<Bullet>();
        Vector2 bulletDirection = new Vector2(directionVector.x, directionVector.y);
        bullet.team = EntityTeam.Player;
        bullet.velocity = bulletDirection;
        bullet.attackPayload.damage = sentryDamage;

        CanFire = false;
        StartCoroutine(FireCooldown());

        muzzleFlashVFX.SetActive(true);
    }

    IEnumerator FireCooldown()
    {
        //Waits for the muzzle flash animation to finish before attempting to fire again
        yield return new WaitForSeconds(muzzleFlashAnimation.length);
        muzzleFlashVFX.SetActive(false);

        //Can go below 0 if sentryFireRate is set too low, therefore hard-cap it at 0.1f so the sentry cant shoot any faster than
        //length of the muzzle flash animation.        
        float cooldownTime = sentryFireRate - muzzleFlashAnimation.length;
        if(cooldownTime <= 0.1)
        {
            cooldownTime = 0.1f;
        }

        yield return new WaitForSeconds(cooldownTime);

        CanFire = true;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        
        StageEntity target = other.attachedRigidbody.gameObject.GetComponent<StageEntity>();
        if(target != null && target.gameObject.tag == "Enemy")
        {
            TargetsInRange.Add(target);
        }    
    }
    
    private void OnTriggerExit2D(Collider2D other) 
    {
        StageEntity target = other.attachedRigidbody.gameObject.GetComponent<StageEntity>();
        if(target != null && target.gameObject.tag == "Enemy")
        {
            TargetsInRange.Remove(target);
        }           
    }

    IEnumerator LifeTimeCounter()
    {
        yield return new WaitForSeconds(sentryLifeTime);
        DisableSelf();
    }

    void DisableSelf()
    {
        if(ObjectIsPooled)
        {gameObject.SetActive(false);}
        else
        {
            Destroy(gameObject);
        }
    }

}
