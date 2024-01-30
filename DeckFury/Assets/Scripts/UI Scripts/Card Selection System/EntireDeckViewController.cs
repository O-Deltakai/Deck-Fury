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

            EventTrigger cardSlotEventTrigger = cardSlot.GetComponent<EventTrigger>();
            cardSlotEventTrigger.triggers.Add(new EventTrigger.Entry{eventID = EventTriggerType.PointerEnter, callback = new EventTrigger.TriggerEvent()});


            cardSlots.Add(cardSlot);
        }        
    }

    void TestMethod()
    {
        print("Test Method");
    }

}
