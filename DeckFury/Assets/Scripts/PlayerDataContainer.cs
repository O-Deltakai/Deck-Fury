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
    [SerializeField] int _currentHP = 200;
    public int CurrentHP{get{ return _currentHP; }
        set
        {
            _currentHP = value;
            OnPlayerDataModified?.Invoke();
        }
    
    }


    public GameDeck CurrentDeck;
    [SerializeField] int _currentMoney = 0;
    public int CurrentMoney{get{ return _currentMoney; }
        set
        {
            _currentMoney = value;
            OnPlayerDataModified?.Invoke();
        }
    }


    public int CurrentScore = 0;

    public int CurrentStageLevelIndex = 0;

    //These Player stats are set base values that can be increased or decreased during battle, but get reset to these base values after the stage
    [SerializeField] int _baseShieldHP = 20;
    public int BaseShieldHP => _baseShieldHP;


    [SerializeField] int _baseArmor = 0;
    public int BaseArmor => _baseArmor;


    [SerializeField] double _baseDefense = 1;
    public double BaseDefense => _baseDefense;


    [SerializeField] int _maxHP = 200;
    public int MaxHP => _maxHP;

    [SerializeField] List<ItemBase> _items = new List<ItemBase>();
    public IReadOnlyList<ItemBase> Items => _items;

    public List<ItemBase> GetItemList()
    {
        return _items;
    }

    public void AddItem(ItemBase item)
    {
        _items.Add(item);
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
        _baseShieldHP = value;
        OnPlayerDataModified?.Invoke();

    }
    public void SetBaseArmor(int value)
    {
        _baseArmor = value;
        OnPlayerDataModified?.Invoke();

    }
    public void SetBaseDefense(double value)
    {
        _baseDefense = value;
        OnPlayerDataModified?.Invoke();

    }
    public void SetMaxHP(int value)
    {
        _maxHP = value;
        OnPlayerDataModified?.Invoke();

    }

}

