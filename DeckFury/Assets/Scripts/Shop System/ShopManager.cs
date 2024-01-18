using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private static ShopManager _instance;
    public static ShopManager Instance => _instance;

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


    void Awake()
    {
        _instance = this;
        stageStateController = StageStateController.Instance;
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
        foreach(ShopPurchasable purchasable in purchasablesParentObject.GetComponentsInChildren<ShopPurchasable>())
        {
            _purchasables.Add(purchasable);
            purchasable.shopManager = this;
        }

        foreach(WorldShopCard shopCard in _purchasables.Cast<WorldShopCard>())
        {
            //Randomize card

            shopCard.Price = (int)(shopCard.Card.GetCardTier() * _cardPriceBase * Random.Range(_cardPriceMinMultiplier, _cardPriceMaxMultiplier));
        }

    }

    void GenerateCardPool()
    {

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
        }
        else
        {
            Debug.LogError("Nowhere to send bought card to.", this);
        }



        Destroy(shopCard.gameObject);

    }

    public void PurchaseItem(WorldShopItem shopItem)
    {

    }

    void NotifyInsufficientFunds()
    {
        print("Player has insufficient funds to purchase object.");
    }

}
