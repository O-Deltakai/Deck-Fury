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

    [SerializeField] bool testSlowMotion = false;

    Coroutine lerpTimeToZeroCoroutine;
    Coroutine lerpTimeToOneCoroutine;
    void OnDestroy()
    {
        _instance = null;
    }

    void Awake()
    {
        _instance = this;
    }

    void Update()
    {
        if(testSlowMotion)
        {
            if(Keyboard.current.tKey.wasPressedThisFrame)
            {
                PauseGame();
            }
            if(Keyboard.current.yKey.wasPressedThisFrame)
            {
                ResumeGame();
            }
        }
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

    public void ForceResumeGame()
    {
        if(lerpTimeToZeroCoroutine != null)
        {
            StopCoroutine(lerpTimeToZeroCoroutine);
        }
        if(lerpTimeToOneCoroutine != null)
        {
            StopCoroutine(lerpTimeToOneCoroutine);
        }
        Time.timeScale = 1f;
    }


    public void LerpTimeToZero(float duration, float decayRate)
    {
        lerpTimeToZeroCoroutine = StartCoroutine(LerpTimeSlowCoroutine(duration, decayRate));
    }

    IEnumerator LerpTimeSlowCoroutine(float duration, float decayRate)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float normalizedTime = elapsed / duration;
            // Apply the exponential decay formula
            Time.timeScale = Mathf.Exp(-decayRate * normalizedTime);
            yield return null;
        }

        Time.timeScale = 0f; // Make sure we end at exactly 0
    }

    public void LerpTimeToOne(float duration)
    {
        lerpTimeToOneCoroutine = StartCoroutine(LerpTimeSpeedUpCoroutine(duration));
    }

    IEnumerator LerpTimeSpeedUpCoroutine(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float normalizedTime = elapsed / duration;
            // Apply the Quadratic Easing In formula
            Time.timeScale = normalizedTime * normalizedTime;
            yield return null;
        }

        Time.timeScale = 1f; // Make sure we end at exactly 1
    }


}
