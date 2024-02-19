using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    private Dictionary<string, AchievementSO> achievements = new Dictionary<string, AchievementSO>();

    public void UnlockAchievement(AchievementSO achievement)
    {
        if (achievements.ContainsValue(achievement) && !achievements[achievement.ID].unlocked)
        {
            achievements[achievement.ID].unlocked = true;
            SaveAchievement(achievement.ID);

            // Check if the notification has been shown before
            if (!achievements[achievement.ID].notificationShown)
            {
                //ShowNotification(achievements[id].description);
                achievements[achievement.ID].notificationShown = true;
                SaveAchievement(achievement.ID); // Save again to update the notificationShown flag
            }
        }
    }


    private void SaveAchievement(string id)
    {
        AchievementSO achievement = achievements[id];
        PlayerPrefs.SetInt($"{id}_unlocked", achievement.unlocked ? 1 : 0);
        PlayerPrefs.SetInt($"{id}_notificationShown", achievement.notificationShown ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the achievements from the player prefs
    /// </summary>
    private void LoadAchievements()
    {
        foreach (var achievement in achievements.Values)
        {
            achievement.unlocked = PlayerPrefs.GetInt($"{achievement.ID}_unlocked", 0) == 1;
            achievement.notificationShown = PlayerPrefs.GetInt($"{achievement.ID}_notificationShown", 0) == 1;
        }
    }


}
