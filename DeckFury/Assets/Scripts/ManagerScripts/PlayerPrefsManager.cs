using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's preferences and saves them to the player's computer.
/// </summary>
public class PlayerPrefsManager : MonoBehaviour
{
    public enum AudioKeys
    {
        MasterVolume,
        MusicVolume,
        SFXVolume,
        AmbianceVolume,
        VoiceVolume
    }

    public enum GameplayKeys
    {
        CursorSensitivity,
        UseRelativeAiming,
        CameraShakeMultiplier
    }

    static PlayerPrefsManager _instance;
    public static PlayerPrefsManager Instance => _instance;




    void Awake()
    {
        _instance = this;
    }

    public static void SetAudioPref(AudioKeys audioKey, float value)
    {
        PlayerPrefs.SetFloat(audioKey.ToString(), value);
        EventBus<ModifiedPlayerPrefEvent>.Raise(new ModifiedPlayerPrefEvent(audioKey.ToString(), value)); 
    }
    public static float GetAudioPref(AudioKeys audioKey) => PlayerPrefs.GetFloat(audioKey.ToString(), float.NaN);

    public static void SetCursorSensitivity(float value)
    {
        PlayerPrefs.SetFloat(GameplayKeys.CursorSensitivity.ToString(), value);
        EventBus<ModifiedPlayerPrefEvent>.Raise(new ModifiedPlayerPrefEvent(GameplayKeys.CursorSensitivity.ToString(), value));
    }
    public static float GetCursorSensitivity() => PlayerPrefs.GetFloat(GameplayKeys.CursorSensitivity.ToString(), float.NaN);

    public static void SetUseRelativeAiming(bool value)
    {
        PlayerPrefs.SetInt(GameplayKeys.UseRelativeAiming.ToString(), value ? 1 : 0);
        EventBus<ModifiedPlayerPrefEvent>.Raise(new ModifiedPlayerPrefEvent(GameplayKeys.UseRelativeAiming.ToString(), value ? 1 : 0));
    }
    public static bool GetUseRelativeAiming() => PlayerPrefs.GetInt(GameplayKeys.UseRelativeAiming.ToString(), 0) == 1;

    public static void SetGlobalCameraShakeMultiplier(float value)
    {
        PlayerPrefs.SetFloat(GameplayKeys.CameraShakeMultiplier.ToString(), value);
        EventBus<ModifiedPlayerPrefEvent>.Raise(new ModifiedPlayerPrefEvent(GameplayKeys.CameraShakeMultiplier.ToString(), value));
    }
    public static float GetGlobalCameraShakeMultiplier() => PlayerPrefs.GetFloat(GameplayKeys.CameraShakeMultiplier.ToString(), float.NaN);

    public static void SavePlayerPrefs()
    {
        PlayerPrefs.Save();
        
    } 
        

    /// <summary>
    /// Clears all player preferences.
    /// </summary>
    public static void ClearPlayerPrefs() => PlayerPrefs.DeleteAll();

    public static void SetStartingDeckUnlockState(string deckName, bool value) => PlayerPrefs.SetInt(deckName, value ? 1 : 0);
    public static bool GetStartingDeckUnlockState(string deckName) => PlayerPrefs.GetInt(deckName, 0) == 1;


}
