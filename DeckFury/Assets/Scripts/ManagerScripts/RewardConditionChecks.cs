using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;


public class RewardConditionChecks : MonoBehaviour
{
    private static readonly Type[] AllowedTypes = { typeof(int), typeof(float), typeof(double) }; // Add more types as needed


    private static RewardConditionChecks _instance;
    public static RewardConditionChecks Instance {get{ return _instance; }}

    public delegate void ConditionMetEventHandler(BonusScoreItemSO bonusScoreItem);
    public event ConditionMetEventHandler OnBonusConditionMet;


    ScoreManager scoreManager;

    private void Awake() 
    {
        scoreManager = GetComponent<ScoreManager>();
        if(!scoreManager)
        {
            scoreManager = GameErrorHandler.NullCheck( FindObjectOfType<ScoreManager>(), "ScoreManager");
        }    
    }

    private object GetFieldValueFromScoreManager(string fieldNameWithType)
    {
        string fieldName = fieldNameWithType.Split(' ')[0]; // Extract the field name from the string
        FieldInfo field = typeof(ScoreManager).GetField(fieldName);
        if (field != null && AllowedTypes.Contains(field.FieldType))
        {
            return field.GetValue(scoreManager);
        }
        return null;
    }


    public void CompareActions(BonusScoreItemSO bonusScoreItem)
    {
        object firstValue = GetFieldValueFromScoreManager(bonusScoreItem.firstActionFieldName);
        object secondValue = GetFieldValueFromScoreManager(bonusScoreItem.secondActionFieldName);

        if (firstValue is IComparable firstComparable && secondValue is IComparable secondComparable)
        {
            int comparisonResult = firstComparable.CompareTo(secondComparable);

            bool conditionMet = false;
            switch (bonusScoreItem.comparisonType)
            {
                case ComparisonType.GreaterThan:
                    conditionMet = comparisonResult > 0;
                    break;
                case ComparisonType.LessThan:
                    conditionMet = comparisonResult < 0;
                    break;
                case ComparisonType.EqualTo:
                    conditionMet = comparisonResult == 0;
                    break;
            }

            if (conditionMet)
            {
                // Raise the event or handle the condition being met
                OnBonusConditionMet?.Invoke(bonusScoreItem);
            }
        }
    }


    public void NoDamageTaken(BonusScoreItemSO bonusScoreItem)
    {
        int damageTaken = scoreManager.TotalDamageTaken;
        if(damageTaken == 0)
        {
            OnBonusConditionMet?.Invoke(bonusScoreItem);
        }
    }


    public void CompareNumberOfMoves(BonusScoreItemSO bonusScoreItem)
    {
        int numberOfMoves = scoreManager.NumberOfMoves;
        if(bonusScoreItem.CompareValue.Compare(numberOfMoves))
        {
            OnBonusConditionMet?.Invoke(bonusScoreItem);
        }
        
    }

    public void CompareCardsUsed(BonusScoreItemSO bonusScoreItem)
    {
        int value = scoreManager.NumberOfCardsUsed;

        if(bonusScoreItem.CompareValue.Compare(value))
        {
            OnBonusConditionMet?.Invoke(bonusScoreItem);
        }
    }

    public void CompareBasicAttacks(BonusScoreItemSO bonusScoreItem)
    {
        int value = scoreManager.NumberOfBasicAttacks;

        if(bonusScoreItem.CompareValue.Compare(value))
        {
            OnBonusConditionMet?.Invoke(bonusScoreItem);
        }
    }

    public void CompareEnemiesKilledWithHazards(BonusScoreItemSO bonusScoreItem)
    {
        if(!bonusScoreItem)
        {
            Debug.LogWarning("Bonus score item was null, aborted checking reward.");
            return;
        }
        if(!scoreManager)
        {
            Debug.LogWarning("ScoreManager is null, cannot check bonus reward condition");
            return;
        }
        int value = scoreManager.EnemiesKilledWithHazards;

        if(bonusScoreItem.CompareValue.Compare(value))
        {
            OnBonusConditionMet?.Invoke(bonusScoreItem);
        }
    }    

    public void CompareTimeSpentOnStage(BonusScoreItemSO bonusScoreItem)
    {
        float value = scoreManager.TimeSpentOnStage;

        if(bonusScoreItem.CompareValue.Compare(value))
        {
            OnBonusConditionMet?.Invoke(bonusScoreItem);
        }
    }

    public void CompareDamageDealtToEnemies(BonusScoreItemSO bonusScoreItem)
    {
        int value = scoreManager.DamageDealtToEnemies;

        if(bonusScoreItem.CompareValue.Compare(value))
        {
            OnBonusConditionMet?.Invoke(bonusScoreItem);
        }
    }    

    public void CompareHighestComboKill(BonusScoreItemSO bonusScoreItem)
    {
        int value = scoreManager.HighestComboKill;

        if(bonusScoreItem.CompareValue.Compare(value))
        {
            OnBonusConditionMet?.Invoke(bonusScoreItem);
        }
    }   

    public void CompareNumberOfComboKills(BonusScoreItemSO bonusScoreItem)
    {
        int value = scoreManager.NumberOfComboKills;

        if(bonusScoreItem.CompareValue.Compare(value))
        {
            OnBonusConditionMet?.Invoke(bonusScoreItem);
        }
    } 


}
