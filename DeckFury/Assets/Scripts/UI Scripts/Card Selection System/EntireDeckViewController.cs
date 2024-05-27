using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EntireDeckViewController : MonoBehaviour
{
    event Action OnFinishInitialization;

    [SerializeField] CardDescriptionPanel cardDescriptionPanel;
    [SerializeField] GameObject cardSlotPrefab;
    [SerializeField] GameObject cardSlotsParent;
    [SerializeField] List<CardSlot> cardSlots = new List<CardSlot>();

    CardPoolManager cardPoolManager;
    PersistentLevelController PLC;

    EventBinding<SelectStartingDeckEvent> selectStartingDeckEventBinding;

    EventBinding<PlayerDataModifiedEvent> playerDataModifiedEventBinding;

    [SerializeField] bool useCardPoolManager = true;


    void Awake()
    {
        OnFinishInitialization += () => gameObject.SetActive(false);

        if(useCardPoolManager)
        {
            cardPoolManager = CardPoolManager.Instance;
            cardPoolManager.OnCompletePooling += InitializeCardSlots;
        }else
        {
            PLC = PersistentLevelController.Instance;

            selectStartingDeckEventBinding = new EventBinding<SelectStartingDeckEvent>(AssignNewDeck);
            EventBus<SelectStartingDeckEvent>.Register(selectStartingDeckEventBinding);            

            playerDataModifiedEventBinding = new EventBinding<PlayerDataModifiedEvent>(UpdateDeckView);
            EventBus<PlayerDataModifiedEvent>.Register(playerDataModifiedEventBinding);
        }
    }


    void Start()
    {
        
    }

    void OnDestroy()
    {
        if(!useCardPoolManager)
        {
            EventBus<SelectStartingDeckEvent>.Deregister(selectStartingDeckEventBinding);
            EventBus<PlayerDataModifiedEvent>.Deregister(playerDataModifiedEventBinding);
        }
    }

    void AssignNewDeck(SelectStartingDeckEvent selectDeckEvent)
    {
        InitializeCardSlots();
    }

    void UpdateDeckView(PlayerDataModifiedEvent playerDataModifiedEvent)
    {
        if(playerDataModifiedEvent.dataType == PlayerDataContainer.PlayerDataType.CurrentDeck)
        {
            InitializeCardSlots();
        }

    }

    void InitializeCardSlots()
    {
        foreach (Transform child in cardSlotsParent.transform)
        {
            Destroy(child.gameObject);
        }
        cardSlots.Clear();

        int numberOfCards;

        if(useCardPoolManager)
        {
            numberOfCards = cardPoolManager.CardObjectReferences.Count;
        }else
        {
            numberOfCards = PLC.PlayerData.CurrentDeck.TotalCards;
        }


        for (int i = 0; i < numberOfCards; i++)
        {
            GameObject newCardSlot = Instantiate(cardSlotPrefab, cardSlotsParent.transform);
            CardSlot cardSlot = newCardSlot.GetComponent<CardSlot>();

            Button cardSlotButton = cardSlot.GetComponent<Button>();

            cardSlotButton.onClick.AddListener(TestMethod);

            //Dynamically add event triggers to the card slot
            EventTrigger cardSlotEventTrigger = cardSlot.GetComponent<EventTrigger>();
            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnterEntry.callback.AddListener((data) => { cardDescriptionPanel.UpdateDescription(cardSlot); });
            cardSlotEventTrigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            pointerExitEntry.callback.AddListener((data) => { cardDescriptionPanel.BeginFadeOut();});
            cardSlotEventTrigger.triggers.Add(pointerExitEntry);

            if(useCardPoolManager)
            {
                cardSlot.Initialize(cardPoolManager.CardObjectReferences[i]);
            }

            cardSlots.Add(cardSlot);
        }

        //If we are not using the card pool manager (i.e, we are using the deck view from Stage Select),
        //we need to populate the card slots with the cards directly from the player's deck
        //This is a rather clumsy patch for this issue, but it works for now 
        if(!useCardPoolManager)
        {
            int cardSlotIndex = 0;

            foreach(var deckElement in PLC.PlayerData.CurrentDeck.CardList)
            {
                for(int i = 0; i < deckElement.cardCount; i++)
                {
                    cardSlots[cardSlotIndex].Initialize(deckElement.card);
                    cardSlotIndex++;
                }
            }
        }

        OnFinishInitialization?.Invoke();        
    }

    CardSlot BuildNewCardSlot()
    {
        GameObject newCardSlot = Instantiate(cardSlotPrefab, cardSlotsParent.transform);
        CardSlot cardSlot = newCardSlot.GetComponent<CardSlot>();

        Button cardSlotButton = cardSlot.GetComponent<Button>();

        cardSlotButton.onClick.AddListener(TestMethod);

        //Dynamically add event triggers to the card slot
        EventTrigger cardSlotEventTrigger = cardSlot.GetComponent<EventTrigger>();

        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnterEntry.callback.AddListener((data) => { cardDescriptionPanel.UpdateDescription(cardSlot); });
        cardSlotEventTrigger.triggers.Add(pointerEnterEntry);

        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExitEntry.callback.AddListener((data) => { cardDescriptionPanel.BeginFadeOut();});
        cardSlotEventTrigger.triggers.Add(pointerExitEntry);        

        cardSlots.Add(cardSlot);

        return cardSlot;
    }



    void TestMethod()
    {
        print("Test Method");
    }

}
