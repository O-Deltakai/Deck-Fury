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
    public List<StatusEffect> statusEffects;
    public bool canTriggerMark; //Can this attack payload trigger interactions with the MarkedForDeath status effect?
    public bool greaterMarked;
    public GameObject attacker;

    /// <summary>
    /// <para>This is the object that instantiated this specific attack payload. 
    /// This is different from the attacker field, which is the object that is responsible for the attack. </para>
    /// 
    /// <para>For example, if a bullet prefab is instantiated by a turret, the attacker field would be the turret,
    /// while the triggerObject would (normally) be the bullet prefab. </para>
    /// 
    /// <para>However, this field does not always have to be set. It is only used in specific cases where the triggerObject is needed. </para>
    /// </summary>
    public GameObject triggerObject;

    //The sprite used to show the player who or what they were defeated by if this attack payload deals the finishing blow.
    public Sprite attackerSprite;

    //If this payload deals the finishing blow on the player, what does the defeat menu say the player was defeated by?
    public string causeOfDeathNote;

    public bool reflected;

    public AttackPayload(int damage,
                        List<StatusEffect> statusEffects,
                        AttackElement attackElement = AttackElement.Neutral,
                        bool canTriggerMark = false,
                        GameObject attacker = null,
                        Sprite attackerSprite = null,
                        string deathNote = "",
                        bool reflected = false,
                        GameObject triggerObject = null)
    {
        this.damage = damage;
        this.attackElement = attackElement;
        this.statusEffects = statusEffects;
        this.canTriggerMark = canTriggerMark;
        this.greaterMarked = false; // Assign default value to 'greaterMarked' field
        this.attacker = attacker;
        this.attackerSprite = attackerSprite;
        causeOfDeathNote = deathNote;
        this.reflected = reflected;
        this.triggerObject = triggerObject;
    }
    public AttackPayload(int damage, GameObject triggerObject = null)
    {
        this.damage = damage;
        attackElement = AttackElement.Neutral;
        statusEffects = new List<StatusEffect>();
        canTriggerMark = false;
        greaterMarked = false; // Assign default value to 'greaterMarked' field
        attacker = null;
        attackerSprite = null;
        causeOfDeathNote = "";
        reflected = false;
        this.triggerObject = null;
    }


}
