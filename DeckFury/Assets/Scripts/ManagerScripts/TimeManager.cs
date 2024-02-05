using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityUtils;

public class TimeManager : MonoBehaviour
{
    static TimeManager _instance;
    public static TimeManager Instance => _instance;

    static bool _slowMotionInProgress;
    public static bool SlowMotionInProgress => _slowMotionInProgress;

    private Stack<float> timeScaleStack = new Stack<float>();

    [SerializeField] bool testSlowMotion = false;

    Coroutine lerpTimeToZeroCoroutine;
    Coroutine lerpTimeToOneCoroutine;


    void Awake()
    {
        InitializePersistentSingleton();

    }
    private void InitializePersistentSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }        
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
        _slowMotionInProgress = false;
        Time.timeScale = 1f;
    }


    /// <summary>
    /// Gradually decrease time scale to 0 over a given duration using an exponential decay formula.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="decayRate"></param>
    /// <returns></returns>
    public void LerpTimeToZero(float duration, float decayRate)
    {
        if(lerpTimeToOneCoroutine != null)
        {
            StopCoroutine(lerpTimeToOneCoroutine);
        }
        lerpTimeToZeroCoroutine = StartCoroutine(LerpTimeSlowCoroutine(duration, decayRate));
    }

    IEnumerator LerpTimeSlowCoroutine(float duration, float decayRate)
    {
        _slowMotionInProgress = true;
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
        lerpTimeToZeroCoroutine = null;

    }

    /// <summary>
    /// Gradually increase time scale to 1 over a given duration using an exponential growth formula.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="growthRate"></param>
    /// <returns></returns>
    public void LerpTimeToOne(float duration, float growthRate)
    {
        if(lerpTimeToZeroCoroutine != null)
        {
            StopCoroutine(lerpTimeToZeroCoroutine);
        }
        lerpTimeToOneCoroutine = StartCoroutine(LerpTimeSpeedUpCoroutine(duration, growthRate));
    }

    IEnumerator LerpTimeSpeedUpCoroutine(float duration, float growthRate)
    {
        _slowMotionInProgress = true;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float normalizedTime = elapsed / duration;
            // Apply the inverted exponential growth formula
            Time.timeScale = 1 - Mathf.Exp(-growthRate * normalizedTime);
            yield return null;
        }

        Time.timeScale = 1f; // Ensure time scale is set to exactly 1 at the end
        lerpTimeToOneCoroutine = null;

        _slowMotionInProgress = false;
    }


}
