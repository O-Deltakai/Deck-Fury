using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

//Handles functionality of the individual card buttons seen on the CardSelectionMenu
public class CardSlot : MonoBehaviour
{
    [HideInInspector]
    public int slotIndex;

    public CardObjectReference cardObjectReference;

    [SerializeField] Image cardImage;
    [SerializeField] GameObject popOutPanel;

    public Button button {get; private set;}
    public CardSlot previousTransfererSlot;

[Header("Audio Clips")]
    [SerializeField] EventReference onHoverSFX;
    [SerializeField] EventReference onClickSFX;


    private void Awake()
    {
        button = GetComponent<Button>();
    }

    void Start()
    {
        if(IsEmpty())
        {
            DisableImage();
            button.interactable = false;    
        }

    }

    void Update()
    {
        
    }

    public void OnClick()
    {
        
    }

    public void OnHover()
    {
        if(button.interactable)
        {
            RuntimeManager.PlayOneShot(onHoverSFX);
        }

    }


    //Takes another card slot as the parameter and transfers the card on this card slot to the other card slot. 
    //Sets the previousTransfererSlot to this CardSlot which allows for card slots to remember which card slot transferred a card to them last.
    //Clears this card slot afterwards. Also returns the CardObjectReference that was transferred.
    public CardObjectReference TransferCardToSlot(CardSlot otherSlot)
    {
        CardObjectReference cardToTransfer = cardObjectReference;

        otherSlot.ChangeCard(cardToTransfer);
        otherSlot.previousTransfererSlot = this;
        otherSlot.button.interactable = true; 

        ClearCardSlot();
        button.interactable = false;



        return cardToTransfer;
    }

    //Choose a card slot to completely replace with the properties of this card slot, including previousTransfererSlot
    public void ReplaceCardSlot(CardSlot otherSlot)
    {
        CardObjectReference cardToMove = cardObjectReference;
        otherSlot.ChangeCard(cardToMove);
        otherSlot.previousTransfererSlot = previousTransfererSlot;
        otherSlot.button.interactable = true; 

        ClearCardSlot();
        button.interactable = false;
    }

    //Change CardObjectReference in card slot to a given card and update the image.
    public void ChangeCard(CardObjectReference card)
    {
        cardObjectReference = card;
        card.cardSlot = this;
        button.interactable = true;
        UpdateImage();
    }

    public void ChangeCard(CardObjectReference card, bool interactable)
    {
        cardObjectReference = card;
        card.cardSlot = this;
        button.interactable = interactable;
        UpdateImage();
    }


    public void ClearCardSlot()
    {
        cardObjectReference = null;
        cardImage.sprite = null;
        cardImage.enabled = false;
        button.interactable = false;
    }

    public void UpdateImage()
    {
        cardImage.enabled = true;
        cardImage.sprite = cardObjectReference.cardSO.GetCardImage();
    }

    public void DisableImage()
    {
        cardImage.enabled = false;
    }

    public bool IsEmpty()
    {
        if(cardObjectReference == null)
        {
            return true;
        }else
        if(cardObjectReference.cardSO == null)
        {
            return true;
        }


        return false;
    }

}
