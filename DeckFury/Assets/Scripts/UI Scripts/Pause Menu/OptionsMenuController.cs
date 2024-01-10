using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{

    [SerializeField] List<GameObject> menuLayouts;
    [SerializeField] List<Button> tabButtons;

    [SerializeField] GameObject currentLayout;
    [SerializeField] Button currentSelectedButton;



    [SerializeField] Slider aimSensitivitySlider;
    [SerializeField] TextMeshProUGUI sensitivtyTooltip;

    void Start()
    {
        sensitivtyTooltip.text = Math.Round(aimSensitivitySlider.value, 3).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SwitchLayout(GameObject layout)
    {
        if(currentLayout != null)
        {
            if(currentLayout == layout) { return; }
        }


        if(!menuLayouts.Contains(layout)) 
        {
            Debug.LogWarning("The given object is not part of the menu layouts, the options menu cannot switch to given object.", layout); 
            return; 
        }

        if(currentLayout == null)
        {
            currentLayout = layout;
        }else
        {
            currentLayout.SetActive(false);
            currentLayout = layout;
        }


        currentLayout.SetActive(true);
    }

    public void ToggleTabButtonState(Button button)
    {
        if(currentSelectedButton != null)
        {
            if(currentSelectedButton == button){ return; }
        }

        if(!tabButtons.Contains(button))
        {
            Debug.LogWarning("The given button is not part of the menu tab buttons, the options menu cannot switch to given button.", button); 
            return; 
        }

        if(currentSelectedButton == null)
        {
            currentSelectedButton = button;
        }else
        {
            currentSelectedButton.interactable = true;
            currentSelectedButton = button;
        }

        currentSelectedButton.interactable = false;


    }

    public void OKButton()
    {
        gameObject.SetActive(false);
    }

/// <summary>
/// Used to set the number above the slider handle to allow players to more accurately adjust sensitivty
/// </summary>
/// <param name="value"></param>
    public void SetSensitivtyTooltip(float value)
    {
        float roundedValue = (float)Math.Round(value, 3);
        sensitivtyTooltip.text = roundedValue.ToString();
    }




}
