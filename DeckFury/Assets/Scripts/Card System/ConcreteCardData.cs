using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConcreteCardData
{
    [SerializeReference] public CardSO cardSO;
    
    public ConcreteCardData(CardSO cardSO)
    {
        this.cardSO = cardSO;
        cardName = cardSO.CardName;
        cardDescription = cardSO.CardDescriptionProperty;

        baseDamage = cardSO.GetBaseDamage();
        attackElement = cardSO.AttackElement;
        cardType = cardSO.CardType;

        statusEffectType = cardSO.oldStatusEffectType;
        statusEffect = cardSO.statusEffect;

        //Make deep copy of quantifiable effects
        quantifiableEffects = new List<QuantifiableEffect>();
        foreach(QuantifiableEffect effect in cardSO.QuantifiableEffects)
        {
            quantifiableEffects.Add(effect);
        }
    }

    public string cardName;
    public string cardDescription;

    //Combat Attributes
    public int baseDamage;
    public AttackElement attackElement;
    public CardType cardType;

    public StatusEffectType statusEffectType;
    public StatusEffect statusEffect;

    public List<QuantifiableEffect> quantifiableEffects;


}
