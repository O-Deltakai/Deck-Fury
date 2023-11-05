using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class RewardConditionEvent : UnityEvent<BonusScoreItemSO>
{
}


public enum ComparisonType
{
    GreaterThan,
    LessThan,
    EqualTo
}


[CreateAssetMenu(fileName = "Bonus Score Item", menuName = "New Bonus Score Item", order = 0)]
public class BonusScoreItemSO : ScriptableObject
{
    [field:SerializeField] public string RewardName{get; private set;}
    [field:SerializeField] public int BaseScore{get; private set;}
    [field:SerializeField] public CompareAgainstValue CompareValue{get; private set;}

[TextArea(5, 20)]
    [SerializeField] string rewardDescription;
    public string RewardDescription{ get { return rewardDescription; } }

    [RewardMethodSelector]
    [SerializeField] string ConditionMethodName;



[Header("Custom Comparison Settings")]
[Tooltip("You can use this section to compare two variables within the ScoreManager with a given comparison type" 
+ " and by setting the RewardCondition method to the CompareActions method in the RewardConditionChecks object.")]
    [ScoreManagerFieldSelector] public string firstActionFieldName;

    [ScoreManagerFieldSelector] public string secondActionFieldName;
    [field: SerializeField] public ComparisonType comparisonType { get; private set; }


    public string GetConditionMethodName()
    {
        return ConditionMethodName;
    }


}
