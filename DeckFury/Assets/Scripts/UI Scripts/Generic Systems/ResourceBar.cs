using System;
using System.Collections;
using System.Collections.Generic;
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

    public void SwitchResourceMeter(ResourceMeter otherMeter)
    {
        resourceMeter.OnIntValueModified -= UpdateBar;
        resourceMeter.OnFloatValueModified -= UpdateBar;
        resourceMeter.OnDoubleValueModified -= UpdateBar;

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



}
