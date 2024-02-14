using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Pool Data", menuName = "New Card Pool", order = 0)]
public class CardPoolSO : ScriptableObject
{
    [SerializeField] List<CardSO> _cardPool;
    public IReadOnlyList<CardSO> CardPool { get { return _cardPool; } }

    [SerializeField] List<CardSO> _tier1Cards;
    public IReadOnlyList<CardSO> Tier1Cards { get { return _tier1Cards; } }

    [SerializeField] List<CardSO> _tier2Cards;
    public IReadOnlyList<CardSO> Tier2Cards { get { return _tier2Cards; } }

    [SerializeField] List<CardSO> _tier3Cards;
    public IReadOnlyList<CardSO> Tier3Cards { get { return _tier3Cards; } }


    /// <summary>
    /// Returns a random card from the pool. Can be given a System.Random for seeded randomness.
    /// </summary>
    /// <param name="random"></param>
    /// <returns></returns>
    public CardSO GetRandomCard(System.Random random = null)
    {
        if(random != null)
        {
            return _cardPool[random.Next(0, _cardPool.Count)];
        }

        return _cardPool[Random.Range(0, _cardPool.Count)];
    }

    /// <summary>
    /// Returns a random card from the specified tier. Can be given a System.Random for seeded randomness.
    /// </summary>
    /// <param name="tier"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentException"></exception>
    public CardSO GetRandomCard(int tier, System.Random random = null)
    {
        if(tier < 1 || tier > 3)
        {
            throw new System.ArgumentException("Tier must be between 1 and 3");
        }

        if(random != null)
        {
            return tier switch
            {
                1 => _tier1Cards[random.Next(0, _tier1Cards.Count)],
                2 => _tier2Cards[random.Next(0, _tier2Cards.Count)],
                3 => _tier3Cards[random.Next(0, _tier3Cards.Count)],
                _ => null,
            };
        }
        else
        {
            return tier switch
            {
                1 => _tier1Cards[Random.Range(0, _tier1Cards.Count)],
                2 => _tier2Cards[Random.Range(0, _tier2Cards.Count)],
                3 => _tier3Cards[Random.Range(0, _tier3Cards.Count)],
                _ => null,
            };
        }
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

    public void SortCardsByTier()
    {
        _tier1Cards.Clear();
        _tier2Cards.Clear();
        _tier3Cards.Clear();

        foreach (var card in _cardPool)
        {
            switch (card.GetCardTier())
            {
                case 1:
                    _tier1Cards.Add(card);
                    break;
                case 2:
                    _tier2Cards.Add(card);
                    break;
                case 3:
                    _tier3Cards.Add(card);
                    break;
                default:
                    break;
            }
        }
    }


}
