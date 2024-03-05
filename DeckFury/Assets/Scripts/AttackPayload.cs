using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///A container data type which stores information on the properties of an incoming attack.
///All variables within the attack payload may be modified freely. 
///</summary>
[System.Serializable]
public struct AttackPayload
{

    public int damage;
    public AttackElement attackElement;
    public List<StatusEffectType> oldStatusEffectType;
    public List<StatusEffect> statusEffects;
    public bool canTriggerMark; //Can this attack payload trigger interactions with the MarkedForDeath status effect?
    public bool greaterMarked;
    public GameObject attacker;

    //The sprite used to show the player who or what they were defeated by if this attack payload deals the finishing blow.
    public Sprite attackerSprite;

    //If this payload deals the finishing blow on the player, what does the defeat menu say the player was defeated by?
    public string causeOfDeathNote;

    public bool reflected;

    public AttackPayload(int damage,
                        List<StatusEffect> statusEffects,
                        List<StatusEffectType> statusEffectTypes,
                        AttackElement attackElement = AttackElement.Neutral,
                        bool canTriggerMark = false,
                        GameObject attacker = null,
                        Sprite attackerSprite = null,
                        string deathNote = "",
                        bool reflected = false)
    {
        this.damage = damage;
        this.attackElement = attackElement;
        this.statusEffects = statusEffects;
        this.oldStatusEffectType = statusEffectTypes;
        this.canTriggerMark = canTriggerMark;
        this.greaterMarked = false; // Assign default value to 'greaterMarked' field
        this.attacker = attacker;
        this.attackerSprite = attackerSprite;
        causeOfDeathNote = deathNote;
        this.reflected = reflected;
    }
    public AttackPayload(int damage)
    {
        this.damage = damage;
        attackElement = AttackElement.Neutral;
        statusEffects = new List<StatusEffect>();
        oldStatusEffectType = new List<StatusEffectType>();
        canTriggerMark = false;
        greaterMarked = false; // Assign default value to 'greaterMarked' field
        attacker = null;
        attackerSprite = null;
        causeOfDeathNote = "";
        reflected = false;
    }


}
