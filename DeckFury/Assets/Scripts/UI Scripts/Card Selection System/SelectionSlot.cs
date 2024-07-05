using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is the script that conrtols the selection card slots in Card Selection section of the card selection menu.
/// </summary>
[RequireComponent(typeof(Button))]
public class SelectionSlot : MonoBehaviour
{
    [SerializeReference] CardObjectReference _cardObjectReference;
    public CardObjectReference CardObjectReference {get => _cardObjectReference;}

    [SerializeField] Image _cardImage;
    [SerializeField] TextMeshProUGUI _ammoCounter;

    [SerializeField] Image _rechargeIndicator;
    [SerializeField] TextMeshProUGUI _rechargeCounter;

    [SerializeField] Image _grayoutOverlay;

    [SerializeField] Button _button;
    public Button SlotButton {get => _button;}

    bool CurrentlyTransferring = false;
    LoadSlot currentLoadSlot; // The load slot that this selection slot is currently transferring its card to.

    public void Awake()
    {
        _cardImage.enabled = false;
        _button.interactable = false;
    }

    public void SetCardObjectReference(CardObjectReference cardObjectReference)
    {
        if(_cardObjectReference != null)
        {
            SubToRechargeEvents(false);
        }

        _cardObjectReference = cardObjectReference;
        _cardImage.sprite = cardObjectReference.cardSO.GetCardImage();
        _cardImage.enabled = true;
        _button.interactable = true;

        SubToRechargeEvents(true);

        UpdateUIElements();
    }

    void SubToRechargeEvents(bool subscribe = true)
    {
        if(subscribe)
        {
            _cardObjectReference.OnBeginRecharge += BeginRecharge;
            _cardObjectReference.OnCycleRechargeCounter += CycleRechargeCounter;
            _cardObjectReference.OnFinishRecharge += FinishRecharge;
        }else
        {
            _cardObjectReference.OnBeginRecharge -= BeginRecharge;
            _cardObjectReference.OnCycleRechargeCounter -= CycleRechargeCounter;
            _cardObjectReference.OnFinishRecharge -= FinishRecharge;
        }
    }

    private void BeginRecharge()
    {
        _rechargeIndicator.fillAmount = 1;
        _rechargeCounter.text = _cardObjectReference.CurrentRechargeTurns.ToString();
        _rechargeCounter.gameObject.SetActive(true);
    }

    private void CycleRechargeCounter()
    {
        _rechargeIndicator.fillAmount = 1 - ((float)_cardObjectReference.CurrentRechargeTurns / _cardObjectReference.RechargeRate);
        _rechargeCounter.text = _cardObjectReference.CurrentRechargeTurns.ToString();
    }

    private void FinishRecharge()
    {
        _rechargeIndicator.fillAmount = 0;
        _rechargeCounter.gameObject.SetActive(false);
        _ammoCounter.text = _cardObjectReference.CurrentAmmoCount.ToString();
    }

    public void ClearSlot()
    {
        if(_cardObjectReference != null)
        {
            SubToRechargeEvents(false);
        }

        _cardObjectReference = null;
        _cardImage.sprite = null;
        _cardImage.enabled = false;
        _ammoCounter.text = "";
        _rechargeCounter.text = "";
        _rechargeIndicator.fillAmount = 0;
        _rechargeCounter.gameObject.SetActive(false);
    }

    public void DisableSlot()
    {
        _button.interactable = false;
        _grayoutOverlay.enabled = true;
    }

    public void EnableSlot()
    {
        if(_cardObjectReference == null){return;}
        if(_cardObjectReference.CurrentAmmoCount == 0){return;}
        if(_cardObjectReference.RechargeInProgress){return;}

        _button.interactable = true;
        _grayoutOverlay.enabled = false;
    }

    public void UpdateUIElements()
    {
        if(_cardObjectReference == null){return;}

        if(_cardObjectReference.CurrentAmmoCount == -1)
        {
            _ammoCounter.text = "INF";
        }else
        {
            _ammoCounter.text = _cardObjectReference.CurrentAmmoCount.ToString();
        }

        if(_cardObjectReference.RechargeInProgress)
        {
            _rechargeIndicator.fillAmount = 1 - ((float)_cardObjectReference.CurrentRechargeTurns / _cardObjectReference.RechargeRate);
            _rechargeCounter.text = _cardObjectReference.CurrentRechargeTurns.ToString();
            _rechargeCounter.gameObject.SetActive(true);
        }else
        {
            _rechargeIndicator.fillAmount = 0;
            _rechargeCounter.gameObject.SetActive(false);
        }

        if(_cardObjectReference.CurrentAmmoCount == 0)
        {
            ToggleInteractable(false);
        }else
        {
            // If the card is not currently transferring to a load slot, then the slot should be interactable as long as it still has ammo.
            if(!CurrentlyTransferring)
            {
                ToggleInteractable(true);
            }else
            {
                ToggleInteractable(false);
            }
        }

    }

    public void ToggleInteractable(bool condition)
    {
        _button.interactable = condition;
        _grayoutOverlay.gameObject.SetActive(!condition);
    }

    public void TransferToLoadSlot(LoadSlot loadSlot)
    {
        if(_cardObjectReference == null){return;}
        if(_cardObjectReference.CurrentAmmoCount == 0){return;}

        loadSlot.SetLoadSlot(this);
        _cardObjectReference.DecrementAmmoCount();

        currentLoadSlot = loadSlot;
        CurrentlyTransferring = true;
        UpdateUIElements();
    }

    public void RescindCardTransfer()
    {
        if(!CurrentlyTransferring)
        {
            Debug.LogWarning("Cannot rescind card transfer. No card is currently transferring.");
            return;
        }

        CurrentlyTransferring = false;
        currentLoadSlot = null;

        _cardObjectReference.IncrementAmmoCount();
        UpdateUIElements();
    }
    
    /// <summary>
    /// Completes the transfer to the current load slot and severs connection to the load slot.
    /// </summary>
    public void CompleteTransfer()
    {
        if(!CurrentlyTransferring)
        {
            Debug.LogWarning("Cannot complete card transfer. No card is currently transferring.");
            return;
        }
        currentLoadSlot.ClearSlot();
        CurrentlyTransferring = false;
        currentLoadSlot = null;
        UpdateUIElements();
    }

}
