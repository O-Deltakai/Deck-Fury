using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using UnityEngine.InputSystem;

public class StagePostProcessingController : MonoBehaviour
{
    static StagePostProcessingController _instance;
    public static StagePostProcessingController Instance => _instance;


    [SerializeField] Volume globalVolume;
    [SerializeField] Volume focusModeVolume;
    [SerializeField] Volume postExposureVolume;


    [Header("Focus Mode Settings")]
    [SerializeField] float resetFocusModeDuration = 1.5f;
    [SerializeField] Ease resetFocusModeEase = Ease.Linear;    


    [Header("Test Settings")]
    [SerializeField] bool testButtonUStart_IReset;

    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void Start()
    {
        focusModeVolume.weight = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(testButtonUStart_IReset)
        {
            if(Keyboard.current.uKey.wasPressedThisFrame)
            {
                TriggerFocusModePostProcessing();
            }
            if(Keyboard.current.iKey.wasPressedThisFrame)
            {
                ResetFocusModePostProcessing();
            }
        }
    }

    public void SetGlobalVolumeProfile(VolumeProfile volumeProfile)
    {
        globalVolume.profile = volumeProfile;
    }

    public Tween LerpVolumeToValue(Volume volume, float duration, float startValue = 0, float endValue = 1, Ease ease = Ease.Linear)
    {
        return DOTween.To(() => volume.weight = startValue, x => volume.weight = x, endValue, duration).SetEase(ease).SetUpdate(true);
    }


    Tween FlashColorAdjustmentPostExposure(float duration, float startValue, float endValue)
    {
        if (postExposureVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            return DOTween.To(() => colorAdjustments.postExposure.value = startValue, (x) => colorAdjustments.postExposure.value = x, endValue, duration);
        }
        else
        {
            Debug.LogWarning("ColorAdjustment component not found in the volume profile.");
            return null;
        }
    }

    public void TriggerFocusModePostProcessing()
    {
        LerpVolumeToValue(focusModeVolume ,0.55f, 0, 1);
        FlashColorAdjustmentPostExposure(0.1f, 0, 1.9f).SetEase(Ease.InOutExpo).SetUpdate(true).OnComplete(
            () => FlashColorAdjustmentPostExposure(0.5f, 1.9f, 0).SetEase(Ease.OutCirc).SetUpdate(true));


    }


    public void ResetFocusModePostProcessing()
    {
        LerpVolumeToValue(focusModeVolume , resetFocusModeDuration, 1, 0, resetFocusModeEase);
        FlashColorAdjustmentPostExposure(0.1f, 0, 1f).SetEase(Ease.InOutExpo).SetUpdate(true).OnComplete(
            () => FlashColorAdjustmentPostExposure(0.5f, 1f, 0).SetEase(Ease.OutCirc).SetUpdate(true));        

    }



}
