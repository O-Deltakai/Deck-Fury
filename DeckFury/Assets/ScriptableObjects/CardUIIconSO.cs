using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card UI Icons", menuName = "New Card UI Icons", order = 0)]
public class CardUIIconSO : ScriptableObject
{
[Header("Card Element Icons")]
    [field:SerializeField] Sprite Neutral; 
    [field:SerializeField] Sprite Blade;
    [field:SerializeField] Sprite Water;
    [field:SerializeField] Sprite Breaking;
    [field:SerializeField] Sprite Electric; 
    [field:SerializeField] Sprite Fire; 
    [SerializeField] Sprite Pure; 


[Header("Status Effect Icons")]
    [field:SerializeField] Sprite None; 
    [field:SerializeField] Sprite Stunned; 
    [field:SerializeField] Sprite Bleeding;
    [field:SerializeField] Sprite ArmorBreak;
    [field:SerializeField] Sprite MarkedForDeath;


[Header("Element Descriptions")]
[TextArea(5, 20)]
    [SerializeField] string NeutralDescription; 
[TextArea(5, 20)]
    [SerializeField] string BladeDescription; 
[TextArea(5, 20)]
    [SerializeField] string WaterDescription;
[TextArea(5, 20)]
    [SerializeField] string BreakingDescription;
[TextArea(5, 20)]
    [SerializeField] string ElectricDescription;
[TextArea(5, 20)]
    [SerializeField] string FireDescription;
[TextArea(5, 20)]
    [SerializeField] string PureDescription;


[Header("Status Effect Descriptions")]
[TextArea(5, 20)]
    [SerializeField] string NoneDescription; 
[TextArea(5, 20)]
    [SerializeField] string StunnedDescription; 
[TextArea(5, 20)]
    [SerializeField] string BleedingDescription;
[TextArea(5, 20)]
    [SerializeField] string ArmorBreakDescription;
[TextArea(5, 20)]
    [SerializeField] string MarkedForDeathDescription;



    public Sprite GetElementIcon(AttackElement element)
    {
        return element switch
        {
            AttackElement.Neutral => Neutral,
            AttackElement.Blade => Blade,
            AttackElement.Fire => Fire,
            AttackElement.Water => Water,
            AttackElement.Electric => Electric,
            AttackElement.Breaking => Breaking,
            AttackElement.Pure => Pure,
            _ => Neutral,
        };
    }

    public Sprite GetStatusIcon(StatusEffectType statusEffect)
    {

        return statusEffect switch
        {
            StatusEffectType.None => None,
            StatusEffectType.Stunned => Stunned,
            StatusEffectType.Bleeding => Bleeding,
            StatusEffectType.Armor_Break => ArmorBreak,
            StatusEffectType.Marked => MarkedForDeath,
            _ => None,
        };
    }

    public string GetStatusDescription(StatusEffectType statusEffect)
    {
        return statusEffect switch
        {
            StatusEffectType.None => NoneDescription,
            StatusEffectType.Stunned => StunnedDescription,
            StatusEffectType.Bleeding => BleedingDescription,
            StatusEffectType.Armor_Break => ArmorBreakDescription,
            StatusEffectType.Marked => MarkedForDeathDescription,
            _ => NoneDescription,
        };        
    }
    public string GetElementDescription(AttackElement element)
    {
        return element switch
        {
            AttackElement.Neutral => NeutralDescription,
            AttackElement.Blade => BladeDescription,
            AttackElement.Fire => FireDescription,
            AttackElement.Water => WaterDescription,
            AttackElement.Electric => ElectricDescription,
            AttackElement.Breaking => BreakingDescription,
            AttackElement.Pure => PureDescription,
            _ => NeutralDescription
        };        
    }


}
