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
            _grayoutOverlay.enabled = true;
            _button.interactable = false;
        }else
        {
            _grayoutOverlay.enabled = false;
            _button.interactable = true;
        }

    }

    public void TransferToLoadSlot(LoadSlot loadSlot)
    {
        if(_cardObjectReference == null){return;}
        if(_cardObjectReference.CurrentAmmoCount == 0){return;}

        loadSlot.SetLoadSlot(this);
        _cardObjectReference.DecrementAmmoCount();
        UpdateUIElements();
    }

    


}
