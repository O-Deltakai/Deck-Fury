using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType
{
    None,
    Stunned,
    Bleeding,
    Armor_Break,
    Marked
}

[System.Serializable]
public struct StatusEffect
{
    public StatusEffectType statusEffectType;

[Tooltip("Effect strength is the multiplier for the status effect when it gets passed to the entity status effect manager. " + 
"Depending on the status effect type, it could either modify the duration of the status effect or the damage/power.")]
[Range(0, 10)]
    public float effectStrength;

    public StatusEffect(StatusEffectType effectType, float strength)
    {
        statusEffectType = effectType;
        effectStrength = strength;
    }


}