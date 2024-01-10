using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
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
    [SerializeField, Range(0, 1)] float MusicVolume = 0.5f;
    [SerializeField, Range(0, 1)] float SFXVolume = 0.5f;
    [SerializeField, Range(0, 1)] float AmbianceVolume = 0.5f;
    [SerializeField, Range(0, 1)] float VoiceVolume = 0.5f;
    [SerializeField, Range(0, 1)] float MasterVolume = 1;



    void Awake()
    {
        AssignBuses();
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
        SetMasterVolume(MasterVolume);
        SetMusicVolume(MusicVolume);
        SetSFXVolume(SFXVolume);
        SetAmbianceVolume(AmbianceVolume);
        SetVoiceVolume(VoiceVolume);
    }


    public void SetMasterVolume(float newValue)
    {
        Master.setVolume(newValue);
    }

    public void SetMusicVolume(float newValue)
    {
        Music.setVolume(newValue);
    }

    public void SetSFXVolume(float newValue)
    {
        SFX.setVolume(newValue);
    }

    public void SetAmbianceVolume(float newValue)
    {
        Ambiance.setVolume(newValue);
    }

    public void SetVoiceVolume(float newValue)
    {
        Voice.setVolume(newValue);
    }

}
