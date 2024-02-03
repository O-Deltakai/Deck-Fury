using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    class StateNode
    {
        public IState State { get; }
        public HashSet<ITransition> Transitions { get; }

        public StateNode(IState state)
        {
            State = state;
            Transitions = new HashSet<ITransition>();
        }

        public void AddTransition(IState to, IPredicate condition)
        {
            Transitions.Add(new Transition(to, condition));
        }

    }
    /// <summary>
    /// The current state node.
    /// </summary>
    StateNode current;
    Dictionary<Type, StateNode> nodes = new();
    /// <summary>
    /// anyTransition is a transition that doesn't need a "from" state - it can happen any time.
    /// </summary>
    HashSet<ITransition> _anyTransitions = new();

    public void Update()
    {
        var transition = GetTransition();
        if (transition != null)
        {
            ChangeState(transition.To);
        }

        current.State?.Update();
    }

    private void ChangeState(IState state)
    {
        if(state == current.State){ return; }

        var previousState = current.State;
        var nextState = nodes[state.GetType()].State;

        previousState?.OnExit();
        nextState?.OnEnter();
        current = nodes[state.GetType()];

    }

    void SetState(IState state)
    {
        current = nodes[state.GetType()];
        current.State.OnEnter();
    }

    ITransition GetTransition()
    {
        foreach (var transition in _anyTransitions)
        {
            if(transition.Condition.Evaluate())
            {
                return transition;
            }
        }

        foreach (var transition in current.Transitions)
        {
            if(transition.Condition.Evaluate())
            {
                return transition;
            }
        }

        return null;
    }

    public void AddTransition(IState from, IState to, IPredicate condition)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }

    public void AddAnyTransition(IState to, IPredicate condition)
    {
        _anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
    }

    StateNode GetOrAddNode(IState state)
    {
        var node = nodes.GetValueOrDefault(state.GetType());

        if(node == null)
        {
            node = new StateNode(state);
            nodes.Add(state.GetType(), node);
        }

        return node;

    }

    public void FixedUpdate()
    {
        current.State?.FixedUpdate();
    }


}
