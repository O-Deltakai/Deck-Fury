using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public delegate bool RewardCondition(ScoreManager scoreManager, CompareAgainstValue compareValue);



[System.Serializable]
public struct CompareAgainstValue
{
    public enum ValueType { Int, Float, Double }
    [field:SerializeField] public bool UseCompareValue{get; private set;}
    [SerializeField] string ValueName;

[Tooltip("Which type of value the reward condition will compare against.")]
    [SerializeField] ValueType valueType;
    [SerializeField] int intValue;
    [SerializeField] float floatValue;
    [SerializeField] double doubleValue;
    [SerializeField] bool isGreaterThan;

    public bool Compare(int otherValue)
    {
        if (valueType != ValueType.Int)
        {
            Debug.LogWarning("Value type of compare value: " + ValueName + " does not match the type it is attempting " +
            "compare against within the ScoreManager. Double check that the Value Type is consistent with the type within the" +
            " method assigned in the RewardCondition.");
            return false;  
        } 

        return isGreaterThan ? otherValue > intValue : otherValue < intValue;
    }

    public bool Compare(float otherValue)
    {
        if (valueType != ValueType.Float)
        {
            Debug.LogWarning("Value type of compare value: " + ValueName + " does not match the type it is attempting " +
            "compare against within the ScoreManager. Double check that the Value Type is consistent with the type within the" +
            " method assigned in the RewardCondition.");
            return false;  
        } 
        return isGreaterThan ? otherValue > floatValue : otherValue < floatValue;
    }

    public bool Compare(double otherValue)
    {
        if (valueType != ValueType.Double)
        {
            Debug.LogWarning("Value type of compare value: " + ValueName + " does not match the type it is attempting " +
            "compare against within the ScoreManager. Double check that the Value Type is consistent with the type within the" +
            " method assigned in the RewardCondition.");
            return false;  
        } 
        return isGreaterThan ? otherValue > doubleValue : otherValue < doubleValue;
    }
    
}



[CreateAssetMenu(fileName = "Score Reward Table", menuName = "New Score Reward Table", order = 0)]
public class ScoreRewardTableSO : ScriptableObject
{

    [field:SerializeField] public List<BonusScoreItemSO> BonusRewardTable {get; private set;} = new List<BonusScoreItemSO>();

    public Dictionary<string, BonusScoreItemSO> GetRewardDictionary()
    {
        Dictionary<string, BonusScoreItemSO> rewardDictionary = new Dictionary<string, BonusScoreItemSO>();

        foreach(var rewardItem in BonusRewardTable)
        {
            rewardDictionary.Add(rewardItem.RewardName, rewardItem);
        }

        return rewardDictionary;
    }

}
