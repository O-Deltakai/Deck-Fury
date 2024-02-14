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