using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Simple component that scales an object back and forth with tweening.
/// </summary>
public class ScaleBackAndForth : MonoBehaviour
{
    [SerializeField] float _scaleDuration = 0.5f;
    [SerializeField] float _scaleAmount = 0.1f;

    [Tooltip("If true, the initial scale will be multiplied by the scale amount.")]
    [SerializeField] bool _multiplyScale = false;

    [SerializeField] bool _scaleX = true;
    [SerializeField] bool _scaleY = true;
    [SerializeField] Ease _easeType = Ease.Linear;

    Vector3 _initialScale;
    Tween scaleTween;

    void Awake()
    {
        _initialScale = transform.localScale;
    }

    void OnEnable()
    {
        ScaleForth();
    }

    void ScaleForth()
    {
        if (scaleTween.IsActive()){ scaleTween.Kill(); }

        Vector3 scaleAmount = new(_scaleX ? _scaleAmount : 0, _scaleY ? _scaleAmount : 0, 0);

        if(_multiplyScale)
        {
            scaleAmount = new Vector3(_initialScale.x * _scaleAmount, _initialScale.y * _scaleAmount, 0);
            scaleTween = transform.DOScale(scaleAmount, _scaleDuration).SetEase(_easeType).OnComplete(ScaleBack);
        }else
        {
            scaleTween = transform.DOScale(transform.localScale + scaleAmount, _scaleDuration).SetEase(_easeType).OnComplete(ScaleBack);
        }
    }

    void ScaleBack()
    {
        if (scaleTween.IsActive()){ scaleTween.Kill(); } 

        scaleTween = transform.DOScale(_initialScale, _scaleDuration).SetEase(_easeType).OnComplete(ScaleForth);
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
