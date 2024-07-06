using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A data container class that is used to store references to pooled card objects and their effect prefab/object summons in the scene.
//Mainly used by the card selection system and card usage system to get references to the correct effect prefab of a card in order to activate it.
[System.Serializable]
public class CardObjectReference
{
    public Action OnBeginRecharge;
    public Action OnCycleRechargeCounter;
    public Action OnFinishRecharge;

    public Action OnAmmoChange;

    public CardSO cardSO;
    public GameObject effectPrefab;
    public CardEffect CardEffect{get => effectPrefab.GetComponent<CardEffect>();}
    public List<GameObject> objectSummonPrefabs = new List<GameObject>();

    public int MaxAmmoCount {get; private set;}
    int _currentAmmoCount;
    public int CurrentAmmoCount {get{return _currentAmmoCount;} 
        private set
        {
            if(_currentAmmoCount == value) { return; }
            _currentAmmoCount = value;
            OnAmmoChange?.Invoke();
        }
    }

    public int RechargeRate {get; private set;}
    public int RechargeAmount {get; private set;}

    /// <summary>
    /// The remaining number of turns until the card is recharged.
    /// </summary>
    public int RechargeTurnsRemaining {get; private set;} = 0;
    public bool RechargeInProgress { get; private set; } = false;

    //Reference to which card slot on the card selection menu this CardObjectReference belongs to. Should only be set if the card is
    //meant to be on the menu.
    public CardSlot cardSlot;

    /// <summary>
    /// If true, the card will not show up in the card selection menu.
    /// </summary>
    public bool invisible = false;

    bool outOfAmmo = false;

    CardSelectionMenu cardSelectionMenu;

    public void InitializeCardObjectReference(CardSO card, CardEffect concreteEffectPrefab)
    {
        if(cardSelectionMenu)
        {
            cardSelectionMenu.OnMenuDisabled -= TryRecharge;
        }

        cardSO = card;
        effectPrefab = concreteEffectPrefab.gameObject;
        CardEffect cardEffect = effectPrefab.GetComponent<CardEffect>();
        cardEffect.cardObjectReference = this;

        objectSummonPrefabs = cardSO.ObjectSummonList;

        //Ammo Stats
        MaxAmmoCount = cardSO.BaseAmmo;
        RechargeRate = cardSO.RechargeRate;
        RechargeAmount = cardSO.RechargeAmount;

        CurrentAmmoCount = cardSO.BaseAmmo;
        RechargeInProgress = false;
        invisible = false;
        
        cardSelectionMenu = CardSelectionMenu.Instance;
        cardSelectionMenu.OnMenuDisabled += TryRecharge;
    }

    void TryRecharge()
    {
        if(CurrentAmmoCount == -1) { return; }
        if(RechargeAmount == 0) { return; }  //If the card has infinite ammo, it will not be affected by this check.
        if(RechargeRate == 0) { return; }

        if(outOfAmmo && !RechargeInProgress)
        {
            BeginRecharge();
        }else if(outOfAmmo && RechargeInProgress)
        {
            CycleRechargeCounter();
        }
    }

    public void BeginRecharge()
    {
        if(RechargeInProgress) { return; }
        if(CurrentAmmoCount == -1) { return; }

        RechargeInProgress = true;
        RechargeTurnsRemaining = RechargeRate;
        OnBeginRecharge?.Invoke();
    }

    /// <summary>
    /// Cycles the recharge counter of the card. If the card is not currently recharging, this method does nothing.
    /// </summary>
    public void CycleRechargeCounter()
    {
        if(!RechargeInProgress) { return; }

        RechargeTurnsRemaining--;
        if(RechargeTurnsRemaining == 0)
        {
            FinishRecharge();
        }else
        {
            OnCycleRechargeCounter?.Invoke();
        }
    }

    public void FinishRecharge()
    {
        RechargeInProgress = false;
        CurrentAmmoCount += RechargeAmount;
        outOfAmmo = false;
        OnFinishRecharge?.Invoke();
    }

    void CancelRecharge()
    {
        RechargeInProgress = false;
        RechargeTurnsRemaining = 0;
        OnFinishRecharge?.Invoke();
    }

    public void DecrementAmmoCount()
    {
        if(CurrentAmmoCount == -1) { return; }
        if(RechargeInProgress) { return; }
        if(CurrentAmmoCount == 0) { return; }  //If the card has infinite ammo, it will not be affected by this check.

        CurrentAmmoCount--;
        if(CurrentAmmoCount == 0)
        {
            outOfAmmo = true;
        }
    }

    public void IncrementAmmoCount()
    {
        if(CurrentAmmoCount == -1) { return; }
        if(RechargeInProgress) { return; }
        if(CurrentAmmoCount == MaxAmmoCount) { return; }

        CurrentAmmoCount++;
        outOfAmmo = false;
    }

    public void ForceRestoreAmmo(int amount)
    {
        if(CurrentAmmoCount == -1) { return; }
        if(amount < 1) { return; }

        if(RechargeInProgress)
        {
            CancelRecharge();
        }

        CurrentAmmoCount += amount;
        if(CurrentAmmoCount > MaxAmmoCount)
        {
            CurrentAmmoCount = MaxAmmoCount;
        }
        outOfAmmo = false;
    }

    public void ClearReferences()
    {
        cardSO = null;
        effectPrefab = null;
        objectSummonPrefabs = null;
    }



}
