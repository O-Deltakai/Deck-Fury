using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RewardType
{
    Card,
    Item,
    Money
}


/// <summary>
/// Serves as the base class for any element that can be given as a reward to the player via the Additional Rewards menu.
/// </summary>
[Serializable]
public abstract class RewardableElement
{
    public event Action<RewardableElement> OnSelectReward;
    public RewardType rewardType;
    public Sprite rewardSprite;
    public string rewardName;

    public virtual void SelectAsReward()
    {
        OnSelectReward?.Invoke(this);
    }




}
