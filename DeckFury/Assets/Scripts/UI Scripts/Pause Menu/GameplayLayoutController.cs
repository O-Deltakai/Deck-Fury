using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayLayoutController : MonoBehaviour
{
    [SerializeField] Toggle _relativeAimingToggle;
    [SerializeField] Slider _aimSensitivitySlider;
    [SerializeField] Slider _cameraShakeSlider;



    void Start()
    {
        _relativeAimingToggle.isOn = SettingsManager.UseRelativeAiming;
        _aimSensitivitySlider.value = SettingsManager.CursorSensitivity;
        _cameraShakeSlider.value = SettingsManager.GlobalCameraShakeMultiplier;
    }

}
