using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class TimeManager : MonoBehaviour
{
    static TimeManager _instance;
    public static TimeManager Instance => _instance;

    private Stack<float> timeScaleStack = new Stack<float>();


    void OnDestroy()
    {
        _instance = null;
    }

    void Awake()
    {
        _instance = this;
    }

    public void PushTimeScale(float newTimeScale)
    {
        timeScaleStack.Push(Time.timeScale);
        Time.timeScale = newTimeScale;
    }

    public void PopTimeScale()
    {
        if (timeScaleStack.Count > 0)
        {
            Time.timeScale = timeScaleStack.Pop();
        }
        else
        {
            Debug.LogWarning("TimeScaleManager: No more time scales to pop.");
        }
    }

    // Optionally, to handle specific scenarios like pausing
    public void PauseGame()
    {
        PushTimeScale(0f);
    }

    public void ResumeGame()
    {
        // Ensure that resuming actually makes sense (e.g., not in the middle of a slow-motion effect)
        if (Time.timeScale == 0f)
        {
            PopTimeScale();
        }
    }




}
