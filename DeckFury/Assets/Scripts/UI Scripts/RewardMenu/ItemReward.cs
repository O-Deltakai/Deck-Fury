using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReward : RewardableElement
{

    public ItemSO rewardItemSO;



    public override void SelectAsReward()
    {
        if(PersistentItemController.Instance)
        {
            ItemBase addedItem = PersistentItemController.Instance.AddItemToPlayerData(rewardItemSO);
            PlayerItemController playerItemController = GameManager.Instance.player.GetComponent<PlayerItemController>();
            playerItemController.GiveItemInstanceToPlayer(addedItem);            
        }else
        {
            Debug.LogError("No PersistenItemController instance found, could not grant reward.");
        }
        base.SelectAsReward();
    }

}
