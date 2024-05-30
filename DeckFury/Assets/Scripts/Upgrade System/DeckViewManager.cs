using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using FMODUnity;
using System;

/// <summary>
/// Manages the deck view in the upgrade screen
/// </summary>
public class DeckViewManager : MonoBehaviour
{
    public event Action OnCardSlotSelected;

    [SerializeField] GameObject cardSlotPrefab;
    [SerializeField] GameObject cardSlotsParent;
    [SerializeField] List<CardSlot> cardSlots = new List<CardSlot>();

    public CardDescriptionPanel selectedCardDescriptionPanel;

    PersistentLevelController PLC;

    [SerializeField] bool stageDebugMode = false;
    CardSlot currentlySelectedSlot;


    [Header("Selector Indicator Settings")]
    [SerializeField] GameObject selectorIndicator;
    [SerializeField] float moveSpeed = 0.1f;
    [SerializeField] Ease easeType = Ease.OutCirc;
    Tween currentMoveTween;

    [Header("SFX")]
    [SerializeField] EventReference onPointerClickSFX;


    void Start()
    {
        if(stageDebugMode)
        {
            InitializeCardSlots(CardPoolManager.Instance.DefaultDeck);
        }

        PLC = PersistentLevelController.Instance;
        if(PLC)
        {
            InitializeCardSlots();
        }
    }

    void Update()
    {
        
    }

    void InitializeCardSlots()
    {
        selectorIndicator.transform.SetParent(transform);
        selectorIndicator.SetActive(false);

        foreach (Transform child in cardSlotsParent.transform)
        {
            Destroy(child.gameObject);
        }
        cardSlots.Clear();

        List<DeckElement> upgradeableCards = PLC.PlayerData.CurrentDeck.GetUpgradableCards();

        foreach (var deckElement in upgradeableCards)
        {
            for (int i = 0; i < deckElement.cardCount; i++)
            {
                BuildNewCardSlot(deckElement.card);
            }
        }

    }

    void InitializeCardSlots(DeckSO deckSO)
    {
        foreach (Transform child in cardSlotsParent.transform)
        {
            Destroy(child.gameObject);
        }
        cardSlots.Clear();
        currentlySelectedSlot = null;

        List<DeckElement> upgradeableCards = deckSO.GetUpgradableCards();


        foreach (var deckElement in deckSO.GetUpgradableCards())
        {
            for (int i = 0; i < deckElement.cardCount; i++)
            {
                BuildNewCardSlot(deckElement.card);
            }
        }
    }

    public void RemoveCardSlot(CardSlot cardSlot)
    {
        cardSlots.Remove(cardSlot);
        Destroy(cardSlot.gameObject);
    }

    public void RemoveCurrentCardSlot()
    {
        if(currentlySelectedSlot != null)
        {
            RemoveCardSlot(currentlySelectedSlot);
            currentlySelectedSlot = null;
            selectorIndicator.transform.SetParent(transform);
            selectorIndicator.SetActive(false);
            OnCardSlotSelected?.Invoke();
        }
    }

    CardSlot BuildNewCardSlot(CardSO cardSO = null)
    {
        GameObject newCardSlot = Instantiate(cardSlotPrefab, cardSlotsParent.transform);
        CardSlot cardSlot = newCardSlot.GetComponent<CardSlot>();

        Button cardSlotButton = cardSlot.GetComponent<Button>();

        //Dynamically add event triggers to the card slot
        EventTrigger cardSlotEventTrigger = cardSlot.GetComponent<EventTrigger>();

        EventTrigger.Entry pointerClickEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        pointerClickEntry.callback.AddListener((data) => { selectedCardDescriptionPanel.UpdateDescription(cardSlot); });
        pointerClickEntry.callback.AddListener((data) => { SelectCardSlot(cardSlot); });
        cardSlotEventTrigger.triggers.Add(pointerClickEntry);      

        cardSlots.Add(cardSlot);

        if(cardSO != null)
        {
            cardSlot.Initialize(cardSO);
        }
        

        return cardSlot;
    }

    public void SelectCardSlot(CardSlot cardSlot)
    {
        if(currentlySelectedSlot == cardSlot)
        {
            return;
        }

        currentlySelectedSlot = cardSlot;

        if(currentMoveTween.IsActive())
        {
            currentMoveTween.Kill();
        }
        selectorIndicator.SetActive(true);

        selectorIndicator.transform.SetParent(cardSlot.transform);
        selectorIndicator.transform.SetAsFirstSibling();

        currentMoveTween = selectorIndicator.transform.DOMove(cardSlot.transform.position, moveSpeed).SetEase(easeType).SetUpdate(true);
        RuntimeManager.PlayOneShot(onPointerClickSFX);

        OnCardSlotSelected?.Invoke();
    }


}
