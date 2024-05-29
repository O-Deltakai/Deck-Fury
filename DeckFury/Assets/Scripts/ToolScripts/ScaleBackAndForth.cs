using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Simple component that scales an object back and forth with tweening. Can be used in manual mode or automatic mode.
/// If in automatic mode, the object will scale back and forth when enabled.
/// </summary>
public class ScaleBackAndForth : MonoBehaviour
{
    public float scaleDuration = 0.5f;
    [Tooltip("The amount to scale the object by. By default it adds this amount to the current scale.")]
    public float scaleAmount = 0.1f;

    [Tooltip("If true, the initial scale will be multiplied by the scale amount.")]
    public bool multiplyScale = false;

    public bool scaleX = true;
    public bool scaleY = true;
    public Ease easeType = Ease.Linear;

    Vector3 _initialScale;
    Tween scaleTween;

    [Tooltip("If true, the object will scale back and forth automatically when enabled.")]
    public bool autoScale = true;

    public bool locked = false;

    public bool ignoreTimeScale = true;

    void Awake()
    {
        _initialScale = transform.localScale;
    }

    void OnEnable()
    {
        if(autoScale)
        {
            ScaleForthThenBack();
        }
    }

    public void StopScaling()
    {
        if (scaleTween.IsActive()){ scaleTween.Kill(); }
        transform.localScale = _initialScale;
    }

    public void LockScaleAtMax()
    {
        if (scaleTween.IsActive()){ scaleTween.Kill(); }
        locked = true;
        transform.localScale = _initialScale + new Vector3(scaleX ? scaleAmount : 0, scaleY ? scaleAmount : 0, 0);
    }

    public void Unlock()
    {
        locked = false;
    }

    public void ScaleForth()
    {
        if(locked){ return; }
        if (scaleTween.IsActive()){ scaleTween.Kill(); }

        Vector3 vectorScaleAmount = new(scaleX ? scaleAmount : 0, scaleY ? scaleAmount : 0, 0);

        if(multiplyScale)
        {
            vectorScaleAmount = new Vector3(_initialScale.x * scaleAmount, _initialScale.y * scaleAmount, 0);
            scaleTween = transform.DOScale(vectorScaleAmount, scaleDuration).SetEase(easeType).SetUpdate(ignoreTimeScale);
        }else
        {
            scaleTween = transform.DOScale(transform.localScale + vectorScaleAmount, scaleDuration).SetEase(easeType).SetUpdate(ignoreTimeScale);
        }
    }

    public void ScaleBack()
    {
        if(locked){ return; }
        if (scaleTween.IsActive()){ scaleTween.Kill(); }

        scaleTween = transform.DOScale(_initialScale, scaleDuration).SetEase(easeType).SetUpdate(ignoreTimeScale);
    }

    void ScaleForthThenBack()
    {
        if(locked){ return; }
        if (scaleTween.IsActive()){ scaleTween.Kill(); }

        Vector3 vectorScaleAmount = new(scaleX ? scaleAmount : 0, scaleY ? scaleAmount : 0, 0);

        if(multiplyScale)
        {
            vectorScaleAmount = new Vector3(_initialScale.x * scaleAmount, _initialScale.y * scaleAmount, 0f);
            scaleTween = transform.DOScale(vectorScaleAmount, scaleDuration).SetEase(easeType).OnComplete(ScaleBackThenForth).SetUpdate(ignoreTimeScale);
        }else
        {
            scaleTween = transform.DOScale(transform.localScale + vectorScaleAmount, scaleDuration).SetEase(easeType).OnComplete(ScaleBackThenForth)
            .SetUpdate(ignoreTimeScale);
        }
    }

    void ScaleBackThenForth()
    {
        if(locked){ return; }
        if (scaleTween.IsActive()){ scaleTween.Kill(); } 

        scaleTween = transform.DOScale(_initialScale, scaleDuration).SetEase(easeType).OnComplete(ScaleForthThenBack).SetUpdate(ignoreTimeScale);
    }

    void OnDestroy()
    {
        if (scaleTween.IsActive()){ scaleTween.Kill();} 
    }

    void OnDisable()
    {
        if (scaleTween.IsActive()){ scaleTween.Kill(); }
    }

}
