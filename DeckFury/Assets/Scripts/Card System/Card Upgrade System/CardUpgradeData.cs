using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardUpgradeData
{
    /// <summary>
    /// The string that will be prefixed to the card's name when it is upgraded.
    /// </summary>
    [SerializeField] string upgradePrefix;
    
    /// <summary>
    /// This description will override the card's description when it is upgraded.
    /// </summary>
    [SerializeField, TextArea(10, 10)] string upgradeDescription;
    [SerializeField] bool useOriginalDescription;

    [SerializeField] int baseDamageOverride;
    [SerializeField] bool useOriginalDamage;

    [SerializeField] AttackElement attackElementOverride;
    [SerializeField] bool useOriginalElement;

    [SerializeField] StatusEffect statusEffectOverride;
    [SerializeField] bool useOriginalStatusEffect;

    [SerializeField] List<QuantifiableEffect> upgradeQuanitifiableEffects = new();

    [SerializeField] GameObject targetingReticleOverride;

}
