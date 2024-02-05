using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using UnityEngine.InputSystem;

public class StagePostProcessingController : MonoBehaviour
{
    [SerializeField] Volume globalVolume;
    [SerializeField] List<VolumeComponent> postProcessingComponents;

    [SerializeField] bool testButtonU;

    void Start()
    {
        postProcessingComponents = globalVolume.profile.components;
        globalVolume.weight = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(testButtonU)
        {
            if(Keyboard.current.uKey.wasPressedThisFrame)
            {
                LerpVolumeZeroToOne(0.75f);
            }
        }
    }

    public void SetVolumeProfile(VolumeProfile volumeProfile)
    {
        globalVolume.profile = volumeProfile;
    }

    public void LerpVolumeZeroToOne(float duration)
    {
        DOTween.To(() => globalVolume.weight = 0, x => globalVolume.weight = x, 1, duration).SetEase(Ease.OutCirc).SetUpdate(true);
    }

    public void GetColorAdjustmentComponent()
    {
        if (globalVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            DOTween.To(() => colorAdjustments.postExposure.value = 0, (x) => colorAdjustments.postExposure.value = x, 1f, 1f);
        }
    }


}
