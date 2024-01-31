using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EntireDeckViewController : MonoBehaviour
{
    [SerializeField] CardDescriptionPanel cardDescriptionPanel;
    [SerializeField] GameObject cardSlotPrefab;
    [SerializeField] GameObject cardSlotsParent;
    [SerializeField] List<CardSlot> cardSlots = new List<CardSlot>();

    CardPoolManager cardPoolManager;


    void Awake()
    {
        cardPoolManager = CardPoolManager.Instance;
        cardPoolManager.OnCompletePooling += InitializeCardSlots;
    }


    void Start()
    {
        
    }

    void InitializeCardSlots()
    {
        for (int i = 0; i < cardPoolManager.CardObjectReferences.Count; i++)
        {
            GameObject newCardSlot = Instantiate(cardSlotPrefab, cardSlotsParent.transform);
            CardSlot cardSlot = newCardSlot.GetComponent<CardSlot>();
            cardSlot.Initialize(cardPoolManager.CardObjectReferences[i]);

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
        }        
    }

    void TestMethod()
    {
        print("Test Method");
    }

}
