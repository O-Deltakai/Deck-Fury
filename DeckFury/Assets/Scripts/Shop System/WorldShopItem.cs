using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldShopItem : ShopPurchasable
{

    [SerializeField] ItemSO item;


    public override void Purchase()
    {
        shopManager.PurchaseItem(this);
    }


}