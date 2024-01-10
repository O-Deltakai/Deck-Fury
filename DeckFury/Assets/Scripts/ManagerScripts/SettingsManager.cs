using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager _instance;
    public static SettingsManager Instance => _instance;

    public static event Action<bool> OnChangeAimingStyle; 

    private static bool _useRelativeAiming = true;
    public static bool UseRelativeAiming{get{ return _useRelativeAiming; }
        set
        {
            if(_useRelativeAiming == value)
            {
                return;
            }else
            {
                _useRelativeAiming = value;
                OnChangeAimingStyle?.Invoke(value);
                print("Relative Aiming has been toggled to " + value);
            }
        }

    
    }




    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        _instance = null;
        OnChangeAimingStyle = null;
    }

    public static void ToggleRelativeAiming(bool flag)
    {
        UseRelativeAiming = flag;
    }


}
