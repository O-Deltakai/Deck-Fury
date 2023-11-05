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
    public List<StatusEffectType> statusEffectType;
    public List<StatusEffect> actualStatusEffects;
    public bool canTriggerMark; //Can this attack payload trigger interactions with the MarkedForDeath status effect?
    public GameObject attacker;

    //The sprite used to show the player who or what they were defeated by if this attack payload deals the finishing blow.
    public Sprite attackerSprite;

    //If this payload deals the finishing blow on the player, what does the defeat menu say the player was defeated by?
    public string causeOfDeathNote; 

    public AttackPayload(int damage,
                        List<StatusEffect> statusEffects,
                        List<StatusEffectType> statusEffectTypes,
                        AttackElement attackElement = AttackElement.Neutral,
                        bool canTriggerMark = false,
                        GameObject attacker = null,
                        Sprite attackerSprite = null,
                        string deathNote = "")
    {
        this.damage = damage;
        this.attackElement = attackElement;
        this.actualStatusEffects = statusEffects;
        this.statusEffectType = statusEffectTypes;
        this.canTriggerMark = canTriggerMark;
        this.attacker = attacker;
        this.attackerSprite = attackerSprite;
        causeOfDeathNote = deathNote;
    }
    public AttackPayload(int damage)
    {
        this.damage = damage;
        attackElement = AttackElement.Neutral;
        actualStatusEffects = new List<StatusEffect>();
        statusEffectType = new List<StatusEffectType>();
        canTriggerMark = false;
        attacker = null;
        attackerSprite = null;
        causeOfDeathNote = "";
    }


}
