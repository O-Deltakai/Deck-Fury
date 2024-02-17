using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioLayoutController : MonoBehaviour
{
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    // Start is called before the first frame update
    void Start()
    {
        SetInitialSliderValues();
    }

    void SetInitialSliderValues()
    {
        masterVolumeSlider.value = AudioManager.Instance.MasterVolume;
        musicVolumeSlider.value = AudioManager.Instance.MusicVolume;
        sfxVolumeSlider.value = AudioManager.Instance.SFXVolume;
    }


}
