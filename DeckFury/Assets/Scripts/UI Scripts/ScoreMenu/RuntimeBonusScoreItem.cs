using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuntimeBonusScoreItem
{
    public BonusScoreItemSO ConcreteBonusScoreItemReference { get; private set; }

    public string RewardName { get; private set; }
    public int BaseScore{ get; private set; }
    public CompareAgainstValue CompareValue{ get; private set; }

    public string rewardDescription{ get; private set; }

    [RewardMethodSelector]
    string ConditionMethodName;

    public RewardConditionEvent RewardCondition { get; private set; } = new RewardConditionEvent();

[Header("Custom Comparison Settings")]
[Tooltip("You can use this section to compare two variables within the ScoreManager with a given comparison type" 
+ " and by setting the RewardCondition method to the CompareActions method in the RewardConditionChecks object.")]
    [ScoreManagerFieldSelector] public string firstActionFieldName;

    [ScoreManagerFieldSelector] public string secondActionFieldName;
    [field: SerializeField] public ComparisonType comparisonType { get; private set; }


    public RuntimeBonusScoreItem(BonusScoreItemSO bonusScoreItem)
    {
        RewardName = bonusScoreItem.RewardName;
        BaseScore = bonusScoreItem.BaseScore;
        CompareValue = bonusScoreItem.CompareValue;
        rewardDescription = bonusScoreItem.RewardDescription;
        ConditionMethodName = bonusScoreItem.GetConditionMethodName();

        ConcreteBonusScoreItemReference = bonusScoreItem;        
    }


    public string GetConditionMethodName()
    {
        return ConditionMethodName;
    }
}
