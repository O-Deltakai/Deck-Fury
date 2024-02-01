using System.Collections;
using System.Collections.Generic;
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

    protected override void Awake()
    {
        base.Awake();
        pathTowardsPlayerState = new PathTowardsPlayerState(seekerAI);
        playerInRangePredicate = new FuncPredicate(PlayerInRange);
        attackState = new AttackState();

        //stateMachine = new StateMachine();
        //stateMachine.AddTransition(pathTowardsPlayerState, attackState, playerInRangePredicate );
        //stateMachine.AddAnyTransition(attackState, playerInRangePredicate);


    }

    bool PlayerInRange()
    {
        return Vector2.Distance(transform.position, GameManager.Instance.player.transform.position) <= 1.5f;
    }


}
