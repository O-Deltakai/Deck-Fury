using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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

    [SerializeField] float _cardPriceMaxMultiplier = 1.2f;
    [SerializeField] float _cardPriceMinMultiplier = 1.1f;
    [SerializeField] float _cardPriceSaleMinMultiplier = 0.5f;
    [SerializeField] float _cardPriceSaleMaxMultiplier = 0.7f;


    [SerializeField, Range(0, 1)] double saleChance = 0.1; 

[Header("Shop UI Elements")]
    [SerializeField] TextMeshProUGUI playerCurrentMoneyText;
    [SerializeField] Color insufficientFundMoneyColor;
    Color originalMoneyTextColor;
    Tween shakeMoneyTween;
    [SerializeField] float shakeMoneyTextDuration;
    [SerializeField] float shakeMoneyTextStrength;
    [SerializeField] int shakeMoneyTextVibrato;


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
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void InitializeShop()
    {
        //Placeholder for now - just gets all the cards available from the Resources folder
        CardSO[] cardPool = GetCardsFromResources("Cards");


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

        foreach(WorldShopCard shopCard in _purchasables.Cast<WorldShopCard>())
        {
            //Randomize card
            
            shopCard.Card = cardPool[random.Next(0, cardPool.Count() - 1)];

            double randomDouble = random.NextDouble();
            

            //Determine if on sale
            if(randomDouble <= saleChance)
            {
                shopCard.onSale = true;
                shopCard.Price = (int)(shopCard.Card.GetCardTier() * _cardPriceBase * RandomFloat(_cardPriceSaleMinMultiplier, _cardPriceSaleMaxMultiplier));
            }else
            {
                shopCard.Price = (int)(shopCard.Card.GetCardTier() * _cardPriceBase * RandomFloat(_cardPriceMinMultiplier, _cardPriceMaxMultiplier));
            }
            
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
                return;
            }


            DeckElement deckElement = StageStateController.Instance.PlayerData.AddCardToDeck(shopCard.Card, 1);
            CardPoolManager.Instance.AddDeckElementToPool(deckElement);
            stageStateController.PlayerData.CurrentMoney -= shopCard.Price;
            Destroy(shopCard.gameObject);
        }
        else
        {
            Debug.LogError("Nowhere to send bought card to.", this);
        }




    }

    public void PurchaseItem(WorldShopItem shopItem)
    {

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
