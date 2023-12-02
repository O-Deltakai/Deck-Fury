using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public delegate void ActionHandler();
public struct BufferedInput
{
    // the delegate that represents the method to be executed for an action.
    public ActionHandler actionHandler;
    public InputAction inputAction;
    public float actionLockoutDuration;
    public float timePressed;


    public BufferedInput(ActionHandler actionHandler, InputAction inputAction, float lockoutDuration, float timePressed)
    {
        this.actionHandler = actionHandler;
        this.inputAction = inputAction;
        actionLockoutDuration = lockoutDuration;
        this.timePressed = timePressed;
    }

}
