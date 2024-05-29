using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Manages the deck view in the upgrade screen
/// </summary>
public class DeckViewManager : MonoBehaviour
{

    [SerializeField] GameObject cardSlotPrefab;
    [SerializeField] GameObject cardSlotsParent;
    [SerializeField] List<CardSlot> cardSlots = new List<CardSlot>();

    public CardDescriptionPanel selectedCardPanel;

    PersistentLevelController PLC;

    [SerializeField] bool stageDebugMode = false;

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

        List<DeckElement> upgradeableCards = deckSO.GetUpgradableCards();


        foreach (var deckElement in deckSO.GetUpgradableCards())
        {
            for (int i = 0; i < deckElement.cardCount; i++)
            {
                BuildNewCardSlot(deckElement.card);
            }
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
        pointerClickEntry.callback.AddListener((data) => { selectedCardPanel.UpdateDescription(cardSlot); });
        cardSlotEventTrigger.triggers.Add(pointerClickEntry);      

        cardSlots.Add(cardSlot);

        if(cardSO != null)
        {
            cardSlot.Initialize(cardSO);
        }
        

        return cardSlot;
    }



}
