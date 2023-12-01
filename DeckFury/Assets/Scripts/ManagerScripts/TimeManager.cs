using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class TimeManager : MonoBehaviour
{
    static TimeManager _instance;
    public static TimeManager Instance => _instance;

    static bool _timeScaleIsModified = false;
    public static bool TimeScaleIsModified => _timeScaleIsModified;

    [SerializeField, Min(0)] float slowMotionScale = 0.5f;
    [SerializeField, Min(0)] float slowMotionDuration = 1.5f;

    Coroutine CR_SlowMotionTimer = null;

    [SerializeField] bool testButtonEnabled = false;

    [SerializeField] Color originalBackgroundColor;
    [SerializeField] Color slowedTimeBackgroundColor;

    void Update()
    {
        if(testButtonEnabled)
        {
            if(Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                TriggerSlowMotion();
            }
        }
    }

    public void TriggerSlowMotion()
    {
        if(CR_SlowMotionTimer != null) { return; }

        CR_SlowMotionTimer = StartCoroutine(SlowMotionTimer(slowMotionDuration));
    }

    IEnumerator SlowMotionTimer(float duration)
    {
        _timeScaleIsModified = true;
        Time.timeScale = slowMotionScale;
        Camera.main.DOColor(slowedTimeBackgroundColor, 0.5f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(duration);
        Camera.main.DOColor(originalBackgroundColor, 0.2f).SetUpdate(true);

        Time.timeScale = 1;
        CR_SlowMotionTimer = null;


    }


}
