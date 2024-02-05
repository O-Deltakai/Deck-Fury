using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI slider controller designed to hook up to any given ResourceMeter. Automatically subscribes appropriate events according to the given Meter Type.
/// </summary>
public class ResourceBar : MonoBehaviour
{
    [SerializeField] ResourceMeter resourceMeter;
    [SerializeField] Slider slider;
    public enum ValueType{Int, Float, Double}
    public ValueType meterType = ValueType.Int;

    public bool smoothBarMovement = false;

    [Header("Bar Tweening Settings")]
    [SerializeField] float tweenDuration = 0.15f;
    [SerializeField] Ease tweenEase = Ease.OutCirc;
    Tween TW_barTween;


    void Awake()
    {
        if(resourceMeter)
        {
            SubscribeResourceMeter();
        }else
        {
            Debug.LogError("ResourceMeter for this ResourceBar was not set, ResourceBar will not function.", this);
        }
    }

    public void SubscribeResourceMeter()
    {

        if(smoothBarMovement)
        {
            resourceMeter.OnIntValueModified -= UpdateBar;
            resourceMeter.OnFloatValueModified -= UpdateBar;
            resourceMeter.OnDoubleValueModified -= UpdateBar;             

            switch (meterType)
            {
                case ValueType.Int:
                    resourceMeter.OnIntValueChanged += UpdateBarSmooth;
                    resourceMeter.OnFloatValueChanged -= UpdateBarSmooth;
                    resourceMeter.OnDoubleValueChanged -= UpdateBarSmooth;

                    break;

                case ValueType.Float:
                    resourceMeter.OnFloatValueChanged += UpdateBarSmooth;
                    resourceMeter.OnIntValueChanged -= UpdateBarSmooth;
                    resourceMeter.OnDoubleValueChanged -= UpdateBarSmooth;

                    break;

                case ValueType.Double:
                    resourceMeter.OnDoubleValueChanged += UpdateBarSmooth;
                    resourceMeter.OnIntValueChanged -= UpdateBarSmooth;
                    resourceMeter.OnFloatValueChanged -= UpdateBarSmooth;
                    break;
                
                default:
                    break;
            }
        }else
        {
            switch (meterType)
            {
                case ValueType.Int:
                    resourceMeter.OnIntValueModified += UpdateBar;
                    resourceMeter.OnFloatValueModified -= UpdateBar;
                    resourceMeter.OnDoubleValueModified -= UpdateBar;
                    break;

                case ValueType.Float:
                    resourceMeter.OnIntValueModified -= UpdateBar;
                    resourceMeter.OnFloatValueModified += UpdateBar;
                    resourceMeter.OnDoubleValueModified -= UpdateBar;                
                    break;

                case ValueType.Double:
                    resourceMeter.OnIntValueModified -= UpdateBar;
                    resourceMeter.OnFloatValueModified -= UpdateBar;
                    resourceMeter.OnDoubleValueModified += UpdateBar;
                    break;
                
                default:
                    break;
            }

        }
    }

    public void SwitchResourceMeter(ResourceMeter otherMeter)
    {
        resourceMeter.OnIntValueModified -= UpdateBar;
        resourceMeter.OnFloatValueModified -= UpdateBar;
        resourceMeter.OnDoubleValueModified -= UpdateBar;

        resourceMeter.OnIntValueChanged -= UpdateBarSmooth;
        resourceMeter.OnFloatValueChanged -= UpdateBarSmooth;
        resourceMeter.OnDoubleValueChanged -= UpdateBarSmooth;

        resourceMeter = otherMeter;

        SubscribeResourceMeter();
    }


    void UpdateBar()
    {
        switch (meterType)
        {
            case ValueType.Int:
                slider.value = resourceMeter.GetIntValuePercentage();
                break;

            case ValueType.Float:
                slider.value = resourceMeter.GetFloatValuePercentage();
                break;

            case ValueType.Double:
                slider.value = resourceMeter.GetDoubleValuePercentage();
                break;
            
            default:
                break;
        }
    }

    void UpdateBarSmooth(int oldInt, int newInt)
    {
        if(TW_barTween.IsActive())
        {
            TW_barTween.Complete();
        }

        // If the difference between the old and new value is less than 5% of the max value, don't tween
        if(Math.Abs(newInt - oldInt)/resourceMeter.MaxIntValue < 0.05f)
        {
            UpdateBar();
            return;
        }

        TW_barTween = slider.DOValue(resourceMeter.GetIntValuePercentage(), tweenDuration).SetEase(tweenEase).SetUpdate(true);
    }

    void UpdateBarSmooth(float oldFloat, float newFloat)
    {
        if(TW_barTween.IsActive())
        {
            TW_barTween.Complete();
        }

        // If the difference between the old and new value is less than 5% of the max value, don't tween
        if(Math.Abs(newFloat - oldFloat)/resourceMeter.MaxFloatValue < 0.05f)
        {
            UpdateBar();
            return;
        }

        TW_barTween = slider.DOValue(resourceMeter.GetFloatValuePercentage(), tweenDuration).SetEase(tweenEase).SetUpdate(true);        
    }

    void UpdateBarSmooth(double oldDouble, double newDouble)
    {
        if(TW_barTween.IsActive())
        {
            TW_barTween.Complete();
        }

        // If the difference between the old and new value is less than 5% of the max value, don't tween
        if(Math.Abs(newDouble - oldDouble)/resourceMeter.MaxDoubleValue < 0.05f)
        {
            UpdateBar();
            return;
        }

        TW_barTween = slider.DOValue(resourceMeter.GetDoubleValuePercentage(), tweenDuration).SetEase(tweenEase).SetUpdate(true);        
    }

}
