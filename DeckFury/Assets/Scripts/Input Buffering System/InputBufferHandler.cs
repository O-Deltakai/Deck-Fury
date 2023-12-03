using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBufferHandler : MonoBehaviour
{

    Queue<BufferedInput> buffer = new Queue<BufferedInput>();
    [SerializeField, Min(0)] float bufferTime = 0.2f;

    float lastActionTime = -1f; //The time when the last action was performed


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
        if(buffer.Count == 0) { return; }

        float currentTime = Time.time;

        int bufferCount = buffer.Count;

        for(int i = 0; i < bufferCount; i++)
        {
            BufferedInput action = buffer.Dequeue();

            if(currentTime - lastActionTime > action.actionLockoutDuration)
            {
                ExecuteAction(action);
                lastActionTime = currentTime;

                //Clear the rest of the buffer as we've executed a recent input
                buffer.Clear();
                break;
            }
            else if (currentTime - action.timePressed < bufferTime)
            {
                BufferAction(action);
            }

        }

    }



}
