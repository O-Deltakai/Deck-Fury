using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Base Player Data", menuName = "New Base Player Data", order = 0)]
public class PlayerDataSO : ScriptableObject
{
    [SerializeField, Min(0)] int _maxHP = 200;
    public int MaxHP => _maxHP;

    [SerializeField, Min(0)] int _baseShields = 25;
    public int BaseShields => _baseShields;

    [SerializeField, Range(0, 80)] int _baseArmor = 0;
    public int BaseArmor => _baseArmor;

    [SerializeField, Range(0.1f, 10)] double _baseDefense = 1;
    public double BaseDefense => _baseDefense;

    [SerializeField, Min(0)] int _startingMoney = 100;
    public int StartingMoney => _startingMoney;

    [SerializeField] DeckSO _startingDeck;
    [SerializeField] List<ItemSO> _startingItems;




}
