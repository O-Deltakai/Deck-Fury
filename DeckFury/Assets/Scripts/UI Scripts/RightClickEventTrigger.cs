using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickEventTrigger : MonoBehaviour
{

    [SerializeField] CardDescriptionPanel cardDescriptionPanel;
    CardSlot cardSlot;

    public void HandleEventData(BaseEventData eventData)
    {
        PointerEventData pointerEventData = (PointerEventData)eventData;

        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            cardSlot = GetComponent<CardSlot>();
            if(cardSlot.IsEmpty()){return;}
            if(cardDescriptionPanel != null)
            {
                cardDescriptionPanel.LockPanelInPlace();
            }

        }
    }

}