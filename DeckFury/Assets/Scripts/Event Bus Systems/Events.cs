using System;
using UnityEngine;

public interface IEvent { }

public struct ItemEvent : IEvent
{
    public ItemBase itemBase;
    public ItemSO itemSO;
    public object targetObject;
}

public struct NPCKilledEvent : IEvent
{
    public NPC npc;
    public AttackPayload killingBlow;
    public Vector3 positionAtDeath;
}

public struct NPCDamagedEvent : IEvent
{
    public AttackPayload? attackPayload;
    public NPC npc;
    public int damageTaken;
}

public struct UseCardEvent : IEvent
{
    public CardObjectReference card;
    public CardSO cardSO;
}

/// <summary>
/// Event used for when you want to relay a game object reference to another script
/// </summary>
public struct RelayGameObjectEvent : IEvent
{
    public GameObject gameObject;
}

public struct PlayerDamagedEvent : IEvent
{
    public AttackPayload attackPayload;
}

public struct ClickDeckElementEvent : IEvent
{
    public DeckCardElementSlot deckCardElementSlot;
}

public struct ModifiedPlayerPrefEvent : IEvent
{
    public string key;
    public object value;

    public ModifiedPlayerPrefEvent(string key, object value)
    {
        this.key = key;
        this.value = value;
    }

}

public struct NotificationEvent : IEvent
{
    public NotificationData notification;

    public NotificationEvent(NotificationData notification)
    {
        this.notification = notification;
    }

}

public struct AchievementUnlockedEvent : IEvent
{
    public AchievementSO achievement;

    public AchievementUnlockedEvent(AchievementSO achievement)
    {
        this.achievement = achievement;
    }

}

/// <summary>
/// Event used for when a scene begins to load
/// </summary>
public struct SceneBeginChangeEvent : IEvent
{
    public string sceneName;
}

/// <summary>
/// Event used for when a scene finishes loading
/// </summary>
public struct SceneFinishChangeEvent : IEvent
{
    public string sceneName;
}

/// <summary>
/// Used by SpawnManager to signal that a wave has ended
/// </summary>
public struct OnWaveEndEvent : IEvent
{
    public int waveNumber; //The wave number that just ended
}

/// <summary>
/// This event is raised when the player selects a starting deck at the start of a run
/// </summary>
public struct SelectStartingDeckEvent : IEvent
{
    public SelectStartingDeckEvent(DeckSO deckSO)
    {
        this.deckSO = deckSO;
    }

    public DeckSO deckSO;
} 

public struct PlayerDataModifiedEvent : IEvent
{
    public PlayerDataModifiedEvent(PlayerDataContainer playerData, PlayerDataContainer.PlayerDataType dataType)
    {
        this.playerData = playerData;
        this.dataType = dataType;
    }

    public PlayerDataContainer playerData {get; private set;}
    public PlayerDataContainer.PlayerDataType dataType {get; private set;}

}
