using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBufferHandler : MonoBehaviour
{

    Queue<BufferedInput> buffer = new Queue<BufferedInput>();
    [SerializeField, Min(0)] float bufferTime = 0.2f;
    [SerializeField] int maxActionsToExecute = 2; // Allows up to 2 actions to be buffered and executed

    float lastActionTime = -1f; //The time when the last action was performed
    float nextEligibleTime = -1f; // Initialize to an invalid time initially
    float lastActionLockoutDuration = 0f; // The lockout duration of the last action that was executed

    public bool useUnscaledTime = false;

    void Update()
    {
        HandleBufferedInputs();
    }

    public void BufferAction(BufferedInput bufferedInput)
    {
        buffer.Enqueue(bufferedInput);
    }


    void ExecuteAction(BufferedInput action)
    {
        action.actionHandler?.Invoke(); // Invoke the method that has been assigned to the actionHandler delegate
    }


    void HandleBufferedInputs()
    {
        if (buffer.Count == 0) return;

        float currentTime = useUnscaledTime ? Time.unscaledTime : Time.time;
        //Debug.Log($"[HandleBufferedInputs] Current Time: {currentTime}, Buffer Count: {buffer.Count}");

        if (currentTime < lastActionTime + lastActionLockoutDuration) {
            Debug.Log($"[HandleBufferedInputs] Waiting for lockout duration to pass. Last Action Time: {lastActionTime}, Lockout Duration: {lastActionLockoutDuration}, Current Time: {currentTime}");
            return; // Wait until lockout duration has passed
        }

        BufferedInput? nextActionToExecute = null;

        // Find the next eligible action to execute
        foreach (var action in buffer)
        {
            float timeSinceAction = currentTime - action.timePressed;
            Debug.Log($"[HandleBufferedInputs] Considering action: {action.actionHandler.Method.Name}, Time Since Action: {timeSinceAction}");

            if (nextActionToExecute == null || action.timePressed < nextActionToExecute.Value.timePressed)
            {
                nextActionToExecute = action;
            }
        }

        // Execute the found action
        if (nextActionToExecute.HasValue)
        {
            float timeSinceAction = currentTime - nextActionToExecute.Value.timePressed;
            ExecuteAction(nextActionToExecute.Value);
            Debug.Log($"[HandleBufferedInputs] Executing action: {nextActionToExecute.Value.actionHandler.Method.Name}, Time Since Action: {timeSinceAction}, Action Time Pressed: {nextActionToExecute.Value.timePressed}");

            lastActionTime = currentTime;
            lastActionLockoutDuration = nextActionToExecute.Value.actionLockoutDuration; // Update this with the executed action's lockout
            buffer.Clear(); // Assuming you execute the most immediate action and clear the buffer

            Debug.Log($"[HandleBufferedInputs] Action executed. Last Action Time updated to: {lastActionTime}, New Lockout Duration: {lastActionLockoutDuration}");
        }
    }

    // void HandleBufferedInputs()
    // {
    //     if (buffer.Count == 0) return;

    //     float currentTime = useUnscaledTime ? Time.unscaledTime : Time.time;
    //     //Debug.Log($"[HandleBufferedInputs] Current Time: {currentTime}, Buffer Count: {buffer.Count}");

    //     if (currentTime < lastActionTime + lastActionLockoutDuration) {
    //         Debug.Log($"[HandleBufferedInputs] Waiting for lockout duration to pass. Last Action Time: {lastActionTime}, Lockout Duration: {lastActionLockoutDuration}");
    //         return; // Wait until lockout duration has passed
    //     }

    //     int actionsExecuted = 0;
    //     List<BufferedInput> actionsToRemove = new List<BufferedInput>();

    //     foreach (var action in buffer)
    //     {
    //         if (currentTime - lastActionTime >= action.actionLockoutDuration && actionsExecuted < maxActionsToExecute)
    //         {
    //             float timeSinceAction = currentTime - action.timePressed;
    //             ExecuteAction(action);
    //             Debug.Log($"[HandleBufferedInputs] Executing action: {action.actionHandler.Method.Name}, Time Since Action: {timeSinceAction}, Action Time Pressed: {action.timePressed}");

    //             lastActionTime = currentTime;
    //             actionsExecuted++;
    //             actionsToRemove.Add(action);

    //             if(actionsExecuted >= maxActionsToExecute) {
    //                 break; // Breaks the loop after executing the allowed number of actions
    //             }
    //         }
    //     }

    //     // Remove executed actions from the buffer
    //     foreach (var action in actionsToRemove)
    //     {
    //         buffer = new Queue<BufferedInput>(buffer.Where(a => a.timePressed != action.timePressed));
    //     }

    //     if(actionsExecuted > 0) {
    //         Debug.Log($"[HandleBufferedInputs] Actions executed: {actionsExecuted}. Buffer updated. New Buffer Count: {buffer.Count}");
    //     }
    // }


}
