using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldShopCard : ShopPurchasable
{
    [SerializeField] CardSO card;



    public override void Purchase()
    {
        shopManager.PurchaseCard(this);
    }

}