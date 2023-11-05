using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FireBombController : MonoBehaviour
{
    StageManager stageManager;
    public AimDirection aimDirection;
    [SerializeField] AnimationClip throwMotionAnimation;
    [SerializeField] AnimationClip fireBombExplosionVFX;
    
    [SerializeField] Transform bombObjectRotation;
    [SerializeField] GameObject bombSprite;
    [SerializeField] GameObject shadowSprite;
    [SerializeField] GameObject bombExplosionSprite;
    [SerializeField] BoxCollider2D bombExplosionCollider;

    Animator bombSpriteAnimator;
    Animator shadowSpriteAnimator;
    [SerializeField] Animator bombExplosionAnimator;
    [SerializeField] AnimationEventIntermediary bombExplosionAnimEventRelay;

    public bool objectIsPooled;
    public Vector2Int impactPoint;
    public AttackPayload attackPayload;

    bool ThrowingInProgress;

    private void Awake() 
    {
        bombSpriteAnimator = bombSprite.GetComponent<Animator>();        
        shadowSpriteAnimator = shadowSprite.GetComponent<Animator>();

        bombObjectRotation.gameObject.SetActive(false);
        bombExplosionCollider.enabled = false;
        bombExplosionCollider.gameObject.SetActive(false);


        bombExplosionAnimEventRelay.OnAnimationEvent += ActivateExplosionCollider;


    }

    void Start()
    {
        stageManager = StageManager.Instance;
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable() 
    {
        BeginBombThrow();   
    }

    void BeginBombThrow()
    {
        if(ThrowingInProgress){return;}

        SetThrowDirection(aimDirection);
        bombObjectRotation.gameObject.SetActive(true);

        //Spin the bomb sprite
        bombSprite.transform.DOLocalRotate(new Vector3(0, 0, 360), 0.25f, RotateMode.FastBeyond360).SetLoops(4, LoopType.Restart)
        .SetEase(Ease.Linear).SetUpdate(false);   

        //Set the throw direction and play the correct motion to move the bomb where it needs to go
        if(aimDirection == AimDirection.Right || aimDirection == AimDirection.Down)
        {
            bombSpriteAnimator.Play("ThrowMotion", 1);
        }else
        {
            bombSpriteAnimator.Play("ThrowMotionInverted", 1);
        }
        shadowSpriteAnimator.Play("ShadowMotion", 2);

        StartCoroutine(DelayBeforeExplosion(throwMotionAnimation.length));


    }

    AimDirection SetThrowDirection(AimDirection aimDirection)
    {
        switch (aimDirection) 
        {
            case AimDirection.Down :
                bombObjectRotation.rotation = Quaternion.Euler(0,0,0);
                impactPoint = new Vector2Int(0, -4);
                return aimDirection;
                
            case AimDirection.Up:
                bombObjectRotation.rotation = Quaternion.Euler(0,0,180);
                impactPoint = new Vector2Int(0, 4);
                return aimDirection;
            
            case AimDirection.Right:
                bombObjectRotation.rotation = Quaternion.Euler(0,0,90);
                impactPoint = new Vector2Int(4, 0);

                return aimDirection;

            case AimDirection.Left:
                bombObjectRotation.rotation = Quaternion.Euler(0,0,-90);
                impactPoint = new Vector2Int(-4, 0);
                return aimDirection;

            default :
                return aimDirection;
        }
    }


    IEnumerator DelayBeforeExplosion(float duration)
    {
        yield return new WaitForSeconds(duration);
        bombObjectRotation.gameObject.SetActive(false);

        bombExplosionCollider.gameObject.SetActive(true);
        MoveExplosionToImpactPoint();
    }

 
    void MoveExplosionToImpactPoint()
    {
        bombExplosionCollider.transform.localPosition = new Vector3(impactPoint.x, impactPoint.y);
        bombExplosionAnimator.Play("FireBombExplosionVFX", 0);
        StartCoroutine(ExplosionVFXDuration());
    }

   //Called by animation event on explosion animation through the AnimationEventIntermediary
    void ActivateExplosionCollider()
    {
        bombExplosionCollider.enabled = true;

        int stageEntitiesLayer = LayerMask.NameToLayer("StageEntities");
        LayerMask stageEntitiesMask = 1 << stageEntitiesLayer;

        Collider2D[] hits = Physics2D.OverlapBoxAll(bombExplosionCollider.transform.position, bombExplosionCollider.size, 0, stageEntitiesMask);
        print("number of hits in fire bomb explosion: " + hits.Length);

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

        StartCoroutine(ExplosionColliderDuration(0.2f));
    }

    IEnumerator ExplosionColliderDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        bombExplosionCollider.enabled = false;

    }

    IEnumerator ExplosionVFXDuration()
    {
        yield return new WaitForSeconds(fireBombExplosionVFX.length);
        bombExplosionCollider.gameObject.SetActive(false);
        ThrowingInProgress = false;
        DisableObject();
    }
    

    private void OnTriggerEnter2D(Collider2D other) 
    {
        // Collider2D[] hits = Physics2D.OverlapBoxAll(bombExplosionCollider.transform.position, bombExplosionCollider.size,0);
        // print("number of hits in fire bomb explosion: " + hits.Length);

        // foreach(Collider2D collider2D in hits) 
        // {
        //     StageEntity entityHit = collider2D.gameObject.GetComponent<StageEntity>();
        //     if(entityHit != null && entityHit.CompareTag("Enemy"))
        //     {
        //         entityHit.HurtEntity(attackPayload);
        //     }             
        // }

  
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

    private void OnDisable() 
    {
        ResetToDefaultState();

    }

    void ResetToDefaultState()
    {

        bombExplosionCollider.enabled = false;
        bombExplosionCollider.gameObject.SetActive(false);
        bombObjectRotation.gameObject.SetActive(false);
        ThrowingInProgress = false;

        bombSprite.transform.position = new Vector3(0,0,0);
        shadowSprite.transform.position = new Vector3(0,0,0);

    }

}
