using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public event Action OnOpenPauseMenu;

    [SerializeField] Canvas menuScreenCanvas;

    [SerializeField] GameObject optionsMenu;

    [SerializeField] GameObject menuButtonsParent;

    [SerializeField] GameObject mainMenuConfirmPopup;

    void Start()
    {
        
    }



    public void OpenPauseMenu()
    {
        if(StageStateController.currentGameState == StageStateController.GameState.Slowmotion)
        {
            return;
        }

        gameObject.SetActive(true);
        menuScreenCanvas.GetComponent<GraphicRaycaster>().enabled = true;

        GameManager.PauseGame();

    }

#region Menu Button Methods

    public void Resume()
    {
        gameObject.SetActive(false);
        menuScreenCanvas.GetComponent<GraphicRaycaster>().enabled = false;

        if(StageStateController.currentGameState != StageStateController.GameState.InMenu)
        {
            GameManager.UnpauseGame();
        }


    }

    public void Options()
    {
        optionsMenu.SetActive(true);
    }


    public void MainMenu()
    {
        mainMenuConfirmPopup.SetActive(true);
        menuButtonsParent.SetActive(false);
    }

    public void DisableConfirmPopup()
    {
        mainMenuConfirmPopup.SetActive(false);
        menuButtonsParent.SetActive(true);
    }

#endregion

}
