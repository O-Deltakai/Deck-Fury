using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "New Item Data", order = 0)]
public class ItemSO : ScriptableObject
{
    [SerializeField] string _itemName;
    public string ItemName => _itemName;

    [SerializeField, TextArea(5, 10)] string _shortDescription;
    public string ShortDescription => _shortDescription;


    [SerializeField, TextArea(10, 20)] string _itemDescription;
    /// <summary>
    /// The unformatted string description of the item.
    /// </summary>
    public string ItemDescription => _itemDescription;

    [Tooltip("The base value of this item which defines how much it will cost when purchasing it from the shop.")]
    [SerializeField, Min(0)] int _value;
    public int Value => _value;

    [Tooltip("Base rarity of the item, which dictates how common the item will be in elite rewards or shops.")]
    [SerializeField, Range(1, 4)] int _rarity = 1;
    public int Rarity => _rarity;

    [SerializeField] List<QuantifiableEffect> _quantifiableEffects;
    public IReadOnlyList<QuantifiableEffect> QuantifiableEffects => _quantifiableEffects;


    [SerializeField] Sprite _itemImage;
    public Sprite ItemImage => _itemImage;

    [SerializeField] GameObject _itemPrefab;
    public GameObject ItemPrefab => _itemPrefab;

    [Header("Advanced Properties")]
    [SerializeField] bool _hasEffectOutsideBattle;
    public bool HasEffectOutsideBattle => _hasEffectOutsideBattle;

    [SerializeField] bool _oneTimeEffect;
    public bool OneTimeEffect => _oneTimeEffect;

    public string GetFormattedDescription()
    {
        string formattedDescription = ItemDescription;

        for(int i = 0; i < QuantifiableEffects.Count ; i++) 
        {
            string qEffect = "Q" + i;
            formattedDescription = formattedDescription.Replace(qEffect, QuantifiableEffects[i].GetValueDynamic().ToString());
        }

        return formattedDescription;
    }    

    public string GetFormattedShortDescription()
    {
        string formattedDescription = ShortDescription;

        for(int i = 0; i < QuantifiableEffects.Count ; i++) 
        {
            string qEffect = "Q" + i;
            formattedDescription = formattedDescription.Replace(qEffect, QuantifiableEffects[i].GetValueDynamic().ToString());
        }

        return formattedDescription;
    } 


}
