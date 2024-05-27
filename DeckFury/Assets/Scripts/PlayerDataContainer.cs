using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to store player stat information that should be persistent across scenes
/// </summary>
[System.Serializable]
public class PlayerDataContainer
{
    /// <summary>
    /// Enum used to specify the type of player data that is being modified
    /// </summary>
    public enum PlayerDataType
    {
        CurrentHP,
        CurrentMoney,
        BaseShieldHP,
        BaseArmor,
        BaseDefense,
        MaxHP,
        CurrentScore,
        CurrentDeck
    }


    public delegate void DataModifiedEventHandler();
    public event DataModifiedEventHandler OnPlayerDataModified;

    public delegate void ItemAddedEventHandler(ItemBase item);
    public event ItemAddedEventHandler OnAddItemToPlayer;

    //Current values can change during and after battle, and will remain persistent across stages.
    [SerializeField] int _currentHP = 200;
    public int CurrentHP{get{ return _currentHP; }
        set
        {
            if(_currentHP == value) { return; }
            if(value > _maxHP)
            {
                _currentHP = _maxHP;
            }else
            if(value <= 0)
            {
                _currentHP = 1;
            }else
            {
                _currentHP = value;
            }
            OnPlayerDataModified?.Invoke();
            EventBus<PlayerDataModifiedEvent>.Raise(new PlayerDataModifiedEvent(this, PlayerDataType.CurrentHP));

        }
    
    }


    public GameDeck CurrentDeck;
    [SerializeField] int _currentMoney = 0;
    public int CurrentMoney{get{ return _currentMoney; }
        set
        {
            if(_currentMoney == value) { return; }
            if(value < 0)
            {
                _currentMoney = 0;
            }else
            {
                _currentMoney = value;
            }
            OnPlayerDataModified?.Invoke();
            EventBus<PlayerDataModifiedEvent>.Raise(new PlayerDataModifiedEvent(this, PlayerDataType.CurrentMoney));
        }
    }



    //These Player stats are set base values that can be increased or decreased during battle, but get reset to these base values after the stage
    [SerializeField] int _baseShieldHP = 20;
    public int BaseShieldHP => _baseShieldHP;


    [SerializeField] int _baseArmor = 0;
    public int BaseArmor => _baseArmor;
    [SerializeField] int _armorHardCap = 80;


    [SerializeField, Range(0.1f, 10f)] double _baseDefense = 1;
    public double BaseDefense => _baseDefense;


    [SerializeField] int _maxHP = 200;
    public int MaxHP => _maxHP;

    public int CurrentScore = 0;

    [SerializeField] List<ItemBase> _items = new List<ItemBase>();
    public IReadOnlyList<ItemBase> Items => _items;

    public List<ItemBase> GetItemList()
    {
        return _items;
    }

    public void AddItem(ItemBase item)
    {
        _items.Add(item);
        OnAddItemToPlayer?.Invoke(item);
    }

    public bool RemoveItem(ItemBase item)
    {
        return _items.Remove(item);
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
        EventBus<PlayerDataModifiedEvent>.Raise(new PlayerDataModifiedEvent(this, PlayerDataType.CurrentDeck));
        return deckElement;
    }

    /// <summary>
    /// Assigns a new deckSO to the player's current deck, clearing the current deck and adding the cards from the deckSO
    /// </summary>
    /// <param name="deckSO"></param>
    public void AssignDeck(DeckSO deckSO)
    {
        CurrentDeck = new GameDeck(deckSO);
        OnPlayerDataModified?.Invoke();
        EventBus<PlayerDataModifiedEvent>.Raise(new PlayerDataModifiedEvent(this, PlayerDataType.CurrentDeck));
    }

    public void SetBaseShieldHP(int value)
    {
        _baseShieldHP = value;
        OnPlayerDataModified?.Invoke();
        EventBus<PlayerDataModifiedEvent>.Raise(new PlayerDataModifiedEvent(this, PlayerDataType.BaseShieldHP));
    }
    public void SetBaseArmor(int value)
    {
        if(value < 0)
        {
            _baseArmor = 0;
        }else
        if(value > _armorHardCap)
        {
            _baseArmor = _armorHardCap;
        }else
        {
            _baseArmor = value;
        }

        OnPlayerDataModified?.Invoke();
        EventBus<PlayerDataModifiedEvent>.Raise(new PlayerDataModifiedEvent(this, PlayerDataType.BaseArmor));

    }
    public void SetBaseDefense(double value)
    {
        if(value < 0.1)
        {
            _baseDefense = 0.1;
        }else
        if(value > 10)
        {
            _baseDefense = 10;
        }else
        {
            _baseDefense = value;
        }

        OnPlayerDataModified?.Invoke();
        EventBus<PlayerDataModifiedEvent>.Raise(new PlayerDataModifiedEvent(this, PlayerDataType.BaseDefense));

    }
    public void SetMaxHP(int value)
    {
        if(value <= 0)
        {
            _maxHP = 1;
        }else
        {
            _maxHP = value;
        }

        if(CurrentHP > _maxHP)
        {
            CurrentHP = _maxHP;
        }

        OnPlayerDataModified?.Invoke();
        EventBus<PlayerDataModifiedEvent>.Raise(new PlayerDataModifiedEvent(this, PlayerDataType.MaxHP));

    }

}

