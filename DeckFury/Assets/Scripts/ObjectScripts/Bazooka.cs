using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bazooka : Bullet
{
    public bool objectIsPooled;
    
    [SerializeField] BoxCollider2D bazookaCollider;
    [SerializeField] BoxCollider2D explosionCollider;
    [SerializeField] Animator bombExplosionAnimator;
    [SerializeField] AnimationClip fireBombExplosionVFX;
    [SerializeField] GameObject bazookaBullet;
    [SerializeField] Transform bazookaTransform;

    [SerializeField] EventReference explosionSFX;
    [SerializeField] EventReference boostedExplosionSFX;
    [SerializeField] Light2D explosionLight;
    [SerializeField] Light2D trailLight;

    [Header("Camera Shake Settings")]
    [SerializeField] Vector3 cameraShakeVelocity;
    [SerializeField] float cameraShakeDuration;
    CinemachineImpulseSourceHelper cinemachineImpulseSourceHelper;

    public AttackPayload impactPayload;

    [Header("Reflect Settings")]
    [SerializeField] SpriteRenderer bazookaSprite;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material reflectMaterial;

    [Header("Speed Settings")]
    [SerializeField] float maxSpeed = 7.5f;
    [SerializeField] float acceleration = 0.5f;

    protected override void Awake()
    {
        base.Awake();

        cinemachineImpulseSourceHelper = GetComponent<CinemachineImpulseSourceHelper>();

        explosionLight.enabled = false;
    }

    void Start()
    {

    }

    void Update()
    {
        if(speed < maxSpeed)
        {
            speed += acceleration * Time.deltaTime;
        }
    }

    //Check for collision with appropriate targets
    //amend position of gameobject on impact
    private void OnCollisionEnter2D(Collision2D other)
    {


        if(team == EntityTeam.Player)
        {
            if(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnvironmentalHazard"))
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                bazookaCollider.enabled = false;
                entity.HurtEntity(impactPayload);
                bazookaTransform.position = other.gameObject.transform.position;
                ExplosionImpact();          
            }
        }else
        if(team == EntityTeam.Enemy)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("EnvironmentalHazard")) // Added this
            {
                StageEntity entity = other.gameObject.GetComponent<StageEntity>();
                entity.HurtEntity(impactPayload);
                bazookaTransform.position = other.gameObject.transform.position;
                bazookaCollider.enabled = false;
                ExplosionImpact();
            }
        }
        if(other.gameObject.tag == "Wall")
        {
            ExplosionImpact();
        }
        

    }

    //activate when rocket hit
    private void ExplosionImpact()
    {
        speed = 0.0f;
        //disable rocket itself and make explosion active
        bazookaCollider.enabled = false;
        bazookaBullet.SetActive(false);
        explosionCollider.enabled = true;

        if (TryGetComponent<ObjectRotater>(out ObjectRotater rotater))
        {
            rotater.enabled = false;
        }

        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        bombExplosionAnimator.Play(fireBombExplosionVFX.name, 0);


        ActivateExplosionCollider();
        explosionLight.enabled = true;
        trailLight.enabled = false;
    }

    void ActivateExplosionCollider()
    {
        cinemachineImpulseSourceHelper.ShakeCameraRandomCircle(cameraShakeVelocity * SettingsManager.GlobalCameraShakeMultiplier, cameraShakeDuration, 1f);
        RuntimeManager.PlayOneShotAttached(explosionSFX, gameObject);

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

        DOTween.To(() => explosionLight.intensity = 1f, x => explosionLight.intensity = x, 0f, 0.25f).SetUpdate(true).SetEase(Ease.InOutSine);
               


        StartCoroutine(ExplosionColliderDuration(0.1f));
    }

    //disable explosion and object
    IEnumerator ExplosionColliderDuration(float duration)
    {
        yield return new WaitForSeconds(fireBombExplosionVFX.length);
        //yield return new WaitForSeconds(duration);
        explosionCollider.enabled = false;
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

    public override void Reflect(GameObject reflector)
    {
        if(IsReflected)
        {
            return;
        }

        StartCoroutine(FlashSprite());

        IsReflected = true;

        impactPayload.damage *= 2;
        impactPayload.attackElement = AttackElement.Fire;
        impactPayload.statusEffects.Add(new StatusEffect(StatusEffectType.Stunned, 1));

        speed *= 3.5f;

        ObjectRotater objectRotater = gameObject.AddComponent<ObjectRotater>();
        objectRotater.rotationSpeed = 2000;

        explosionSFX = boostedExplosionSFX;

    }

    IEnumerator FlashSprite()
    {
        bazookaSprite.material = reflectMaterial;
        yield return new WaitForSeconds(0.1f);
        bazookaSprite.material = defaultMaterial;
    }


}
