using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private static ShopManager _instance;
    public static ShopManager Instance => _instance;

    [SerializeField] GameObject purchasablesParentObject;

    [SerializeField] List<ShopPurchasable> _purchasables = new List<ShopPurchasable>();
    public IReadOnlyList<ShopPurchasable> Purchasables => _purchasables;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        InitializeShop();
    }

    void InitializeShop()
    {
        foreach(ShopPurchasable purchasable in purchasablesParentObject.GetComponentsInChildren<ShopPurchasable>())
        {
            _purchasables.Add(purchasable);
            purchasable.shopManager = this;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void PurchaseCard(WorldShopCard shopCard)
    {

    }

    public void PurchaseItem(WorldShopItem shopItem)
    {

    }



}
