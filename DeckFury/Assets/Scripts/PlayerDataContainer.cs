using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to store player stat information that should be persistent across scenes
/// </summary>
[System.Serializable]
public class PlayerDataContainer
{

    public delegate void DataModifiedEventHandler();
    public event DataModifiedEventHandler OnPlayerDataModified;

    //Current values can change during and after battle, and will remain persistent across stages.
    public int CurrentHP = 200;
    public GameDeck CurrentDeck;
    public int CurrentMoney = 0;
    public int CurrentScore = 0;
    // public int CurrentScore 
    // {
    //     get{return CurrentScore;}
    //     set
    //     {
    //         CurrentScore = value;
    //         OnPlayerDataModified?.Invoke();
            
    //     }

    // }
    public int CurrentStageLevelIndex = 0;

    //These Player stats are set base values that can be increased or decreased during battle, but get reset to these base values after the stage
    [field:SerializeField] public int BaseShieldHP{get; private set;} = 20;
    [field:SerializeField] public int BaseArmor{get; private set;} = 0;
    [field:SerializeField] public double BaseDefense{get; private set;} = 1;
    [field:SerializeField] public int MaxHP{get; private set;} = 200;

    [SerializeField] List<ItemSO> _items = new List<ItemSO>();
    public IReadOnlyList<ItemSO> Items => _items;


    public void AddItem(ItemSO item, int amount)
    {
        
    }

    public DeckElement AddCardToDeck(CardSO card, int cardCount)
    {
        DeckElement deckElement = new DeckElement()
        {
            card = card,
            cardCount = cardCount
        };
        CurrentDeck.CardList.Add(deckElement);
        OnPlayerDataModified?.Invoke();
        return deckElement;
    }

    public void SetBaseShieldHP(int value)
    {
        BaseShieldHP = value;
        OnPlayerDataModified?.Invoke();

    }
    public void SetBaseArmor(int value)
    {
        BaseArmor = value;
    }
    public void SetBaseDefense(double value)
    {
        BaseDefense = value;
    }
    public void SetMaxHP(int value)
    {
        MaxHP = value;
    }

}

