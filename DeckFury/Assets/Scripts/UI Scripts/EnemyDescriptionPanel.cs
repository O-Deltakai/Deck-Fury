using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using FMODUnity;
using FMOD.Studio;

public class EnemyDescriptionPanel : MonoBehaviour
{
    public CardSelectionMenu cardSelectionMenu;

    public EnemyDataSO enemyData;
    [SerializeField] CardUIIconSO uiIcons;

    [SerializeField] GameObject descriptionPanel;
    [SerializeField] Button hoverSelectorButton;

    [SerializeField] TextMeshProUGUI enemyName;


[Header("Armor Popout Elements")]
    [SerializeField] GameObject armorPopout;
    [SerializeField] TextMeshProUGUI armorTextValue;

[Header("Weakness Popout Elements")]
    [SerializeField] GameObject weaknessPopout;
    [SerializeField] List<Image> weaknessIcons;
    [SerializeField] GameObject noWeaknessIndicator; // Object that is shown if enemy has no weaknesses

[Header("Resistance Popout Elements")]
    [SerializeField] GameObject resistPopout;
    [SerializeField] List<Image> resistIcons;
    [SerializeField] GameObject noResistsIndicator; // Object that is shown if enemy has no resistances

    [Header("SFX")]
    [SerializeField] EventReference onClickSFX;
    EventInstance  onClickSFXInstance;

    bool panelIsOpen = false;

    Coroutine CR_DisablePanelCoroutine = null;


    void Awake()
    {
        DisableSelectorButton();
    }

    void Start()
    {
        cardSelectionMenu = GameErrorHandler.NullCheck(CardSelectionMenu.Instance, "CardSelectionMenu");
        AssignEvents();
        AssignDataToPanel(enemyData);

        onClickSFXInstance = RuntimeManager.CreateInstance(onClickSFX);
    }

    void AssignEvents()
    {
        cardSelectionMenu.OnUnpreviewStage += DisableSelectorButton;
        cardSelectionMenu.OnUnpreviewStage += ExitHoverOverSelector;

        cardSelectionMenu.OnPreviewStage += EnableSelectorButton;

        cardSelectionMenu.OnMenuDisabled += DisableSelectorButton;
        cardSelectionMenu.OnMenuDisabled += ExitHoverOverSelector;

    }

    void OnDestroy()
    {
        cardSelectionMenu.OnUnpreviewStage -= DisableSelectorButton;
        cardSelectionMenu.OnUnpreviewStage -= ExitHoverOverSelector;

        cardSelectionMenu.OnPreviewStage -= EnableSelectorButton;


        cardSelectionMenu.OnMenuDisabled -= DisableSelectorButton;        
        cardSelectionMenu.OnMenuDisabled -= ExitHoverOverSelector;

    }


    void AssignDataToPanel(EnemyDataSO data)
    {

        armorTextValue.text = data.Armor.ToString();
        enemyName.text = data.EnemyName;

    //Assign weakness icons
        if(data.Weaknesses.Count <= 0)
        {
            foreach(Image weakIcon in weaknessIcons)
            {
                weakIcon.gameObject.SetActive(false);
            }
            noWeaknessIndicator.SetActive(true);
        }else
        if(data.Weaknesses.Count <= 3)
        {
            int iconIndex = 0;

            foreach(AttackElement attackElement in data.Weaknesses)
            {
                weaknessIcons[iconIndex].sprite = uiIcons.GetElementIcon(attackElement);
                weaknessIcons[iconIndex].gameObject.SetActive(true);

                iconIndex++;
            }

        }else
        {
            for (int i = 0; i < 3; i++) // Only a maximum of 3 icons will be displayed if enemy data contains more weaknesses.
            {
                weaknessIcons[i].sprite = uiIcons.GetElementIcon(data.Weaknesses[i]);
            }
        } 

    //Assign resistance icons
        if(data.Resistances.Count <= 0)
        {
            foreach(Image resistIcon in resistIcons)
            {
                resistIcon.gameObject.SetActive(false);
            }
            noResistsIndicator.SetActive(true);
        }else
        if(data.Resistances.Count <= 3)
        {
            int iconIndex = 0;

            foreach(AttackElement attackElement in data.Resistances)
            {
                resistIcons[iconIndex].sprite = uiIcons.GetElementIcon(attackElement);
                resistIcons[iconIndex].gameObject.SetActive(true);

                iconIndex++;
            }

        }else
        {
            for (int i = 0; i < 3; i++) // Only a maximum of 3 icons will be displayed if enemy data contains more resistances.
            {
                resistIcons[i].sprite = uiIcons.GetElementIcon(data.Resistances[i]);
            }
        }        

    }

    public void OnClickSelectorButton()
    {
        if(panelIsOpen)
        {
            ToggleDescriptionPanel(false);
        }else
        {
            ToggleDescriptionPanel(true);
        }

        onClickSFXInstance.start();
    }

    void EnableSelectorButton()
    {
        hoverSelectorButton.interactable = true;
        hoverSelectorButton.gameObject.SetActive(true);
    }
    void DisableSelectorButton()
    {
        hoverSelectorButton.interactable = false;
        hoverSelectorButton.gameObject.SetActive(false);

        ToggleDescriptionPanel(false);
    }

    //What happens when the pointer hovers over the selector
    public void HoverOverSelector()
    {
        GameObject selectorObject = hoverSelectorButton.gameObject;
        Image selectorImage = selectorObject.GetComponent<Image>();

        selectorObject.transform.DOLocalRotate(new Vector3(0, 0, 45), 0.2f).SetUpdate(true);
        selectorImage.DOFade(1f, 0.2f).SetUpdate(true);
    }

    //What happens when the pointer exits the selector
    public void ExitHoverOverSelector()
    {
        GameObject selectorObject = hoverSelectorButton.gameObject;
        Image selectorImage = selectorObject.GetComponent<Image>();

        selectorObject.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f).SetUpdate(true);
        selectorImage.DOFade(0f, 0.2f).SetUpdate(true);
    }


    void DisablePanel()
    {
        ToggleDescriptionPanel(false);
    }

    public void ToggleDescriptionPanel(bool condition)
    {
        //Prevent toggling the panel whilst the coroutine for closing the panel is still running
        if(CR_DisablePanelCoroutine != null ) { return; }

        if(condition)
        {
            descriptionPanel.SetActive(true);
            panelIsOpen = true;
            descriptionPanel.transform.DOScale(1, 0.2f).SetUpdate(true); //Expand panel
        }else
        {
            if(descriptionPanel.gameObject.activeInHierarchy)
            {
                CR_DisablePanelCoroutine = StartCoroutine(DisableDescriptionPanelCoroutine(0.2f));
            }
        }
    }

    IEnumerator DisableDescriptionPanelCoroutine(float delay)
    {
        panelIsOpen = false;
        descriptionPanel.transform.DOScale(0, 0.2f).SetUpdate(true); //Shrink panel
        yield return new WaitForSecondsRealtime(delay);
        descriptionPanel.gameObject.SetActive(false);

        CR_DisablePanelCoroutine = null;
    }


    public void ToggleArmorPopout(bool condition)
    {
        if(condition)
        {
            armorPopout.SetActive(true);
        }else
        {
            armorPopout.SetActive(false);
        }
    }

    public void ToggleWeaknessPopout(bool condition)
    {
        if(condition)
        {
            weaknessPopout.SetActive(true);
        }else
        {
            weaknessPopout.SetActive(false);
        }
    }

    public void ToggleResistPopout(bool condition)
    {
        if(condition)
        {
            resistPopout.SetActive(true);
        }else
        {
            resistPopout.SetActive(false);
        }
    }

}
