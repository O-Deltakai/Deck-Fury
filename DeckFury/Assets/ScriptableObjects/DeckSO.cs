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


}


/// <summary>
/// Deck that is used by the player in game and filled during run-time. Allows creation of temporary decks run-to-run.
/// </summary>
[System.Serializable]
public class GameDeck
{
    [field:SerializeField] public List<DeckElement> CardList {get; private set;}
}