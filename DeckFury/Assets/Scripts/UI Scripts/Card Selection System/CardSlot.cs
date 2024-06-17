using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Handles functionality of the individual card buttons seen on the CardSelectionMenu
public class CardSlot : MonoBehaviour
{
    [HideInInspector]
    public int slotIndex;

    public CardObjectReference cardObjectReference;
    public CardSO cardSO;

    public bool initializedWithCardSO {get; private set;} = false;

    [SerializeField] Image cardImage;


    public Button button {get; private set;}
    public CardSlot previousTransfererSlot;

[Header("Audio Clips")]
    [SerializeField] EventReference onHoverSFX;
    [SerializeField] EventReference onClickSFX;
    EventInstance onHoverSFXInstance;
    EventInstance onClickSFXInstance;

[Header("Ammo Status Elements")]
    [SerializeField] TextMeshProUGUI _currentAmmoText;
    [SerializeField] TextMeshProUGUI _rechargeTimerText;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    void Start()
    {
        button = GetComponent<Button>();
        if(IsEmpty() && !initializedWithCardSO)
        {
            DisableImage();
            button.interactable = false;    
        }

        onHoverSFXInstance = RuntimeManager.CreateInstance(onHoverSFX);
        onClickSFXInstance = RuntimeManager.CreateInstance(onClickSFX);
    }

    public void Initialize(CardObjectReference card)
    {
        if(card == null){return;}
        ChangeCard(card);
    }

    public void Initialize(CardSO card)
    {
        if(card == null)
        {
            Debug.LogWarning("Attempted to initialize null CardSO in CardSlot. Aborting...");    
            return;
        }
        initializedWithCardSO = true;
        ChangeCard(card);
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
            onHoverSFXInstance.start();
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
        if(button) button.interactable = false;



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
        if(button) button.interactable = false;
    }

    //Change CardObjectReference in card slot to a given card and update the image.
    public void ChangeCard(CardObjectReference card)
    {
        if(card == null){return;}
        cardObjectReference = card;
        card.cardSlot = this;
        if(button) button.interactable = true;
        UpdateImage();
    }

    public void ChangeCard(CardSO otherCard)
    {
        cardSO = otherCard;
        if(button) button.interactable = true;
        UpdateImage(otherCard);
    }


    public void ChangeCard(CardObjectReference card, bool interactable)
    {
        if(card == null){return;}
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
        if(button) button.interactable = false;
    }

    public void UpdateImage()
    {
        cardImage.enabled = true;
        cardImage.sprite = cardObjectReference.cardSO.GetCardImage();
    }

    public void UpdateImage(CardSO otherCardSO)
    {
        cardImage.sprite = otherCardSO.GetCardImage();
        cardImage.enabled = true;
        print("Updated image");

    }

    public void DisableImage()
    {
        cardImage.enabled = false;
    }

    public bool IsEmpty()
    {
        if(initializedWithCardSO)
        {
            if(cardSO == null)
            {
                return true;
            }else
            {
                return false;
            }
        }

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
