using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager _instance;
    public static SettingsManager Instance => _instance;

    /// <summary>
    /// Event that is called when the aiming style is changed. Returns true if relative aiming is used, false if not.
    /// </summary>
    public static event Action<bool> OnChangeAimingStyle;
    public static event Action<float> OnChangeSensitivity; 

    static float _globalCameraShakeMultiplier = 1.0f;
    public static float GlobalCameraShakeMultiplier{get{ return _globalCameraShakeMultiplier; } 
        set
        {
            _globalCameraShakeMultiplier = value;
        }
    }

    private static bool _useRelativeAiming = false;
    public static bool UseRelativeAiming{get{ return _useRelativeAiming; }
        set
        {
            _useRelativeAiming = value;
            OnChangeAimingStyle?.Invoke(value);
            print("Relative Aiming has been toggled to " + value);
        }
    }

    [Range(0.001f, 1)] static float _cursorSensitivity = 0.2f;
    public static float CursorSensitivity{ get { return _cursorSensitivity; } 
        set
        {
            _cursorSensitivity = value;
            OnChangeSensitivity?.Invoke(value);
        }
    
    }





    void Awake()
    {
        _instance = this;
        GetGameplayPrefs();
    }

    void GetGameplayPrefs()
    {
        UseRelativeAiming = PlayerPrefsManager.GetUseRelativeAiming();

        CursorSensitivity = PlayerPrefsManager.GetCursorSensitivity();
        if(float.IsNaN(CursorSensitivity))
        {
            CursorSensitivity = 0.2f; //Default value
        }

        GlobalCameraShakeMultiplier = PlayerPrefsManager.GetGlobalCameraShakeMultiplier();
        if(float.IsNaN(GlobalCameraShakeMultiplier))
        {
            GlobalCameraShakeMultiplier = 1.0f; //Default value
        }


    }

    void OnDestroy()
    {
        _instance = null;
    }

    public static void ToggleRelativeAiming(bool flag)
    {
        UseRelativeAiming = flag;
        PlayerPrefsManager.SetUseRelativeAiming(flag);
    }

    public static void SetCursorSensitivity(float value)
    {
        CursorSensitivity = value;
        PlayerPrefsManager.SetCursorSensitivity(value);
    }

    public static void SetGlobalCameraShakeMultiplier(float value)
    {
        GlobalCameraShakeMultiplier = value;
        PlayerPrefsManager.SetGlobalCameraShakeMultiplier(value);
    }

}
