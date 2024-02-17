using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeckUnlockCondition
{
    [SerializeField] string _conditionName;
    public string ConditionName { get => _conditionName; }
    [SerializeField] GlobalPlayerStatsManager.StatKey statKey;
    [SerializeField] int compareValue;
    [SerializeField] bool isGreaterThan;

    public bool Evaluate()
    {
        int value = GlobalPlayerStatsManager.GetPlayerPrefStat(statKey, out bool exists);

        if (!exists)
        {
            return false;
        }

        return isGreaterThan ? value > compareValue : value < compareValue;
    }

}
