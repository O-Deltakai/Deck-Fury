using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance => _instance;

    const string MASTER_PATH = "bus:/";
    const string MUSIC_PATH = "bus:/Music";
    const string SFX_PATH = "bus:/SFX";
    const string AMBIANCE_PATH = "bus:/Ambiance";
    const string VOICE_PATH = "bus:/Voice";

    Bus Master;
    Bus Music;
    Bus SFX;
    Bus Ambiance;
    Bus Voice;

[Header("Default Volumes")]
    [SerializeField, Range(0, 1)] float _musicVolume = 0.5f;
    public float MusicVolume { get => _musicVolume; }
    [SerializeField, Range(0, 1)] float _sfxVolume = 0.5f;
    public float SFXVolume { get => _sfxVolume; }
    [SerializeField, Range(0, 1)] float _ambianceVolume = 0.5f;
    public float AmbianceVolume { get => _ambianceVolume; }
    [SerializeField, Range(0, 1)] float _voiceVolume = 0.5f;
    public float VoiceVolume { get => _voiceVolume; }
    [SerializeField, Range(0, 1)] float _masterVolume = 1;
    public float MasterVolume { get => _masterVolume; }



    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        AssignBuses();
        GetAudioPrefs();
        SetInitialVolumes();
    }

    void AssignBuses()
    {
        Master = RuntimeManager.GetBus(MASTER_PATH);
        Music = RuntimeManager.GetBus(MUSIC_PATH);
        SFX = RuntimeManager.GetBus(SFX_PATH);
        Ambiance = RuntimeManager.GetBus(AMBIANCE_PATH);
        Voice = RuntimeManager.GetBus(VOICE_PATH);
    }

    void SetInitialVolumes()
    {
        SetMasterVolume(_masterVolume);
        SetMusicVolume(_musicVolume);
        SetSFXVolume(_sfxVolume);
        SetAmbianceVolume(_ambianceVolume);
        SetVoiceVolume(_voiceVolume);
    }

    void GetAudioPrefs()
    {
        _masterVolume = PlayerPrefsManager.GetAudioPref(PlayerPrefsManager.AudioKeys.MasterVolume);
        if (float.IsNaN(_masterVolume))
        {
            _masterVolume = 1f; // Default Master Volume
        }

        _musicVolume = PlayerPrefsManager.GetAudioPref(PlayerPrefsManager.AudioKeys.MusicVolume);
        if (float.IsNaN(_musicVolume))
        {
            _musicVolume = 0.5f; // Default Music Volume
        }

        _sfxVolume = PlayerPrefsManager.GetAudioPref(PlayerPrefsManager.AudioKeys.SFXVolume);
        if (float.IsNaN(_sfxVolume))
        {
            _sfxVolume = 0.5f; // Default SFX Volume
        }

        _ambianceVolume = PlayerPrefsManager.GetAudioPref(PlayerPrefsManager.AudioKeys.AmbianceVolume);
        if (float.IsNaN(_ambianceVolume))
        {
            _ambianceVolume = 0.5f; // Default Ambiance Volume
        }

        _voiceVolume = PlayerPrefsManager.GetAudioPref(PlayerPrefsManager.AudioKeys.VoiceVolume);
        if (float.IsNaN(_voiceVolume))
        {
            _voiceVolume = 0.5f; // Default Voice Volume
        }
    }


    public void SetMasterVolume(float newValue)
    {
        Master.setVolume(newValue);
        PlayerPrefsManager.SetAudioPref(PlayerPrefsManager.AudioKeys.MasterVolume, newValue);
    }

    public void SetMusicVolume(float newValue)
    {
        Music.setVolume(newValue);
        PlayerPrefsManager.SetAudioPref(PlayerPrefsManager.AudioKeys.MusicVolume, newValue);
    }

    public void SetSFXVolume(float newValue)
    {
        SFX.setVolume(newValue);
        PlayerPrefsManager.SetAudioPref(PlayerPrefsManager.AudioKeys.SFXVolume, newValue);
    }

    public void SetAmbianceVolume(float newValue)
    {
        Ambiance.setVolume(newValue);
        PlayerPrefsManager.SetAudioPref(PlayerPrefsManager.AudioKeys.AmbianceVolume, newValue);
    }

    public void SetVoiceVolume(float newValue)
    {
        Voice.setVolume(newValue);
        PlayerPrefsManager.SetAudioPref(PlayerPrefsManager.AudioKeys.VoiceVolume, newValue);
    }

}
