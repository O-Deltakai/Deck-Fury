using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyReward : RewardableElement
{
    public int rewardAmount;



    public override void SelectAsReward()
    {
        if(PersistentLevelController.Instance)
        {
            PersistentLevelController.Instance.PlayerData.CurrentMoney += rewardAmount;
        }

        base.SelectAsReward();
    }

}
