using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GenericRewardSlot : MonoBehaviour
{
    public event Action<RewardableElement> OnSelectReward;


    [SerializeField] RewardableElement _rewardElement;
    public RewardableElement RewardElement {get { return _rewardElement; }
        set
        {
            _rewardElement = value;
        }
    }


    [SerializeField] TextMeshProUGUI _rewardNameText;
    [SerializeField] Image _rewardFrameImage;
    [SerializeField] Image _rewardObjectImage;

    public void Initialize()
    {
        if(_rewardElement == null)
        {
            Debug.LogError("Reward element has not been set for this reward slot.", this);
            return;
        }

        _rewardNameText.text = _rewardElement.rewardName;
        _rewardObjectImage.sprite = _rewardElement.rewardSprite;


    }


    public void SelectReward()
    {
        if(_rewardElement == null)
        {
            Debug.LogError("Reward element has not been set for this reward slot.", this);
            return;
        }        

        _rewardElement.SelectAsReward();
        OnSelectReward?.Invoke(_rewardElement);
        Destroy(gameObject);
    }

}
