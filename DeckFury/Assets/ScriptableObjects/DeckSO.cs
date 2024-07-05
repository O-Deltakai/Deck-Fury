using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckElement
{
    public CardSO card;
    public int cardCount;

    /// <summary>
    /// This property should only be set if the DeckElement is part of the GameDeck.
    /// </summary>
    public bool InLoadout {get; set;} = false;

}

[CreateAssetMenu(fileName = "Deck Data", menuName = "New Deck", order = 0)]
public class DeckSO : ScriptableObject
{
    public string DeckName;
    [field:SerializeField] List<DeckElement> CardList {get; set;}
    public IReadOnlyList<DeckElement> CardListReadOnly => CardList;

    

    public List<DeckElement> GetUpgradableCards()
    {
        List<DeckElement> upgradableCards = new List<DeckElement>();
        foreach(DeckElement deckElement in CardList)
        {
            if(deckElement.card.HasUpgrades)
            {
                upgradableCards.Add(deckElement);
            }
        }
        return upgradableCards;
    }


    [SerializeField] List<StatUnlockCondition> _unlockConditions;
    public IReadOnlyList<StatUnlockCondition> UnlockConditions => _unlockConditions;


}


/// <summary>
/// Deck that is used by the player in game and filled during run-time. Allows creation of temporary decks run-to-run.
/// </summary>
[System.Serializable]
public class GameDeck
{
    [field:SerializeField] public List<DeckElement> CardList {get; private set;}

    [SerializeField] List<DeckElement> _reserveCards = new();
    [SerializeField] List<DeckElement> _loadOut = new();
    public IReadOnlyList<DeckElement> ReserveCards => _reserveCards;
    public IReadOnlyList<DeckElement> LoadOut => _loadOut;

    [SerializeField] int _maxLoadOutSize = 10;
    public int MaxLoadOutSize => _maxLoadOutSize;

    /// <summary>
    /// Total number of non-unique cards in the deck
    /// </summary>
    public int TotalCards {get 
    {
        int total = 0;
        foreach(DeckElement deckElement in CardList)
        {
            total += deckElement.cardCount;
        }
        return total;
    }}

    public int TotalUpgradableCards {get 
    {
        int total = 0;
        foreach(DeckElement deckElement in CardList)
        {
            if(deckElement.card.HasUpgrades)
            {
                total += deckElement.cardCount;
            }
        }
        return total;
    }}

    /// <summary>
    /// Total number of unique cards in the deck
    /// </summary>
    public int TotalUniqueCards {get => CardList.Count;}

    public List<DeckElement> GetUpgradableCards()
    {
        List<DeckElement> upgradableCards = new List<DeckElement>();
        foreach(DeckElement deckElement in CardList)
        {
            if(deckElement.card.HasUpgrades)
            {
                upgradableCards.Add(deckElement);
            }
        }
        return upgradableCards;
    }

    /// <summary>
    /// Adds a new card to the deck as a DeckElement
    /// </summary>
    /// <param name="card"></param>
    /// <param name="cardCount"></param>
    public void AddCard(CardSO card, int cardCount)
    {
        DeckElement deckElement = new DeckElement()
        {
            card = card,
            cardCount = cardCount
        };
        CardList.Add(deckElement);
        _reserveCards.Add(deckElement);
    }


    public GameDeck()
    {
        CardList = new List<DeckElement>();
    }

    public GameDeck(DeckSO deckSO)
    {
        CardList = new List<DeckElement>();
        foreach(DeckElement deckElement in deckSO.CardListReadOnly)
        {
            DeckElement newDeckElement = new DeckElement()
            {
                card = deckElement.card,
                cardCount = deckElement.cardCount
            };

            CardList.Add(newDeckElement);
        }

        if(_maxLoadOutSize > CardList.Count)
        {
            for(int i = 0; i < CardList.Count; i++)
            {
                _loadOut.Add(CardList[i]);
            }
        }else
        {
            for(int i = 0; i < _maxLoadOutSize; i++)
            {
                _loadOut.Add(CardList[i]);
            }

            if(CardList.Count > _maxLoadOutSize)
            {
                for(int i = _maxLoadOutSize; i < CardList.Count; i++)
                {
                    _reserveCards.Add(CardList[i]);
                }
            }
        }

    }

}