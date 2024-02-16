using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class OneShotSFXPlayer : MonoBehaviour
{
    [SerializeField] EventReference _sfxEvent;
    EventInstance sfxInstance;

    [SerializeField] bool _playOnEnable = false;
    [SerializeField] bool _useEventInstance = false;

    void Awake()
    {
        if (_useEventInstance)
        {
            sfxInstance = RuntimeManager.CreateInstance(_sfxEvent);
            sfxInstance.setPaused(true);
        }
    }

    void OnEnable()
    {
        if (_playOnEnable)
        {
            PlaySFX();
        }
    }

    public void PlaySFX()
    {
        if (_useEventInstance)
        {
            sfxInstance.setPaused(false);
            sfxInstance.start();
        }
        else
        {
            RuntimeManager.PlayOneShot(_sfxEvent);
        }
    }

    public void PlaySFX(EventReference eventReference)
    {
        RuntimeManager.PlayOneShot(eventReference);
    }


}
