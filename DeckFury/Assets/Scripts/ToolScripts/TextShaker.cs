using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextShaker : MonoBehaviour
{
    [SerializeField] TextMeshPro textElement;
    [SerializeField] TextMeshProUGUI textElementUGUI;

    [SerializeField] bool useUGUI = false;


    public float shakeDuration = 0.12f;
    public float baseShakeStrength = 0.2f;
    public int shakeVibrato = 1000;
    public float shakeRandomness = 90;

    Tween shakeTween = null;

    public bool ignoreTimeScale = true;
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
            StopShaking();
            StartShaking();
        }   
    }

    public void StartShaking()
    {
        if(isShaking){return;}
        isShaking = true;

        if(shakeTween.IsActive()){shakeTween.Kill();}

        if(useUGUI)
        {
            shakeTween = textElementUGUI.transform.DOShakePosition(shakeDuration, baseShakeStrength, shakeVibrato, shakeRandomness, false, true)
            .SetLoops(-1).SetUpdate(ignoreTimeScale);
        }else
        {
            shakeTween = textElement.transform.DOShakePosition(shakeDuration, baseShakeStrength, shakeVibrato, shakeRandomness, false, true)
            .SetLoops(-1).SetUpdate(ignoreTimeScale);
        }

    }

    public void StopShaking()
    {
        if(shakeTween.IsActive()){shakeTween.Kill();}
        isShaking = false;
    }

}
