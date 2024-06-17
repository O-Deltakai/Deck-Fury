using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A data container class that is used to store references to pooled card objects and their effect prefab/object summons in the scene.
//Mainly used by the card selection system and card usage system to get references to the correct effect prefab of a card in order to activate it.
[System.Serializable]
public class CardObjectReference
{
    public CardSO cardSO;
    public GameObject effectPrefab;
    public CardEffect CardEffect{get => effectPrefab.GetComponent<CardEffect>();}
    public List<GameObject> objectSummonPrefabs = new List<GameObject>();

    public int MaxAmmoCount => cardSO.BaseAmmo;
    public int currentAmmoCount;

    public int RechargeRate => cardSO.RechargeRate;
    /// <summary>
    /// How many turns have passed since the card began recharging.
    /// </summary>
    public int CurrentRechargeTurns {get; private set;} = 0;

    public bool ReloadInProgress { get; private set; } = false;



    //Reference to which card slot on the card selection menu this CardObjectReference belongs to. Should only be set if the card is
    //meant to be on the menu.
    public CardSlot cardSlot;

    /// <summary>
    /// If true, the card will not show up in the card selection menu.
    /// </summary>
    public bool invisible = false;

    public void ClearReferences()
    {
        cardSO = null;
        effectPrefab = null;
        objectSummonPrefabs = null;
    }



}
