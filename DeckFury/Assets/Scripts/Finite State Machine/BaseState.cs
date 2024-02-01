using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState : IState
{
    public virtual void FixedUpdate(){}
    public virtual void Update(){}
    public virtual void OnEnter(){}
    public virtual void OnExit(){}

}
