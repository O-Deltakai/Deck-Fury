using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(SeekerAI))]
[RequireComponent(typeof(NPC))]
public class ShieldBearerController : MonoBehaviour
{
    SeekerAI seekerAI;

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

    [SerializeField] GameObject shieldBlockHitVFX;


    [Header("Attack Settings")]
    [SerializeField] float prepareAttackTime = 1f;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] BoxCollider2D attackHitbox;
    [SerializeField] GameObject attackHitboxAnchor;
    [SerializeField] LayerMask attackLayer;
    CinemachineImpulseSource impulseSource;

    [SerializeField] List<AbilityData> NPCAbilities;


    [Header("SFX")]
    [SerializeField] EventReference shieldBashSFX;
    [SerializeField] EventReference hitShieldSFX;

    Coroutine CR_PrepareAttack;
    Coroutine CR_ShieldBash;
    Coroutine CR_ShieldBlockHitVFX;

    AimDirection currentAimDirection;

    [SerializeField] bool faceTowardsPlayerTest;

    [SerializeField] bool preparingAttack;

    StageManager stageManager;

    float updateRate = 0.1f;
    float timer = 0;

    NPC npc;

    void Awake()
    {
        seekerAI = GetComponent<SeekerAI>();
        npc = GetComponent<NPC>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        stageManager = StageManager.Instance;
    }


    // Start is called before the first frame update
    void Start()
    {
        seekerAI = GetComponent<SeekerAI>();
        seekerAI.Target = GameManager.Instance.player;
        npc.StatusEffectManager.OnStunned += OnStunned;
        ShieldsUp();        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > updateRate)
        {
            timer = 0;
        }else
        {
            return;
        }

        //if(!CanAct) { return; }



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
            if(!_shieldsUp)
            {
                ShieldsUp();
            }
            FaceTowardsPlayer();
        }

        
    }

    IEnumerator PrepareAttackCoroutine()
    {
        ShieldsUp();
        seekerAI.pauseAI = true;
        preparingAttack = true;
        AbilityData shieldBash = NPCAbilities[0];

        Vector2Int playerDistance = (Vector2Int)GameManager.Instance.player.currentTilePosition - (Vector2Int)npc.currentTilePosition;
        AimDirection aimDirection = CardinalAimSystem.GetClosestAimDirectionByVector(playerDistance);
        currentAimDirection = aimDirection;

        stageManager.SetWarningTiles(CardinalAimSystem.AnchoredAimTowardsDirection
        (aimDirection, shieldBash.rangeOfInfluence, npc.currentTilePosition), prepareAttackTime);
        FaceTowardsPlayer();

        yield return new WaitForSeconds(prepareAttackTime);


        InitiateAttack();
        yield return new WaitForSeconds(attackCooldown);
        ShieldsUp();

        yield return new WaitForSeconds(attackCooldown * 0.5f);

        preparingAttack = false;
        CR_PrepareAttack = null;
        seekerAI.pauseAI = false;

    }

    void InitiateAttack()
    {
        npc.EntityAnimator.PlayOneShotAnimationReturnIdle(npc.EntityAnimator.animationList[0]);
        
    }


    void OnStunned()
    {
        if(CR_PrepareAttack != null)
        {
            StopCoroutine(CR_PrepareAttack);
            CR_PrepareAttack = null;
        }
        seekerAI.pauseAI = false;
        ShieldsDown();
    }



    void ShieldsUp()
    {
        shieldHitboxObject.SetActive(true);
        _shieldsUp = true;
        shieldCollider.enabled = true;
        npc.EntityAnimator.PlayOneShotAnimation(npc.EntityAnimator.animationList[4]);
        shieldHitboxObject.transform.localScale = normalShieldScale;
        shieldHitboxObject.GetComponent<SpriteRenderer>().DOColor(shieldColor, 0.1f).SetEase(Ease.InOutSine);
        shieldHitboxObject.transform.DOLocalMove(Vector3.zero, 0.1f).SetEase(Ease.InOutSine);
    
    }

    void ShieldsDown()
    {
        _shieldsUp = false;
        shieldCollider.enabled = false;
        preparingAttack = false;

        npc.EntityAnimator.PlayOneShotAnimation(npc.EntityAnimator.animationList[2]);

        shieldHitboxObject.GetComponent<SpriteRenderer>().DOFade(0, 0.1f).SetEase(Ease.InOutSine);
        shieldHitboxObject.SetActive(false);

    }





    void InitializeDamageBuilder()
    {
        //Create a new damage builder
        EntityDamageBuilder damageBuilder = new EntityDamageBuilder();
        //Create a new damage context
        DamageContext context = new DamageContext();

        context.actionHandlers.Add(PrefixDamageAction);


        damageBuilder.AddPreDamageActions(context);
    }

    void PrefixDamageAction(EntityDamageBuilder damageBuilder, StageEntity _, ref AttackPayload? payload)
    {
        if(payload.Value.attackElement == AttackElement.Pure)
        {
            return;
        }

        if (_shieldsUp)
        {
            if (CheckShieldBlock(payload.Value))
            {
                if (!payload.Value.statusEffects.Where(se => se.statusEffectType == StatusEffectType.Stunned).Any())
                {
                    StartCoroutine(FlashShieldCoroutine());
                }
                BlockHitVFX();

                var modifiedPayload = payload.Value;
                modifiedPayload.damage = 0;
                payload = modifiedPayload;

                damageBuilder.SetHitSFX(hitShieldSFX);
            }
        }

    }    

    /// <summary>
    /// Checks if an attack is blocked by looking at the attacker of the payload and then casting a ray from the attacker to the shield layer. If the shield is hit, the attack is blocked.
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    bool CheckShieldBlock(AttackPayload payload)
    {
        if(payload.attacker != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(payload.attacker.transform.position, (npc.worldTransform.position - payload.attacker.transform.position).normalized, Mathf.Infinity, shieldLayer);

            if(hit.collider == shieldCollider)
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator FlashShieldCoroutine()
    {
        yield return shieldHitboxObject.GetComponent<SpriteRenderer>().DOColor(hitShieldColor, shieldFlashDuration).SetEase(shieldFlashEase).WaitForCompletion();

        yield return shieldHitboxObject.GetComponent<SpriteRenderer>().DOColor(shieldColor, shieldFlashDuration).SetEase(shieldFlashEase).WaitForCompletion();
    }

    /// <summary>
    /// Used to play the vfx when the shield blocks an attack
    /// </summary>
    void BlockHitVFX()
    {
        if(CR_ShieldBlockHitVFX != null)
        {
            StopCoroutine(CR_ShieldBlockHitVFX);
            shieldBlockHitVFX.SetActive(false);
        }
        CR_ShieldBlockHitVFX = StartCoroutine(BlockHitVFXCoroutine());
    }

    IEnumerator BlockHitVFXCoroutine()
    {
        shieldBlockHitVFX.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        shieldBlockHitVFX.SetActive(false);
        CR_ShieldBlockHitVFX = null;
    }


    void FaceTowardsPlayer()
    {
        Vector2Int playerDistance = (Vector2Int)GameManager.Instance.player.currentTilePosition - (Vector2Int)npc.currentTilePosition;
        AimDirection aimDirection = CardinalAimSystem.GetClosestAimDirectionByVector(playerDistance);

        if(Time.timeScale != 0)
        {
            if(currentAimDirection == AimDirection.Right) {transform.localScale = new Vector3(1, 1, 1);}
            else if(currentAimDirection == AimDirection.Left) {transform.localScale = new Vector3(-1, 1, 1);}
        }


        if(currentAimDirection == aimDirection) { return; }

        shieldHitboxObject.transform.DORotate(CardinalAimSystem.GetRotationWithAimDirection(aimDirection).eulerAngles, 0.1f).SetEase(Ease.InOutSine);
        attackHitboxAnchor.transform.rotation = CardinalAimSystem.GetRotationWithAimDirection(aimDirection);

        currentAimDirection = aimDirection;

    }

    void TriggerAttackHitbox()
    {
        impulseSource.GenerateImpulseWithVelocity(0.1f * SettingsManager.GlobalCameraShakeMultiplier * CardinalAimSystem.GetVector3WithAimDirection(currentAimDirection));

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

            if(entity == null)
            {
                continue;
            }else
            {
                if (entity.CompareTag(TagNames.Player.ToString()) || entity.CompareTag(TagNames.EnvironmentalHazard.ToString()))
                {
                    //entity.HurtEntity(NPCAbilities[0].attackPayload);
                    Vector2Int shoveDirection = Vector2Int.RoundToInt(CardinalAimSystem.GetVector3WithAimDirection(currentAimDirection)) * 2;
                    entity.AttemptMovement(shoveDirection.x, shoveDirection.y, 0.15f, Ease.OutQuart, ForceMoveMode.Forward);    
                }
            }



        }
           
    }



}
