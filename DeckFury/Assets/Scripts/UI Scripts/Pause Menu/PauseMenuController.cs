using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public event Action OnOpenPauseMenu;

    [SerializeField] Canvas menuScreenCanvas;


    void Start()
    {
        
    }



    public void OpenPauseMenu()
    {
        gameObject.SetActive(true);
        menuScreenCanvas.GetComponent<GraphicRaycaster>().enabled = true;


    }

#region Menu Button Methods

    public void Resume()
    {

    }

    public void Options()
    {

    }


    public void MainMenu()
    {
        
    }


#endregion

}
