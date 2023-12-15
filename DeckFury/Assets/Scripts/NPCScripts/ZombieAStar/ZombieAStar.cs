using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAStar : NPC
{
    [SerializeField] ZombieBasicAttack BasicAttack;
    [SerializeField] float attackCooldown;
    [SerializeField] float movementCooldown;

    [SerializeField] bool EnableAI = true;
    [SerializeField] bool canAttemptMove = true;
    [SerializeField] bool canAttack = true;
    [SerializeField] bool isAttacking = false;

    [SerializeField] bool triggerAttack;

    Vector2Int playerDistance;

    Coroutine attackWindUpCoroutine = null;

    [SerializeField] float attackWindupDuration = 0.75f;

    SeekerAI seekerAI;

    protected override void Awake()
    {
        base.Awake();
        seekerAI = GetComponent<SeekerAI>();
        statusEffectManager.OnStunned += CancelAttack;
    }

    protected override void Start()
    {
        base.Start();
        BasicAttack.abilityData = NPCAbilities[0];
        seekerAI.Target = GameManager.Instance.player;
    }

    void Update()
    {
        if(triggerAttack)
        {
            TriggerBasicAttack();
            triggerAttack = false;
        }

        if(!EnableAI){return;}

        if(attackWindUpCoroutine == null)
        {
            FaceTowardsPlayer();
        }

        if(canAttack)
        {
            AttackPlayer();
        }
    }

    void FaceTowardsPlayer()
    {
        playerDistance = (Vector2Int)currentTilePosition - (Vector2Int)player.currentTilePosition;

        if(playerDistance.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }


    protected override void AdditionalDestructionEvents(AttackPayload? killingBlow)
    {
        base.AdditionalDestructionEvents(killingBlow);
        if(attackWindUpCoroutine != null)
        {
            StopCoroutine(attackWindUpCoroutine);
        }

        BasicAttack.gameObject.SetActive(false);
        EnableAI = false;
        seekerAI.pauseAI = true;
    }

    void MoveTowardsPlayer()
    {
        if(!CanInitiateMovementActions){return;}
        if(!canAttemptMove||isAttacking)
        {return;}
        canAttemptMove = false;

        if(playerDistance.x > 0 && Math.Abs(playerDistance.x) > Math.Abs(playerDistance.y))
        {
            StartCoroutine(TweenMove(-1, 0, 0.1f, MovementEase));
        }else
        if(playerDistance.x < 0 && Math.Abs(playerDistance.x) > Math.Abs(playerDistance.y))
        {
            StartCoroutine(TweenMove(1, 0, 0.1f, MovementEase));
        }else
        if(playerDistance.y > 0 && Math.Abs(playerDistance.x) < Math.Abs(playerDistance.y))
        {
            StartCoroutine(TweenMove(0, -1, 0.1f, MovementEase));
        }else
        if(playerDistance.y < 0 && Math.Abs(playerDistance.x) < Math.Abs(playerDistance.y))
        {
            StartCoroutine(TweenMove(0, 1, 0.1f, MovementEase));
        }else
        {
            int randomMoveInt = UnityEngine.Random.Range(0, 2);
            int randomInt = UnityEngine.Random.Range(-1, 2);
            if(randomInt == 0){randomInt++;}

            if(randomMoveInt == 0)
            {
                StartCoroutine(TweenMove(0, randomInt, 0.1f, MovementEase));
            }else
            {
                StartCoroutine(TweenMove(randomInt, 0, 0.1f, MovementEase));
            }

        }

        StartCoroutine(MovementCooldown());
        //print(playerDistance);


    }

    //Check if zombie is directly adjacent to player, then triggers the basic attack
    void AttackPlayer()
    {
        if(!CanAct){return;}

        if(playerDistance.x == 1 && playerDistance.y == 0)
        {
            TriggerBasicAttack(AimDirection.Left);
        }else
        if(playerDistance.x == -1 && playerDistance.y == 0)
        {
            TriggerBasicAttack(AimDirection.Right);
        }else
        if(playerDistance.y == 1 && playerDistance.x == 0)
        {
            TriggerBasicAttack(AimDirection.Down);
        }else
        if(playerDistance.y == -1 && playerDistance.x == 0)
        {
            TriggerBasicAttack(AimDirection.Up);
        }



    }


    public void TriggerBasicAttack(AimDirection aimDirection = AimDirection.Left)
    {
        if(!canAttack){return;}
        if(!CanAct){ return; }
        if(attackWindUpCoroutine != null){ return; }
        canAttack = false;
        isAttacking = true;
        seekerAI.pauseAI = true;
        

        BasicAttack.FaceTowardsAimpoint(aimDirection);
        
        _stageManager.SetVFXTile(_stageManager.DangerVFXTile,
                                BasicAttack.abilityData.AnchorRangeOfInfluenceToTilePosition(aimDirection, currentTilePosition), attackWindupDuration);

        attackWindUpCoroutine = StartCoroutine(AttackWindup());


    }

    IEnumerator AttackWindup()
    {
        yield return new WaitForSeconds(attackWindupDuration);
        entityAnimator.PlayAnimationClip(BasicAttack.abilityData.animationToUse);
        yield return new WaitForSeconds(BasicAttack.abilityData.animationToUse.length - 0.125f);
        //BasicAttack.gameObject.SetActive(true);

        yield return new WaitForSeconds(attackCooldown + BasicAttack.abilityData.animationToUse.length);
        //canAttack = true;
        isAttacking = false;
        
   
        attackWindUpCoroutine = null;
    }

    void CancelAttack()
    {
        if(attackWindUpCoroutine != null)
        {
            StopCoroutine(attackWindUpCoroutine);
            canAttack = true;
            isAttacking = false;
            seekerAI.pauseAI = false;


            attackWindUpCoroutine = null;
        }
    }

    //Used by animation event
    void ActivateAttackHitbox()
    {
        BasicAttack.gameObject.SetActive(true);
        attackWindUpCoroutine = null;
    }
    //Used by animation event
    void TriggerCooldown()
    {
        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        isAttacking = false;        
        seekerAI.pauseAI = false;
    }

    IEnumerator MovementCooldown()
    {
        float randomFloat = UnityEngine.Random.Range(0.2f, 1f);
        yield return new WaitForSeconds(movementCooldown + randomFloat);
        canAttemptMove = true;
    }

}
