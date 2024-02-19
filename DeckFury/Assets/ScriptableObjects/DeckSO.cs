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

    [SerializeField] List<UnlockCondition> _unlockConditions;
    public IReadOnlyList<UnlockCondition> UnlockConditions => _unlockConditions;


}


/// <summary>
/// Deck that is used by the player in game and filled during run-time. Allows creation of temporary decks run-to-run.
/// </summary>
[System.Serializable]
public class GameDeck
{
    [field:SerializeField] public List<DeckElement> CardList {get; private set;}

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