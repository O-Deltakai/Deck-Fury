using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckElement
{
    public CardSO card;
    public int cardCount;
}

[CreateAssetMenu(fileName = "Deck Data", menuName = "New Deck", order = 0)]
public class DeckSO : ScriptableObject
{
    public string DeckName;
    [field:SerializeField] public List<DeckElement> CardList {get; private set;}

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

    /// <summary>
    /// Total number of unique cards in the deck
    /// </summary>
    public int TotalUniqueCards {get => CardList.Count;}

    public GameDeck()
    {
        CardList = new List<DeckElement>();
    }

    public GameDeck(DeckSO deckSO)
    {
        CardList = new List<DeckElement>();
        foreach(DeckElement deckElement in deckSO.CardList)
        {
            CardList.Add(deckElement);
        }
    }

}