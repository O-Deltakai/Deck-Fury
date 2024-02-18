using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerStatsManager : MonoBehaviour
{
    public enum StatKey
    {
        TotalEnemiesKilled,
        TotalEnemiesKilledWithHazards,

        TotalDamageDealt,
        TotalDamageTaken,

        TotalReflectKills,
        TotalBoostKills,
        TotalComboKills,

        NumberOfCardsUsed,
        TotalBasicAttacks,
        TotalNumberOfMoves,
        
        HighestScore,
        HighestComboKill,
        MostMoneyEarnedOnOneRun,

        NumberOfDeaths,
        NumberOfCompletedRuns,
        NumberOfEliteStagesCompleted,
    }

    public static bool HasBeatenTheGame = false;

    public static GlobalPlayerStatsManager Instance {get; private set;}

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Sets the player pref stat to the value
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetPlayerPrefStat(StatKey key, int value)
    {
        PlayerPrefs.SetInt(key.ToString(), value);
    }

    /// <summary>
    /// Adds to the player pref stat if it exists, otherwise it sets it to the value
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void AddToPlayerPrefStat(StatKey key, int value)
    {
        int currentValue = GetPlayerPrefStat(key, out bool exists);
        if(exists)
        {
            SetPlayerPrefStat(key, currentValue + value);
        }else
        {
            SetPlayerPrefStat(key, value);
        }
    }


    public static int GetPlayerPrefStat(StatKey key, out bool exists)
    {
        if(PlayerPrefs.HasKey(key.ToString()))
        {
            exists = true;
            return PlayerPrefs.GetInt(key.ToString());
        }else
        {
            exists = false;
            return 0;
        }
    }

    public static void SetBeatenGameState(bool value)
    {
        HasBeatenTheGame = value;
        PlayerPrefs.SetInt("HasBeatenTheGame", value ? 1 : 0);
    }
    public static int GetBeatenGameState()
    {
        if (PlayerPrefs.HasKey("HasBeatenTheGame"))
        {
            return PlayerPrefs.GetInt("HasBeatenTheGame");
        }else
        {
            return 0;
        }
    }



}
