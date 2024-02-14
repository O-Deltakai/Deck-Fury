using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Pool Data", menuName = "New Card Pool", order = 0)]
public class CardPoolSO : ScriptableObject
{
    [SerializeField] List<CardSO> _cardPool;
    public IReadOnlyList<CardSO> CardPool { get { return _cardPool; } }
    public CardSO GetRandomCard()
    {
        return _cardPool[Random.Range(0, _cardPool.Count)];
    }
    public CardSO GetCard(int index)
    {
        return _cardPool[index];
    }
    public int GetCardCount()
    {
        return _cardPool.Count;
    }
    public void AddCard(CardSO card)
    {
        _cardPool.Add(card);
    }
    public void RemoveCard(CardSO card)
    {
        _cardPool.Remove(card);
    }
    public void RemoveCard(int index)
    {
        _cardPool.RemoveAt(index);
    }
    public void ClearPool()
    {
        _cardPool.Clear();
    }
    public void ShufflePool()
    {
        for (int i = 0; i < _cardPool.Count; i++)
        {
            CardSO temp = _cardPool[i];
            int randomIndex = Random.Range(i, _cardPool.Count);
            _cardPool[i] = _cardPool[randomIndex];
            _cardPool[randomIndex] = temp;
        }
    }

}
