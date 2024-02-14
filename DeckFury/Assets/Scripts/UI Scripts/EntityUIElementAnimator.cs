using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class EntityUIElementAnimator : MonoBehaviour
{
[Header("Number Counter Variables")]
    [SerializeField] float countFPS = 24;
    [SerializeField] float countDuration = 0.12f;
    [SerializeField] int thresholdForFastCount = 10;
    [SerializeField] int thresholdForSlowCount = 50;
    [SerializeField] float multiplierForFastCount = 0.5f;
    [SerializeField] float multiplierForSlowCount = 1.25f;

[Header("Number Shake Variables")]
    [SerializeField]float ShakeDuration = 0.12f;
    [SerializeField]float BaseShakeStrength = 0.2f;
    [SerializeField]int ShakeVibrato = 1000;
    [SerializeField] int thresholdForLightShake = 10;
    [SerializeField] int thresholdForMediumShake = 50;
    [SerializeField] float multiplierForLightShake = 0.75f;
    [SerializeField] float multiplierForHeavyShake = 2f;

    Tween textShakeTween = null;

    Coroutine CR_AnimateNumberCounter = null;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimateNumberCounter(TextMeshPro textElement, int initialNum, int finalNum)
    {
        if(CR_AnimateNumberCounter != null)
        {
            StopCoroutine(CR_AnimateNumberCounter);
        }

        int difference = Math.Abs(initialNum - finalNum);
        if(difference <= thresholdForFastCount)
        {
            CR_AnimateNumberCounter = StartCoroutine(AnimateNumber(textElement, initialNum, finalNum, countDuration * multiplierForFastCount));
        }else 
        if(difference < thresholdForSlowCount)
        {
            CR_AnimateNumberCounter = StartCoroutine(AnimateNumber(textElement, initialNum, finalNum, countDuration));
        }else
        {
            CR_AnimateNumberCounter = StartCoroutine(AnimateNumber(textElement, initialNum, finalNum, countDuration * multiplierForSlowCount));
        }

    }

    //Method which animates a dynamic counter for a number text element
    private IEnumerator AnimateNumber(TextMeshPro textElement, int initialNum, int finalNum, float countSpeed)
    {
        int startNum = initialNum;

        int endNum = finalNum;

        int stepAmount;
        float defaultDelay = countDuration / countFPS;
        int numOfSteps = (int) Math.Round(countDuration/defaultDelay, MidpointRounding.AwayFromZero);


        int difference = Mathf.Abs(startNum - endNum);

        stepAmount = Mathf.CeilToInt((float)difference/(float)numOfSteps);

        int updatedNumSteps = difference / stepAmount;

        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(countDuration/updatedNumSteps);


        int stepCounter = 0;
        if(startNum > endNum)
        {
            while(stepCounter < updatedNumSteps)
            {
                stepCounter++;
                startNum -= stepAmount;
                textElement.text = Mathf.Clamp(startNum, 0, startNum).ToString();
                if(startNum <= 0)
                {
                    textElement.gameObject.SetActive(false);
                    //AnimateHPCoroutine = null;
                    yield break;
                    
                }
                yield return wait;

            }
            textElement.text = endNum.ToString();
            stepCounter = 0;

        }else 
        if(startNum < endNum)
        {
            while(stepCounter < updatedNumSteps)
            {
                stepCounter++;
                startNum += stepAmount;
                textElement.text = Mathf.Clamp(startNum, 0, startNum).ToString();

                yield return wait;
            }
            textElement.text = endNum.ToString();
            stepCounter = 0;
    
        }

        CR_AnimateNumberCounter = null;

        //AnimateHPCoroutine = null;

    }


    //Method to use when animating shaking for a text element.
    public void AnimateShakeNumber(TextMeshPro textElement, int amount, Color originalColor, Color transitionColor)
    {
        if(amount <= 0) { return; }
        if(textShakeTween != null)
        {
            textShakeTween.Complete();
        }


        if(amount <= thresholdForLightShake)
        {
            textShakeTween = textElement.rectTransform.DOShakePosition(ShakeDuration, BaseShakeStrength*0.5f, ShakeVibrato).SetUpdate(true);
            StartCoroutine(FadeTextInAndOutColor(textElement, originalColor, transitionColor, ShakeDuration * multiplierForLightShake));

        }else if(amount < thresholdForMediumShake)
        {
            textShakeTween = textElement.rectTransform.DOShakePosition(ShakeDuration*1.5f, BaseShakeStrength, ShakeVibrato).SetUpdate(true);
            StartCoroutine(FadeTextInAndOutColor(textElement, originalColor, transitionColor, ShakeDuration));
        }else
        {
            textShakeTween = textElement.rectTransform.DOShakePosition(ShakeDuration*2f, BaseShakeStrength*2f, ShakeVibrato).SetUpdate(true);

            StartCoroutine(FadeTextInAndOutColor(textElement, originalColor, transitionColor, ShakeDuration * multiplierForHeavyShake));
        }

    }


    IEnumerator FadeTextInAndOutColor(TextMeshPro textElement, Color originalColor, Color tempColor, float duration)
    {


        textElement.DOColor(tempColor, duration*0.6f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(duration*0.5f);
        textElement.DOColor(originalColor, duration*0.4f).SetUpdate(true);
        
    }




}
