using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;



public class ShooterFlee : NPC
{

    enum ShooterAnims
    {
        Shooter_Idle = 0,
        Shooter_Defeat = 1,
        Shooter_Aim = 2,
        Shooter_Fire = 3

    }

    public delegate void ReadyToFireEvent();
    public event ReadyToFireEvent OnReadyToFire;

    [SerializeField] Transform firePoint;
    [SerializeField] GameObject projectile;

    [SerializeField] float shootCooldown = 3f;
    public int shooterDamage = 10;
    //public float shooterRate = 1f;
    [SerializeField] bool canAttemptMove = true;
    [SerializeField] bool EnableAI = true;
    [SerializeField] bool CanFire;
    //[SerializeField] float randomMoveDuration = 3f;
    [SerializeField] float movementCooldown;

    //Targeting reticle used to show shooter is preparing to shoot the player
    [SerializeField] GameObject targetingReticle;
    [SerializeField] float aimWindupTime = 0.5f;

    [SerializeField] AttackPayload bulletPayload;

    bool isPreparedToFire;
    bool isAiming;
    Vector2Int playerDistance;

    Coroutine MovementCooldownCoroutine;
    Coroutine FiringCooldownCoroutine;
    Coroutine AimingWindupCoroutine;

    Vector3 fireDirection;

    FleeAI fleeAI;

    protected override void Start()
    {
        base.Start();

        fleeAI = GetComponent<FleeAI>();
        fleeAI.Target = GameManager.Instance.player;

        statusEffectManager.OnStunned += CancelFiring;
        //OnReadyToFire += PrepareToFire;


        targetingReticle.SetActive(false);

        //Start firecooldown right at the start so the shooter doesn't immediately start shooting the player on spawn
        CanFire = false;
        isAiming = false;
        StartCoroutine(FireCooldown(2));

    }


    void Update()
    {
        if (!EnableAI) { return; }
        playerDistance = (Vector2Int)currentTilePosition - (Vector2Int)player.currentTilePosition;

        if (playerDistance.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }


        WanderBehaviour();
        PrepareToFire();

    }

    private void FixedUpdate()
    {
        if (isAiming && !GameManager.GameIsPaused)
        {
            SetTargetingReticleOnPlayer();
        }
    }

    void PrepareToFire()
    {
        if (!CanFire || !CanAct) { return; }
        AimingWindupCoroutine = StartCoroutine(AimingWindupThenFire(aimWindupTime));
    }

    void CancelFiring()
    {
        if (AimingWindupCoroutine != null)
        {
            StopCoroutine(AimingWindupCoroutine);
        }
        entityAnimator.PlayAnimationClip(entityAnimator.animationList[(int)ShooterAnims.Shooter_Idle]);
        isAiming = false;
        targetingReticle.SetActive(false);
        // FiringCooldownCoroutine = StartCoroutine(FireCooldown());
        // if (MovementCooldownCoroutine != null)
        // {
        //     StopCoroutine(MovementCooldownCoroutine);
        // }        
        // MovementCooldownCoroutine = StartCoroutine(MovementCooldown());
    }

    void WanderBehaviour()
    {
        if (!CanInitiateMovementActions) { return; }
        if (!canAttemptMove) { return; }

        canAttemptMove = false;

        if (fleeAI.ShouldFlee())
        {
            Vector3Int? fleeTo = fleeAI.CanFlee();
            if (fleeTo != null)
            {
                // Debug.Log("Fleeing");
                fleeAI.Flee((Vector3Int)fleeTo);
            }
            else
            {
                MoveRandom();
            }
        }
        else
        {
            MoveRandom();
        }
        

        //Prevent multiple movement cooldown coroutines from running at once which produces wacky movement behaviours
        if (MovementCooldownCoroutine != null)
        {
            StopCoroutine(MovementCooldownCoroutine);
        }
        MovementCooldownCoroutine = StartCoroutine(MovementCooldown());
    }


    void SetTargetingReticleOnPlayer()
    {
        targetingReticle.transform.DOLocalRotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360).SetLoops(4, LoopType.Restart)
        .SetEase(Ease.Linear).SetUpdate(false);
        targetingReticle.transform.DOMove(player.currentTilePosition, aimWindupTime);
    }

    IEnumerator AimingWindupThenFire(float duration)
    {
        canAttemptMove = false;//Shooter should not be able to move while aiming
        CanFire = false;
        entityAnimator.PlayOneShotAnimation(entityAnimator.animationList[(int)ShooterAnims.Shooter_Aim]);
        if (MovementCooldownCoroutine != null)
        {
            StopCoroutine(MovementCooldownCoroutine);
        }


        targetingReticle.SetActive(true);
        targetingReticle.transform.localPosition = new Vector3(0, 0, 0);
        isAiming = true;

        yield return new WaitForSeconds(duration);//Wait for aiming to complete before firing a bullet
        entityAnimator.PlayOneShotAnimationReturnIdle(entityAnimator.animationList[(int)ShooterAnims.Shooter_Fire]);
        fireDirection = (player.currentTilePosition - transform.position).normalized;
        Vector3 direction = player.currentTilePosition - transform.position;
        FireBullet(direction.normalized);

        isAiming = false;
        targetingReticle.SetActive(false);


        FiringCooldownCoroutine = StartCoroutine(FireCooldown());
        MovementCooldownCoroutine = StartCoroutine(MovementCooldown());

    }



    //Instantiates a bullet with necessary variables set
    void FireBullet(Vector3 directionVector)
    {

        Bullet bullet = Instantiate(projectile, firePoint.position, transform.rotation).GetComponent<Bullet>();

        //Set bullet variables
        Vector2 bulletVelocity = new Vector2(directionVector.x, directionVector.y);
        bullet.ChangeTrailRendererColor(Color.red);
        bullet.team = EntityTeam.Enemy;
        bullet.velocity = bulletVelocity;
        bullet.speed = 15;
        bullet.trailRenderer.time = 0.2f;
        bullet.attackPayload = bulletPayload;




        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>()); // ensure it doesnt hit themselves

    }

    IEnumerator FireCooldown()
    {
        float randomFloat = UnityEngine.Random.Range(-1f, 1f);
        yield return new WaitForSeconds(shootCooldown + randomFloat);
        CanFire = true;
        //OnReadyToFire?.Invoke();
    }
    IEnumerator FireCooldown(float duration)
    {
        float randomFloat = UnityEngine.Random.Range(-1f, 1f);
        yield return new WaitForSeconds(duration + randomFloat);
        CanFire = true;
        //OnReadyToFire?.Invoke();
    }
    //Move a random tile in the cardinal directions
    void MoveRandom()
    {
        // potential moves
        Vector2Int[] possibleMoves = new Vector2Int[]{
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1)
            };

        Vector2Int chosenMove = possibleMoves[UnityEngine.Random.Range(0, possibleMoves.Length)];

        StartCoroutine(TweenMove(chosenMove.x, chosenMove.y, 0.1f, MovementEase));

    }
    IEnumerator MovementCooldown()
    {
        float randomFloat = UnityEngine.Random.Range(0.5f, 1f);
        yield return new WaitForSeconds(movementCooldown + randomFloat);
        canAttemptMove = true;

    }

    protected override void AdditionalDestructionEvents(AttackPayload? killingBlow = null)
    {
        base.AdditionalDestructionEvents(killingBlow);
        EnableAI = false;
        isAiming = false;
        targetingReticle.SetActive(false);
        CancelFiring();
    }


}