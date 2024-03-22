using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;


public class BGMManager : MonoBehaviour
{

    StudioEventEmitter eventEmitter;

    [SerializeField] EventReference currentBGM;
    EventInstance currentBGMInstance;

    [Header("BGM Events")]
    [SerializeField] EventReference mainMenuBGM;
    [SerializeField] EventReference tutorialBGM;
    [SerializeField] EventReference combatBGM;
    [SerializeField] EventReference bossBGM;
    [SerializeField] EventReference victoryBGM;
    [SerializeField] EventReference defeatBGM;
    [SerializeField] EventReference shopBGM;
    [SerializeField] EventReference peaceBGM;

    [SerializeField] bool testTrigger = false;
    [SerializeField] bool testStopTrigger = false;
    
    [SerializeField] float fadeOutDuration = 1;


    void Awake()
    {
        eventEmitter = GetComponent<StudioEventEmitter>();
        eventEmitter.AllowFadeout = true;
    }

    public void PlayBGM()
    {
        if(currentBGMInstance.isValid())
        {
            currentBGMInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentBGMInstance.release();
        }

        currentBGMInstance = RuntimeManager.CreateInstance(currentBGM);
        currentBGMInstance.start();

    }

    public void StopBGM()
    {
        if(currentBGMInstance.isValid())
        {
            currentBGMInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentBGMInstance.release();
        }        
    }

    public void ChangeBGM(EventReference bgmReference)
    {
        if(currentBGMInstance.isValid())
        {
            currentBGMInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentBGMInstance.release();
        }

        currentBGM = bgmReference;
        currentBGMInstance = RuntimeManager.CreateInstance(currentBGM);
        currentBGMInstance.start();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if(testTrigger)
        {
            testTrigger = false;
            PlayBGM();
        }

        if(testStopTrigger)
        {
            testStopTrigger = false;
            StopBGM();
        }
    }
}
