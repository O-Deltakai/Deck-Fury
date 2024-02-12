using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using DG.Tweening;


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

    [Header("SFX")]
    [SerializeField] EventReference sentryFireSFX;

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
        Vector3 direction = TargetsInRange[0].currentTilePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        sentryTurret.transform.rotation = Quaternion.Slerp(sentryTurret.transform.rotation, targetRotation, 10 * Time.deltaTime);
        FireBullet(direction.normalized);
    }

    void FireBullet(Vector3 directionVector)
    {
        if(!CanFire){return;}
        RuntimeManager.PlayOneShot(sentryFireSFX, transform.position);
        Bullet bullet = Instantiate(projectile, firepoint.position, Quaternion.LookRotation(Vector3.forward, directionVector)).GetComponent<Bullet>();
        Vector2 bulletDirection = new Vector2(directionVector.x, directionVector.y);
        bullet.team = EntityTeam.Player;
        bullet.velocity = bulletDirection;
        bullet.speed = 45;
        bullet.attackPayload.damage = sentryDamage;

        CanFire = false;
        StartCoroutine(FireCooldown());

        muzzleFlashVFX.SetActive(true);
    }

    IEnumerator InitialCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        CanFire = true;
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
