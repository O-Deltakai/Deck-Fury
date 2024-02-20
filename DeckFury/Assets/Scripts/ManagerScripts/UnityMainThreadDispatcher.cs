using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();
    private static UnityMainThreadDispatcher _instance;
    public static UnityMainThreadDispatcher Instance => _instance;

    void Awake()
    {
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }        
    }



    public void Enqueue(Action action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        _executionQueue.Enqueue(action);
    }

    public Task EnqueueTask(Action action)
    {
        print("Enqueueing task...");
        var tcs = new TaskCompletionSource<bool>();
        Enqueue(() =>
        {
            try
            {
                action();
                tcs.SetResult(true); // Signal completion
                print("Task completed");
            }
            catch (Exception ex)
            {
                tcs.SetException(ex); // Signal an error occurred
            }
        });
        return tcs.Task;
    }


    private void Update()
    {
        // Execute all queued actions
        while (_executionQueue.TryDequeue(out var action))
        {
            action.Invoke();
        }
    }
}
