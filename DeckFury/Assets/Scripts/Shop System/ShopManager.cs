using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FMODUnity;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private static ShopManager _instance;
    public static ShopManager Instance => _instance;

    public System.Random random;

    StageStateController stageStateController;

    [SerializeField] GameObject purchasablesParentObject;

    [SerializeField] List<ShopPurchasable> _purchasables = new List<ShopPurchasable>();
    public IReadOnlyList<ShopPurchasable> Purchasables => _purchasables;

    [SerializeField] int _cardPriceBase = 50; // Multiplied by card tier to get actual price
    public int CardPriceBase {get { return _cardPriceBase; }
        set
        {
            _cardPriceBase = value;
        }
    }

    [SerializeField] int _itemBasePrice = 100; // Multiplied by item rarity to get actual price

[Header("Card Price Multipliers")]
    [SerializeField] float _cardPriceMaxMultiplier = 1.2f;
    [SerializeField] float _cardPriceMinMultiplier = 1.1f;
    [SerializeField] float _cardPriceSaleMinMultiplier = 0.5f;
    [SerializeField] float _cardPriceSaleMaxMultiplier = 0.7f;

[Header("Item Price Multipliers")]
    [SerializeField] float _itemPriceMinMultiplier = 1.5f;
    [SerializeField] float _itemPriceMaxMultiplier = 1.8f;
    [SerializeField] float _itemPriceSaleMinMultiplier = 1.1f;
    [SerializeField] float _itemPriceSaleMaxMultiplier = 1.4f;

    [SerializeField, Range(0, 1)] double itemSaleChance = 0.05;
    [SerializeField, Range(0, 1)] double cardSaleChance = 0.15; 

[Header("Shop UI Elements")]
    [SerializeField] TextMeshProUGUI playerCurrentMoneyText;
    [SerializeField] Color insufficientFundMoneyColor;
    Color originalMoneyTextColor;
    Tween shakeMoneyTween;
    [SerializeField] float shakeMoneyTextDuration;
    [SerializeField] float shakeMoneyTextStrength;
    [SerializeField] int shakeMoneyTextVibrato;


    [Header("SFX")]
    [SerializeField] EventReference buyCardSFX;
    [SerializeField] EventReference buyItemSFX;
    [SerializeField] EventReference denyPurchaseSFX;

    void Awake()
    {
        _instance = this;
        stageStateController = StageStateController.Instance;
        stageStateController.PlayerData.OnPlayerDataModified += SetMoneyText;


        originalMoneyTextColor = playerCurrentMoneyText.color;
    }

    void Start()
    {
        InitializeShop();
        SetMoneyText();

    }

    void OnDestroy()
    {
        _instance = null;
    }

    void InitializeShop()
    {
        //Placeholder for now - just gets all the cards available from the Resources folder
        CardSO[] cardPool = GetCardsFromResources("Cards");
        //Placeholder for now - just gets all the items available from the Resources folder
        ItemSO[] itemPool = GetItemsFromResources("Items");


        if(PersistentLevelController.Instance)
        {
            random = PersistentLevelController.Instance.runRandomGenerator;
        }else
        {
            random = new System.Random();
        }

        foreach(ShopPurchasable purchasable in purchasablesParentObject.GetComponentsInChildren<ShopPurchasable>())
        {
            _purchasables.Add(purchasable);
            purchasable.shopManager = this;
        }

        //Initialize purchasable cards
        foreach(WorldShopCard shopCard in _purchasables.OfType<WorldShopCard>())
        {
            //Randomize card
            shopCard.Card = cardPool[random.Next(0, cardPool.Count())];

            double randomDouble = random.NextDouble();
            

            //Determine if on sale
            if(randomDouble <= cardSaleChance)
            {
                shopCard.onSale = true;
                shopCard.Price = (int)(shopCard.Card.GetCardTier() * _cardPriceBase * RandomFloat(_cardPriceSaleMinMultiplier, _cardPriceSaleMaxMultiplier));
            }else
            {
                shopCard.Price = (int)(shopCard.Card.GetCardTier() * _cardPriceBase * RandomFloat(_cardPriceMinMultiplier, _cardPriceMaxMultiplier));
            }
            
        }

        List<ItemSO> selectedItems = new();
        int shopItemCount = _purchasables.OfType<WorldShopItem>().Count();

        for(int i = 0; i < shopItemCount; i++)
        {
            ItemSO nextItem = itemPool[random.Next(0, itemPool.Count())];

            while(selectedItems.Contains(nextItem))
            {
                nextItem = itemPool[random.Next(0, itemPool.Count())];
            }

            selectedItems.Add(nextItem);
        }



        int index = 0;
        //Initialize purchasable items
        foreach(WorldShopItem shopItem in _purchasables.OfType<WorldShopItem>())
        {
            ItemSO pickedItem = selectedItems[index];

            shopItem.ItemSO = pickedItem;

            double randomDouble = random.NextDouble();
            

            //Determine if on sale
            if(randomDouble <= itemSaleChance)
            {
                shopItem.onSale = true;
                shopItem.Price = (int)(shopItem.ItemSO.Rarity * _itemBasePrice * RandomFloat(_itemPriceSaleMinMultiplier, _itemPriceSaleMaxMultiplier) * shopItem.ItemSO.ValueMultiplier);
            }else
            {
                shopItem.Price = (int)(shopItem.ItemSO.Rarity * _itemBasePrice * RandomFloat(_itemPriceMinMultiplier, _itemPriceMaxMultiplier) * shopItem.ItemSO.ValueMultiplier);
            }
            index++;
        }


    }

    public float RandomFloat(float min, float max)
    {
        if (min >= max)
        {
            throw new ArgumentException("min should be less than max");
        }

        double range = max - min;
        double sample = random.NextDouble();
        double scaled = (sample * range) + min;
        return (float)scaled;
    }


    void GenerateCardPool()
    {
        CardSO[] cards = GetCardsFromResources("Cards");
    }

    CardSO[] GetCardsFromResources(string path)
    {
        CardSO[] cardResources = Resources.LoadAll<CardSO>(path);
        return cardResources;
    }

    ItemSO[] GetItemsFromResources(string path)
    {
        ItemSO[] itemResources = Resources.LoadAll<ItemSO>(path);
        return itemResources;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void PurchaseCard(WorldShopCard shopCard)
    {

        if(StageStateController.Instance && CardPoolManager.Instance)
        {
            if(shopCard.Price > stageStateController.PlayerData.CurrentMoney)
            {
                NotifyInsufficientFunds();
                RuntimeManager.PlayOneShot(denyPurchaseSFX);
                return;
            }


            DeckElement deckElement = StageStateController.Instance.PlayerData.AddCardToDeck(shopCard.Card, 1);
            CardPoolManager.Instance.AddDeckElementToPool(deckElement);
            stageStateController.PlayerData.CurrentMoney -= shopCard.Price;
            Destroy(shopCard.gameObject);

            RuntimeManager.PlayOneShot(buyCardSFX);

        }
        else
        {
            Debug.LogError("Nowhere to send bought card to.", this);
        }

    }

    public void PurchaseItem(WorldShopItem shopItem)
    {
        if(stageStateController)
        {
            if(shopItem.Price > stageStateController.PlayerData.CurrentMoney)
            {
                NotifyInsufficientFunds();
                RuntimeManager.PlayOneShot(denyPurchaseSFX);
                return;
            }
        }

        if(PersistentItemController.Instance)
        {
            ItemBase addedItem = PersistentItemController.Instance.AddItemToPlayerData(shopItem.ItemSO);
            PlayerItemController playerItemController = GameManager.Instance.player.GetComponent<PlayerItemController>();
            playerItemController.GiveItemInstanceToPlayer(addedItem);

            stageStateController.PlayerData.CurrentMoney -= shopItem.Price;
            Destroy(shopItem.gameObject);

            RuntimeManager.PlayOneShot(buyCardSFX);

        }else
        if(GameManager.Instance.player)
        {
            PlayerItemController playerItemController = GameManager.Instance.player.GetComponent<PlayerItemController>();
            playerItemController.InstantiateAndGiveItemToPlayer(shopItem.ItemSO);

            stageStateController.PlayerData.CurrentMoney -= shopItem.Price;
            Destroy(shopItem.gameObject);

            RuntimeManager.PlayOneShot(buyCardSFX);

        }
        else
        {
            Debug.LogError("Nowhere to send bought card to.", this);
        }
    }

    void SetMoneyText()
    {
        playerCurrentMoneyText.text = "$:" + stageStateController.PlayerData.CurrentMoney.ToString();
    }

    async void NotifyInsufficientFunds()
    {
        if(!shakeMoneyTween.IsActive())
        {
            shakeMoneyTween = playerCurrentMoneyText.transform.DOShakePosition(shakeMoneyTextDuration, shakeMoneyTextStrength, shakeMoneyTextVibrato);

            await playerCurrentMoneyText.DOColor(insufficientFundMoneyColor, shakeMoneyTextDuration*0.2f).AsyncWaitForCompletion();
            
            playerCurrentMoneyText.DOColor(originalMoneyTextColor, shakeMoneyTextDuration*0.8f);
        }
        print("Player has insufficient funds to purchase object.");
    }

}
