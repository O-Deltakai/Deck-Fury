using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using FMODUnity;
using UnityEngine;

public class ShieldGuy : NPC
{

#region States

    class ShieldsUpState : BaseState
    {

    }

    class PathTowardsPlayerState : BaseState
    {
        SeekerAI seekerAI;

        public PathTowardsPlayerState(SeekerAI seekerAI)
        {
            this.seekerAI = seekerAI;
        }

    }

    class IdleState : BaseState
    {

    }

    class AttackState : BaseState
    {

    }

    #endregion


    SeekerAI seekerAI;
    StateMachine stateMachine;
    PathTowardsPlayerState pathTowardsPlayerState;
    AttackState attackState;
    FuncPredicate playerInRangePredicate;

    [Header("Shield Bearer Settings")]
    [SerializeField] PlayerInRangeChecker playerInRangeChecker;


    bool _isAttacking;

    [Header("Shield Settings")]
    [SerializeField] bool _shieldsUp;
    [SerializeField] GameObject shieldHitboxObject;
    [SerializeField] Vector3 normalShieldScale;
    [SerializeField] Vector3 attackingShieldScale;
    [SerializeField] Color shieldColor;
    [SerializeField] Color hitShieldColor;
    [SerializeField, Tooltip("Color of the shield when attacking.")] Color shieldAttackColor;
    [SerializeField] Ease shieldAnimationEase;
    [SerializeField] float shieldAttackVelocity = 0.15f;

    [SerializeField] Ease shieldFlashEase;
    [SerializeField] float shieldFlashDuration = 0.1f;
    [SerializeField] Collider2D shieldCollider;
    [SerializeField] LayerMask shieldLayer;

    [Header("Attack Settings")]
    [SerializeField] float prepareAttackTime = 1f;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] BoxCollider2D attackHitbox;
    [SerializeField] GameObject attackHitboxAnchor;
    [SerializeField] LayerMask attackLayer;
    CinemachineImpulseSource impulseSource;

    [Header("SFX")]
    [SerializeField] EventReference shieldBashSFX;
    [SerializeField] EventReference hitShieldSFX;

    Coroutine CR_PrepareAttack;
    Coroutine CR_ShieldBash;

    AimDirection currentAimDirection;

    [SerializeField] bool faceTowardsPlayerTest;

    [SerializeField] bool preparingAttack;

    protected override void Awake()
    {
        base.Awake();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        pathTowardsPlayerState = new PathTowardsPlayerState(seekerAI);
        //playerInRangePredicate = new FuncPredicate(PlayerInRange);
        attackState = new AttackState();

        //stateMachine = new StateMachine();
        //stateMachine.AddTransition(pathTowardsPlayerState, attackState, playerInRangePredicate );
        //stateMachine.AddAnyTransition(attackState, playerInRangePredicate);


    }

    protected override void Start()
    {
        base.Start();
        seekerAI = GetComponent<SeekerAI>();
        seekerAI.Target = GameManager.Instance.player;

        ShieldsUp();
    }

    void Update()
    {
        if(playerInRangeChecker.InRange)
        {
            if(CR_PrepareAttack == null)
            {
                FaceTowardsPlayer();
                CR_PrepareAttack = StartCoroutine(PrepareAttackCoroutine());
            }
        }

        if(faceTowardsPlayerTest && !preparingAttack)
        {
            FaceTowardsPlayer();
        }


    }

    IEnumerator PrepareAttackCoroutine()
    {
        preparingAttack = true;
        AbilityData shieldBash = NPCAbilities[0];

        Vector2Int playerDistance = (Vector2Int)GameManager.Instance.player.currentTilePosition - (Vector2Int)currentTilePosition;
        AimDirection aimDirection = CardinalAimSystem.GetClosestAimDirectionByVector(playerDistance);
        currentAimDirection = aimDirection;

        _stageManager.SetWarningTiles(CardinalAimSystem.AnchoredAimTowardsDirection
        (aimDirection, shieldBash.rangeOfInfluence, currentTilePosition), prepareAttackTime);
        FaceTowardsPlayer();

        yield return new WaitForSeconds(prepareAttackTime);


        InitiateAttack();
        yield return new WaitForSeconds(attackCooldown);
        preparingAttack = false;
        CR_PrepareAttack = null;
        ShieldsUp();
    }

    void PrepareAttack()
    {

    }

    void InitiateAttack()
    {
        entityAnimator.PlayOneShotAnimationReturnIdle(entityAnimator.animationList[0]);
        
    }

    void ShieldsUp()
    {
        _shieldsUp = true;
        shieldCollider.enabled = true;
        entityAnimator.PlayOneShotAnimation(entityAnimator.animationList[4]);
        shieldHitboxObject.transform.localScale = normalShieldScale;
        shieldHitboxObject.GetComponent<SpriteRenderer>().color = shieldColor;
        shieldHitboxObject.transform.localPosition = Vector3.zero;

        //Armor = 100;       
    }

    void ShieldsDown()
    {
        _shieldsUp = false;
        Armor = EnemyData.Armor;
        entityAnimator.PlayOneShotAnimation(entityAnimator.animationList[2]);
    }

    void TriggerShieldBashAnimation()
    {

        CR_ShieldBash = StartCoroutine(ShieldBashCoroutine());
    }

    protected override AttackPayload PrefixDamageCalculations(AttackPayload payload)
    {
        if(_shieldsUp)
        {
            if(CheckShieldBlock(payload))
            {
                StartCoroutine(FlashShieldCoroutine());
                payload.damage = 0;
            }
        }
        return payload;
    }

    bool CheckShieldBlock(AttackPayload payload)
    {
        if(payload.attacker != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(payload.attacker.transform.position, (worldTransform.position - payload.attacker.transform.position).normalized, Mathf.Infinity, shieldLayer);

            // Visualize the raycast
            if (hit.collider != null)
            {
                // If the raycast hit a collider, draw a red line to the hit point
                Debug.DrawLine(payload.attacker.transform.position, hit.point, Color.red);
                Debug.Log("Raycast hit a collider on the target layer: " + hit.collider.gameObject.name);
            }
            else
            {
                // If the raycast did not hit, draw a green line for the length of the ray
                Debug.DrawLine(payload.attacker.transform.position, (worldTransform.position - payload.attacker.transform.position).normalized * 20, Color.green);
            }

            if(hit.collider == shieldCollider)
            {
                print("Attack hit shield");
                return true;
            }else
            {
                print("Attack did not hit shield");
            }
        }

        return false;
    }

    public override void HurtEntity(AttackPayload payload, Color? hitFlashColor = null, EventReference? hitSFX = null)
    {
        if(_shieldsUp && CheckShieldBlock(payload))
        {
            hitSFX = hitShieldSFX;
        }

        base.HurtEntity(payload, hitFlashColor, hitSFX);
    }


    IEnumerator FlashShieldCoroutine()
    {
        yield return shieldHitboxObject.GetComponent<SpriteRenderer>().DOColor(hitShieldColor, shieldFlashDuration).SetEase(shieldFlashEase).WaitForCompletion();
        yield return shieldHitboxObject.GetComponent<SpriteRenderer>().DOColor(shieldColor, shieldFlashDuration).SetEase(shieldFlashEase).WaitForCompletion();


    }

    IEnumerator ShieldBashCoroutine()
    {
        shieldCollider.enabled = false;
        _shieldsUp = false;

        TriggerAttackHitbox();

        shieldHitboxObject.transform.localScale = normalShieldScale;
        shieldHitboxObject.GetComponent<SpriteRenderer>().color = shieldColor;
        shieldHitboxObject.transform.localPosition = Vector3.zero;



        shieldHitboxObject.transform.DOScale(attackingShieldScale, shieldAttackVelocity).SetEase(shieldAnimationEase);
        shieldHitboxObject.GetComponent<SpriteRenderer>().DOColor(shieldAttackColor, shieldAttackVelocity * 0.75f).SetEase(Ease.Linear);
        shieldHitboxObject.transform.DOLocalMove(CardinalAimSystem.GetVector3WithAimDirection(currentAimDirection), shieldAttackVelocity).SetEase(shieldAnimationEase);
        shieldHitboxObject.GetComponent<SpriteRenderer>().DOFade(0, shieldAttackVelocity).SetEase(Ease.Linear);

        yield return new WaitForSeconds(shieldAttackVelocity);

    }

    void FaceTowardsPlayer()
    {
        Vector2Int playerDistance = (Vector2Int)GameManager.Instance.player.currentTilePosition - (Vector2Int)currentTilePosition;
        AimDirection aimDirection = CardinalAimSystem.GetClosestAimDirectionByVector(playerDistance);

        if(currentAimDirection == aimDirection) { return; }

        shieldHitboxObject.transform.DORotate(CardinalAimSystem.GetRotationWithAimDirection(aimDirection).eulerAngles, 0.1f).SetEase(Ease.InOutSine);
        attackHitboxAnchor.transform.rotation = CardinalAimSystem.GetRotationWithAimDirection(aimDirection);

        currentAimDirection = aimDirection;

    }

    void TriggerAttackHitbox()
    {
        impulseSource.GenerateImpulseWithVelocity(CardinalAimSystem.GetVector3WithAimDirection(currentAimDirection) * 0.1f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(attackHitbox.transform.position, attackHitbox.size, attackHitboxAnchor.transform.eulerAngles.z, attackLayer);
        
        var sortedEntities = hits.OrderBy(h => -Vector2.Distance(h.transform.position, transform.position)).ToList();


        foreach(var collider2D in sortedEntities) 
        {
            StageEntity entity;
            if(collider2D.attachedRigidbody != null)
            {
                if(!collider2D.attachedRigidbody.gameObject.TryGetComponent<StageEntity>(out entity))
                {
                    print("collider did not have a StageEntity attached");
                    continue;
                }

            }else
            {
                collider2D.gameObject.TryGetComponent<StageEntity>(out entity);
            }
            //if(!entity.CompareTag("Enemy") || !entity.CompareTag("EnvironmentalHazard")){continue;}
            if(entity == null)
            {
                continue;
            }else
            {
                if (entity.CompareTag(TagNames.Player.ToString()) || entity.CompareTag(TagNames.EnvironmentalHazard.ToString()))
                {
                    entity.HurtEntity(NPCAbilities[0].attackPayload);
                    Vector2Int shoveDirection = Vector2Int.RoundToInt(CardinalAimSystem.GetVector3WithAimDirection(currentAimDirection)) * 2;
                    entity.AttemptMovement(shoveDirection.x, shoveDirection.y, 0.15f, Ease.OutQuart, ForceMoveMode.Forward);    
                }
            }



        }


        // foreach(Collider2D collider2D in hits) 
        // {
        //     StageEntity entityHit = collider2D.gameObject.GetComponent<StageEntity>();

        //     if (entityHit == null)
        //     {
        //         print("collider did not have a StageEntity attached");
        //         continue;
        //     }

        //     //impulseSource.GenerateImpulse();

        //     if (entityHit.CompareTag(TagNames.Player.ToString()) || entityHit.CompareTag(TagNames.EnvironmentalHazard.ToString()))
        //     {
        //         entityHit.HurtEntity(NPCAbilities[0].attackPayload);
        //         Vector2Int shoveDirection = Vector2Int.RoundToInt(CardinalAimSystem.GetVector3WithAimDirection(currentAimDirection)) * 2;
        //         entityHit.AttemptMovement(shoveDirection.x, shoveDirection.y, 0.15f, Ease.OutQuart, ForceMoveMode.Forward);    
        //     }
        // }
           
    }



}
