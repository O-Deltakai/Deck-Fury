using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextShaker : MonoBehaviour
{
    [SerializeField] TextMeshPro textElement;

    public float shakeDuration = 0.12f;
    public float baseShakeStrength = 0.2f;
    public int shakeVibrato = 1000;
    public float shakeRandomness = 90;

    Tween shakeTween = null;

    [SerializeField] bool resetShake = false;

    bool isShaking = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(resetShake)
        {
            resetShake = false;
            StartShaking();
        }   
    }

    public void StartShaking()
    {
        if(isShaking){return;}
        isShaking = true;

        if(shakeTween.IsActive()){shakeTween.Kill();}
        shakeTween = textElement.transform.DOShakePosition(shakeDuration, baseShakeStrength, shakeVibrato, 90, false, true).SetLoops(-1);
    }

    public void StopShaking()
    {
        if(shakeTween.IsActive()){shakeTween.Kill();}
        isShaking = false;
    }

}
