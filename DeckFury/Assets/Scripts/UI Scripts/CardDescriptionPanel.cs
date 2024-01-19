using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Panel that pops up when the user hovers over card slot - should display information on the card like its name, damage and description
public class CardDescriptionPanel : MonoBehaviour
{
    [SerializeField] CardSelectionMenu cardSelectionMenu;
    [SerializeField] CardUIIconSO cardUIIcons;

    [SerializeField] TextMeshProUGUI textDescription;
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] Image cardImage;
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] Image elementIcon;
    [SerializeField] Image statusIcon;
    [SerializeField] Image lockedIcon;
    [Header("Element Popout Panel")]
    [SerializeField] GameObject elementDescriptionPanel;
    [SerializeField] TextMeshProUGUI elementDescription;
    [SerializeField] TextMeshProUGUI elementName;
    [SerializeField] Image elementPopoutIcon;

    [Header("StatusEffect Popout Panel")]
    [SerializeField] GameObject statusDescriptionPanel;
    [SerializeField] TextMeshProUGUI statusDescription;
    [SerializeField] TextMeshProUGUI statusName;
    [SerializeField] Image statusPopoutIcon;

    Image descriptionPanelImage;

    bool lockedInPlace = false;    

    CardSlot currentlyViewedSlot;
    [field:SerializeField] public CardSO CurrentlyViewedCardSO{get; private set;}

    private void Awake() 
    {
        descriptionPanelImage = GetComponent<Image>();    
    }

    void Start()
    {
        if(cardSelectionMenu)
        {
            gameObject.SetActive(false);
            lockedIcon.enabled = false;
            cardSelectionMenu.OnMenuDisabled += UnlockPanel;
        }else
        {
            if(CurrentlyViewedCardSO)
            {
                UpdateDescription(CurrentlyViewedCardSO);
                if(lockedIcon) lockedIcon.enabled = false;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeColorBasedOnCardTier(int cardTier)
    {
        if(!descriptionPanelImage) 
        {
            descriptionPanelImage = GetComponent<Image>();    
        }

        descriptionPanelImage.color = cardTier switch
        {
            1 => Color.grey,
            2 => Color.cyan,
            3 => Color.yellow,
            _ => Color.grey,
        };
    }



    //Take in a card slot and update the panel based on the cardSO stored within the card slot.
    public void UpdateDescription(CardSlot cardSlot)
    {
        if(lockedInPlace){return;}
        if(cardSlot.IsEmpty()){return;}

        CardSO cardSO = cardSlot.cardObjectReference.cardSO;
        CurrentlyViewedCardSO = cardSO;

        //Set values for main panel
        textDescription.text = cardSO.GetFormattedDescription();
        cardName.text = cardSO.CardName;
        cardImage.sprite = cardSO.GetCardImage();
        elementIcon.sprite = cardUIIcons.GetElementIcon(cardSO.AttackElement);
        statusIcon.sprite = cardUIIcons.GetStatusIcon(cardSO.StatusEffect);
        ChangeColorBasedOnCardTier(cardSO.GetCardTier());

    //Set values for popout panels

        //Element Popout Panel
        elementPopoutIcon.sprite = cardUIIcons.GetElementIcon(cardSO.AttackElement);
        elementName.text = cardSO.AttackElement.ToString();
        elementDescription.text = cardUIIcons.GetElementDescription(cardSO.AttackElement);

        //Status Popout Panel
        statusPopoutIcon.sprite = cardUIIcons.GetStatusIcon(cardSO.StatusEffect);
        statusName.text = cardSO.StatusEffect.ToString();
        statusDescription.text = cardUIIcons.GetStatusDescription(cardSO.StatusEffect);


        if(cardSO.GetBaseDamage() <= 0)
        {
            damageText.text = "N/A";
        }else
        {
            damageText.text = cardSO.GetBaseDamage().ToString();
        }
        gameObject.SetActive(true);
    }
    //Overload for taking in just a specific CardSO
    public void UpdateDescription(CardSO cardSO)
    {

        CurrentlyViewedCardSO = cardSO;

        //Set values for main panel
        textDescription.text = CurrentlyViewedCardSO.GetFormattedDescription();
        cardName.text = CurrentlyViewedCardSO.CardName;
        cardImage.sprite = CurrentlyViewedCardSO.GetCardImage();
        elementIcon.sprite = cardUIIcons.GetElementIcon(CurrentlyViewedCardSO.AttackElement);
        statusIcon.sprite = cardUIIcons.GetStatusIcon(CurrentlyViewedCardSO.StatusEffect);
        ChangeColorBasedOnCardTier(CurrentlyViewedCardSO.GetCardTier());

    //Set values for popout panels

        //Element Popout Panel
        elementPopoutIcon.sprite = cardUIIcons.GetElementIcon(cardSO.AttackElement);
        elementName.text = cardSO.AttackElement.ToString();
        elementDescription.text = cardUIIcons.GetElementDescription(cardSO.AttackElement);

        //Status Popout Panel
        statusPopoutIcon.sprite = cardUIIcons.GetStatusIcon(cardSO.StatusEffect);
        statusName.text = cardSO.StatusEffect.ToString();
        statusDescription.text = cardUIIcons.GetStatusDescription(cardSO.StatusEffect);


        if(cardSO.GetBaseDamage() <= 0)
        {
            damageText.text = "N/A";
        }else
        {
            damageText.text = cardSO.GetBaseDamage().ToString();
        }
        gameObject.SetActive(true);
    }


    public void DisablePanel()
    {
        if(lockedInPlace){return;}
        gameObject.SetActive(false);
    }

    public void LockPanelInPlace()
    {
        if(!lockedInPlace)
        {
            lockedInPlace = true;
            lockedIcon.enabled = true;
        }else
        {
            lockedIcon.enabled = false;
            lockedInPlace = false;
        }
    }

    void UnlockPanel()
    {
        lockedInPlace = false;
        lockedIcon.enabled = false;
        DisablePanel();
    }

    public void ToggleElementPopoutPanel(bool condition)
    {
        elementDescriptionPanel.gameObject.SetActive(condition);
    }

    public void ToggleStatusPopoutPanel(bool condition)
    {
        statusDescriptionPanel.gameObject.SetActive(condition);
    }


}
