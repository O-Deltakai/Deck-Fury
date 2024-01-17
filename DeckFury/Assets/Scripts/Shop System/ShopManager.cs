using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private static ShopManager _instance;
    public static ShopManager Instance => _instance;

    [SerializeField] List<ShopPurchasable> _purchasables = new List<ShopPurchasable>();
    public IReadOnlyList<ShopPurchasable> Purchasables => _purchasables;

    void Awake()
    {
        _instance = this;
    }


    void InitializeShop()
    {

    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
