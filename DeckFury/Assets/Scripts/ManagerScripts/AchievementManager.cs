using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    static AchievementManager _instance;
    public static AchievementManager Instance => _instance;

    static Dictionary<string, AchievementSO> _achievements = new Dictionary<string, AchievementSO>();
    public IReadOnlyDictionary<string, AchievementSO> Achievements { get => _achievements; }


    void Awake()
    {
        _instance = this;

        RetrieveAchievementsFromResources();
        LoadAchievements();
    }

    void OnDestroy()
    {
        _instance = null;
    }


    public static void UnlockAchievement(AchievementSO achievement)
    {
        if (_achievements.ContainsValue(achievement) && !_achievements[achievement.ID].unlocked)
        {
            _achievements[achievement.ID].unlocked = true;
            SaveAchievement(achievement.ID);

            // Check if the notification has been shown before
            if (!_achievements[achievement.ID].notificationShown)
            {
                EventBus<NotificationEvent>.Raise(new NotificationEvent(achievement.Notification));
                _achievements[achievement.ID].notificationShown = true;
                SaveAchievement(achievement.ID); // Save again to update the notificationShown flag
            }
        }
    }


    static void SaveAchievement(string id)
    {
        AchievementSO achievement = _achievements[id];
        PlayerPrefs.SetInt($"{id}_unlocked", achievement.unlocked ? 1 : 0);
        PlayerPrefs.SetInt($"{id}_notificationShown", achievement.notificationShown ? 1 : 0);
        PlayerPrefs.Save();
    }

    void RetrieveAchievementsFromResources()
    {
        AchievementSO[] achievementArray = Resources.LoadAll<AchievementSO>("Achievements");
        foreach (var achievement in achievementArray)
        {
            _achievements.Add(achievement.ID, achievement);
        }
    }

    void EvaluateAchievementConditions(AchievementSO achievement)
    {
        if (achievement.Evaluate())
        {
            UnlockAchievement(achievement);
        }
    }

    /// <summary>
    /// Loads the achievements from the player prefs
    /// </summary>
    void LoadAchievements()
    {
        foreach (var achievement in _achievements.Values)
        {
            achievement.unlocked = PlayerPrefs.GetInt($"{achievement.ID}_unlocked", 0) == 1;
            achievement.notificationShown = PlayerPrefs.GetInt($"{achievement.ID}_notificationShown", 0) == 1;
        }
    }


}
