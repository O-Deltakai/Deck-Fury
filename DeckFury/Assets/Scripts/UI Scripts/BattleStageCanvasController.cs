using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStageCanvasController : MonoBehaviour
{

    GraphicRaycaster graphicRaycaster;


    void Start()
    {
        graphicRaycaster = GetComponent<GraphicRaycaster>();

        GameManager.Instance.PauseMenu.OnOpenPauseMenu += DisableGraphicRayCaster;
        GameManager.Instance.PauseMenu.OnClosePauseMenu += EnableGraphicRayCaster;
                
    }


    void DisableGraphicRayCaster()
    {
        graphicRaycaster.enabled = false;
    }


    void EnableGraphicRayCaster()
    {
        graphicRaycaster.enabled = true;
    }

}
