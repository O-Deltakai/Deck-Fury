using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Manages the achievements in the game. Is assumed to be placed on the GameManager object.
/// </summary>
public class AchievementManager : MonoBehaviour
{
    static AchievementManager _instance;
    public static AchievementManager Instance => _instance;

    static Dictionary<string, AchievementSO> _achievements = new();
    public static IReadOnlyDictionary<string, AchievementSO> Achievements { get => _achievements; }

    const int BatchSize = 10;


    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;

            RetrieveAchievementsFromResources();
            LoadAchievements();
        
        }
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

/// <summary>
/// Returns a batch of achievements from the dictionary
/// </summary>
/// <param name="startIndex"></param>
/// <param name="count"></param>
/// <returns></returns>
    private static List<AchievementSO> GetAchievementBatch(int startIndex, int count)
    {
        List<AchievementSO> batch = new List<AchievementSO>();
        int endIndex = startIndex + count;

        int currentIndex = 0;
        foreach (var kvp in _achievements)
        {
            if (currentIndex >= startIndex && currentIndex < endIndex)
            {
                batch.Add(kvp.Value);
            }

            currentIndex++;

            if (currentIndex >= endIndex)
            {
                break;
            }
        }

        return batch;
    }

    public static async void CheckAchievementsAsync()
    {
        print("Checking achievements async");
        if(_achievements.Count == 0)
        {
            print("No achievements found, returning");
            return;
        }

        try
        {
            print("Trying to check achievements async");
            for (int i = 0; i < _achievements.Count; i += BatchSize)
            {
                // Take a batch of achievements to check
                print("Checking batch " + i + " to " + Mathf.Min(BatchSize, _achievements.Count - i));
                var batch = GetAchievementBatch(i, Mathf.Min(BatchSize, _achievements.Count - i));

                // Run the batch check asynchronously to avoid blocking the main thread
                print("Running async batch check");
                #if !UNITY_WEBGL // If not compiling for WebGL, use Task.Run
                    await Task.Run(() => 
                    {
                        async() => await CheckAchievementBatch(batch)
                    });
                #else // For WebGL, just await the method without Task.Run
                    await CheckAchievementBatch(batch);
                #endif

                //await a small delay to spread out the computation
                await Task.Delay(10); // Wait for 10 milliseconds (adjust based on performance)
            }
        }
        catch (Exception ex)
        {
            print($"Exception occurred: {ex.Message}");
        }


    }

    private static async Task CheckAchievementBatch(List<AchievementSO> batch)
    {
        if(batch.Count == 0)
        {
            Debug.LogWarning("Achievement batch is empty, something might have gone wrong.");
            return;
        }
        foreach (var achievement in batch)
        {
            // Perform the check
            print("Checking achievement " + achievement.ID);
            await UnityMainThreadDispatcher.Instance.EnqueueTask(() => EvaluateAchievementConditions(achievement));

        }
    }


    void RetrieveAchievementsFromResources()
    {
        print("Retrieving achievements from resources");
        AchievementSO[] achievementArray = Resources.LoadAll<AchievementSO>("Achievements");
        print("Number of achievements found: " + achievementArray.Length);
        foreach (var achievement in achievementArray)
        {
            _achievements.Add(achievement.ID, achievement);
        }
    }

    static bool EvaluateAchievementConditions(AchievementSO achievement)
    {
        if (achievement.Evaluate())
        {
            UnlockAchievement(achievement);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Loads the achievements from the player prefs
    /// </summary>
    void LoadAchievements()
    {
        print("Loading achievements");
        foreach (var achievement in _achievements.Values)
        {
            achievement.unlocked = PlayerPrefs.GetInt($"{achievement.ID}_unlocked", 0) == 1;
            achievement.notificationShown = PlayerPrefs.GetInt($"{achievement.ID}_notificationShown", 0) == 1;
        }
    }


}
