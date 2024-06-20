using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script controls the card slots in the Card Magazine section of the card selection menu.
/// </summary>
public class LoadSlot : MonoBehaviour
{
    public int slotIndex;

    [SerializeReference] CardObjectReference _cardObjectReference;

    [Tooltip("The selection slot that is currently transferring its card to this load slot.")]
    [SerializeReference] SelectionSlot _transferringSelectionSlot;

    public CardObjectReference CardObjectReference {get => _cardObjectReference;}
    CardSO CardSO => _cardObjectReference.cardSO;

[Header("UI Elements")]
    [SerializeField] Image _cardImage;
    [SerializeField] Button _button;
    public Button SlotButton {get => _button;}


    public void SetLoadSlot(SelectionSlot selectionSlot)
    {
        _transferringSelectionSlot = selectionSlot;
        _cardObjectReference = selectionSlot.CardObjectReference;
        _cardImage.sprite = _cardObjectReference.cardSO.GetCardImage();
        _cardImage.enabled = true;

        _button.interactable = true;
    }

    /// <summary>
    /// Transfers the card and its transferring selection slot in this load slot to another load slot.
    /// </summary>
    /// <param name="otherLoadSlot"></param>
    public void TransferToOtherLoadSlot(LoadSlot otherLoadSlot)
    {
        if(_transferringSelectionSlot == null) { return; }

        otherLoadSlot.SetLoadSlot(_transferringSelectionSlot);
        ClearSlot();
    }

    public void ReturnCardToSelectionSlot()
    {
        if(_transferringSelectionSlot == null) { return; }
        if(_cardObjectReference == null) { return; } 

        _cardObjectReference.IncrementAmmoCount();
        _transferringSelectionSlot.UpdateUIElements();
        ClearSlot();
    }

    public void ClearSlot()
    {
        _cardObjectReference = null;
        _transferringSelectionSlot = null;
        _cardImage.sprite = null;
        _cardImage.enabled = false;

        _button.interactable = false;
    }

}
