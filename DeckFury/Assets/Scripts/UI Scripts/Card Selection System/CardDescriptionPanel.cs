using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Panel that pops up when the user hovers over card slot - should display information on the card like its name, damage and description
public class CardDescriptionPanel : MonoBehaviour
{
    public event Action OnCardUpdated;

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

[Header("Status Effect Popout Panel")]
    [SerializeField] GameObject statusDescriptionPanel;
    [SerializeField] TextMeshProUGUI statusDescription;
    [SerializeField] TextMeshProUGUI statusName;
    [SerializeField] Image statusPopoutIcon;

    Image descriptionPanelImage;

    bool lockedInPlace = false;    

    CardSlot currentlyViewedSlot;
    [field:SerializeField] public CardSO CurrentlyViewedCardSO{get; private set;}

    CanvasGroup _canvasGroup;

    [SerializeField] float fadeoutDuration = 2f;
    bool pointerIsOnPanel = false;
    Tween fadeoutTween;
    Coroutine CR_FadeoutTimer = null;

    public bool disableFade = false;
    public bool forceEnableFade = false;

    private void Awake() 
    {
        _canvasGroup = GetComponent<CanvasGroup>();
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

            }else
            {
                lockedIcon.enabled = false;
                gameObject.SetActive(false);
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

        CardSO cardSO;
        if(cardSlot.cardObjectReference.cardSO != null)
        {
            cardSO = cardSlot.cardObjectReference.cardSO;
        }else
        {
            cardSO = cardSlot.cardSO;
        }

        bool updatedCard = false;
        if(CurrentlyViewedCardSO != cardSO)
        {
            updatedCard = true;
        }

        CurrentlyViewedCardSO = cardSO;
        if(cardSO == null)
        {
            Debug.LogError("CardSO from cardSlot is null");
            return;
        }


        //Set values for main panel
        textDescription.text = cardSO.GetFormattedDescription();
        cardName.text = cardSO.CardName;
        cardImage.sprite = cardSO.GetCardImage();
        elementIcon.sprite = cardUIIcons.GetElementIcon(cardSO.AttackElement);
        statusIcon.sprite = cardUIIcons.GetStatusIcon(cardSO.statusEffect.statusEffectType);
        ChangeColorBasedOnCardTier(cardSO.GetCardTier());

    //Set values for popout panels

        //Element Popout Panel
        elementPopoutIcon.sprite = cardUIIcons.GetElementIcon(cardSO.AttackElement);
        elementName.text = cardSO.AttackElement.ToString();
        elementDescription.text = cardUIIcons.GetElementDescription(cardSO.AttackElement);

        //Status Popout Panel
        statusPopoutIcon.sprite = cardUIIcons.GetStatusIcon(cardSO.statusEffect.statusEffectType);
        statusName.text = cardSO.statusEffect.statusEffectType.ToString().Replace("_", " ");

        statusDescription.text = cardUIIcons.GetStatusDescription(cardSO.statusEffect.statusEffectType);


        if(cardSO.GetBaseDamage() <= 0)
        {
            damageText.text = "N/A";
        }else
        {
            damageText.text = cardSO.GetBaseDamage().ToString();
        }
        RefreshFadeOut();
        gameObject.SetActive(true);

        if(updatedCard)
        {
            OnCardUpdated?.Invoke();
        }
    }
    //Overload for taking in just a specific CardSO
    public void UpdateDescription(CardSO cardSO)
    {
        bool updatedCard = false;
        if(CurrentlyViewedCardSO != cardSO)
        {
            updatedCard = true;
        }
        CurrentlyViewedCardSO = cardSO;

        //Set values for main panel
        textDescription.text = CurrentlyViewedCardSO.GetFormattedDescription();
        cardName.text = CurrentlyViewedCardSO.CardName;
        cardImage.sprite = CurrentlyViewedCardSO.GetCardImage();
        elementIcon.sprite = cardUIIcons.GetElementIcon(CurrentlyViewedCardSO.AttackElement);
        statusIcon.sprite = cardUIIcons.GetStatusIcon(CurrentlyViewedCardSO.statusEffect.statusEffectType);
        ChangeColorBasedOnCardTier(CurrentlyViewedCardSO.GetCardTier());

    //Set values for popout panels

        //Element Popout Panel
        elementPopoutIcon.sprite = cardUIIcons.GetElementIcon(cardSO.AttackElement);
        elementName.text = cardSO.AttackElement.ToString();
        elementDescription.text = cardUIIcons.GetElementDescription(cardSO.AttackElement);

        //Status Popout Panel
        statusPopoutIcon.sprite = cardUIIcons.GetStatusIcon(cardSO.statusEffect.statusEffectType);
        statusName.text = cardSO.statusEffect.statusEffectType.ToString().Replace("_", " ");
        statusDescription.text = cardUIIcons.GetStatusDescription(cardSO.statusEffect.statusEffectType);


        if(cardSO.GetBaseDamage() <= 0)
        {
            damageText.text = "N/A";
        }else
        {
            damageText.text = cardSO.GetBaseDamage().ToString();
        }
        RefreshFadeOut();
        gameObject.SetActive(true);

        if(updatedCard)
        {
            OnCardUpdated?.Invoke();
        }
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

    public void RefreshFadeOut()
    {
        if(!forceEnableFade)
        {
            if(disableFade) { return; }
            if(!cardSelectionMenu){ return; }
        }

        if(CR_FadeoutTimer != null)
        {
            StopCoroutine(CR_FadeoutTimer);
            CR_FadeoutTimer = null;
        }
        if(fadeoutTween.IsActive())
        {
            fadeoutTween.Kill();
        }
        _canvasGroup.alpha = 1;
    }

    public void BeginFadeOut()
    {
        if(!forceEnableFade)
        {
            if(disableFade) { return; }
            if(!cardSelectionMenu){ return; }
        }

        if(!gameObject.activeInHierarchy) { return; }
        if(lockedInPlace) { return; }
        if(CR_FadeoutTimer != null) { return; }

        CR_FadeoutTimer = StartCoroutine(FadeoutTimer());

    }

    IEnumerator FadeoutTimer()
    {
        fadeoutTween = _canvasGroup.DOFade(0, fadeoutDuration).SetUpdate(true);
        yield return new WaitForSecondsRealtime(fadeoutDuration);
        DisablePanel();
        CR_FadeoutTimer = null;
    }

}
