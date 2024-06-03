using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class MainMenuBGMController : MonoBehaviour
{
    [SerializeField] StudioEventEmitter mainMenuBGM;


    EventBinding<SceneBeginChangeEvent> sceneChangeEventBinding;

    void Start()
    {

        StartCoroutine(DelayBeforePlayingBGM());
    }


    void Update()
    {
        
    }

    IEnumerator DelayBeforePlayingBGM()
    {
        yield return new WaitForSeconds(1f);
        mainMenuBGM.Play();
    }


}
