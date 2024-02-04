using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{

    private bool _isOpen = false;
    public bool IsOpen => _isOpen;


    public event Action OnOpenPauseMenu;
    public event Action OnClosePauseMenu;
    public event Action OnOpenOptionsMenu;

    [SerializeField] Canvas menuScreenCanvas;

    [SerializeField] GameObject optionsMenu;

    [SerializeField] GameObject menuButtonsParent;

    [SerializeField] GameObject mainMenuConfirmPopup;





    public void OpenPauseMenu()
    {
        if(GameManager.currentGameState == GameManager.GameState.Slowmotion)
        {
            return;
        }

        gameObject.SetActive(true);
        menuScreenCanvas.GetComponent<GraphicRaycaster>().enabled = true;

        GameManager.PauseGame();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _isOpen = true;

        OnOpenPauseMenu?.Invoke();
    }

#region Menu Button Methods

    public void Resume()
    {
        gameObject.SetActive(false);
        menuScreenCanvas.GetComponent<GraphicRaycaster>().enabled = false;

        if(GameManager.currentGameState != GameManager.GameState.InMenu)
        {
            GameManager.UnpauseGame();
        }

        _isOpen = false;
        OnClosePauseMenu?.Invoke();

    }

/// <summary>
/// Opens the options menu
/// </summary>
    public void Options()
    {
        optionsMenu.SetActive(true);
        OnOpenOptionsMenu?.Invoke();
    }

/// <summary>
/// Opens the option menu without needing to open the pause menu
/// </summary>
    public void OpenOptionsExternal()
    {
        menuScreenCanvas.GetComponent<GraphicRaycaster>().enabled = true;
        optionsMenu.SetActive(true);
        OnOpenOptionsMenu?.Invoke();
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
