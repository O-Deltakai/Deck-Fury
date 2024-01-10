using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager _instance;
    public static SettingsManager Instance => _instance;

    public static event Action<bool> OnChangeAimingStyle;
    public static event Action<float> OnChangeSensitivity; 


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
    }

    public static void ToggleRelativeAiming(bool flag)
    {
        UseRelativeAiming = flag;
    }

    public static void SetCursorSensitivity(float value)
    {
        CursorSensitivity = value;
    }


}
