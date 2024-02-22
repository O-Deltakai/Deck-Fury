using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterFinishAnimation : MonoBehaviour
{
    [SerializeField] AnimationClip _animationClip;
    [SerializeField] bool useUnscaledTime;

    Coroutine CR_Timer;

    void OnEnable()
    {
        if (CR_Timer != null)
        {
            StopCoroutine(CR_Timer);
        }

        CR_Timer = StartCoroutine(Timer(_animationClip.length));
    }

    void OnDisable()
    {
        if (CR_Timer != null)
        {
            StopCoroutine(CR_Timer);
        }
    }

    IEnumerator Timer(float duration)
    {
        if (useUnscaledTime)
        {
            yield return new WaitForSecondsRealtime(duration);
        }
        else
        {
            yield return new WaitForSeconds(duration);
        }

        gameObject.SetActive(false);
    }

}
