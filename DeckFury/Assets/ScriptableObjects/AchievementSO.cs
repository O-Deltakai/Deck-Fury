using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Achievement Data", menuName = "New Achievement Data", order = 0)]
public class AchievementSO : ScriptableObject
{
    [SerializeField] string _id;
    public string ID { get => _id; }

    [SerializeField] NotificationData _notification;
    public NotificationData Notification { get => _notification; }

    [SerializeField] List<StatUnlockCondition> unlockConditions = new ();

    public bool unlocked = false;
    public bool notificationShown = false;

    public bool Evaluate()
    {
        if(unlockConditions.Count == 0)
        {
            return true;
        }

        foreach (StatUnlockCondition condition in unlockConditions)
        {
            if (!condition.Evaluate())
            {
                return false;
            }
        }

        return true;
    }


}
