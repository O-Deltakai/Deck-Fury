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

public struct UseCardEvent : IEvent
{
    public CardObjectReference card;
    public CardSO cardSO;
}