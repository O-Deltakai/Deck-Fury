using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[System.Serializable]
public class QuantifiableEffect
{
    [field:SerializeField] public string EffectName{get; private set;}
    [field:SerializeField] public int IntegerQuantity{get; private set;}
    [field:SerializeField] public float FloatQuantity{get; private set;}
    [field:SerializeField] public bool CanBeModified{get; private set;} = false;
    [field:SerializeField] public float ModCoefficient{get; private set;} = 1;

    public (int IntVal, float FloatVal) GetValueTuple()
    {
        if(IntegerQuantity == 0)
        {
            return (0, FloatQuantity);
        }else
        {
            return(IntegerQuantity, 0);
        }
    }

    public object GetValueDynamic()
    {
        if(IntegerQuantity == 0 && FloatQuantity != 0)
        {
            return FloatQuantity;
        }else
        if(FloatQuantity == 0 && IntegerQuantity != 0)
        {
            return IntegerQuantity;
        }else
        {
            Debug.LogWarning("Quantifiable effect for " + EffectName + " was not set properly, returned 0");
            return 0;
        }
    }


}


[CreateAssetMenu(fileName = "Card Data", menuName = "New Card", order = 0)]
//Scriptable object used for storing data on cards
public class CardSO : ScriptableObject
{
    [field:SerializeField] public string CardName {get; private set;}
[TextArea(10, 20)]
    [SerializeField] string CardDescription;

[Header("Combat Attributes")]
    [SerializeField] int BaseDamage;
    [field:SerializeField] public AttackElement AttackElement {get; private set;}
    [field:SerializeField] public CardType CardType {get; private set;}
    [field:SerializeField] public StatusEffectType StatusEffect {get; private set;}
    [field:SerializeField] public StatusEffect statusEffect {get; private set;}

    [field:SerializeField] public int EnergyCost {get; private set;}
    [Range(1,3)]
    [SerializeField] int CardTier = 1;
    [field:SerializeField] public List<QuantifiableEffect> QuantifiableEffects {get;private set;}


[Header("Advanced Properties")]
    [SerializeField] Sprite CardImage;

    //PlayerAnimation is animation the card will trigger to play when used. 
    [field:SerializeField] public AnimationClip PlayerAnimation{get; private set;}
    //Determines if the card uses an animation event to trigger its effect. Should only be true on cards that have PlayerAnimation set.
    [field:SerializeField] public bool UsesAnimationEvent{get; private set;}

    //The EffectPrefab is the Prefab that is used to activate the card's effects on the scene.
    [field:SerializeField] public GameObject EffectPrefab{get; private set;}

    //If the card instantiates additional objects during the script within the EffectPrefab, this list stores references
    //to those prefabs so that they may be instantiated on activation of the EffectPrefab, or pooled via the ObjectPoolManager
    //and then enabled with the EffectPrefab.
    [field:SerializeField] public List<GameObject> ObjectSummonList{get; private set;}

    //Bool that determines whether objects from the ObjectSummonList are pooled or not - determines whether the card instantiates
    //new objects during runtime or objects are pooled and then enabled via scripts.
    [field:SerializeField] public bool ObjectSummonsArePooled{get; private set;}
    [field:SerializeField] public bool UseTargetingReticle{get; private set;}
    [field:SerializeField] public bool ReticleIsStatic{get; private set;}
    [field:SerializeField] public GameObject TargetingReticle{get; private set;}
    [field:SerializeField] public Vector2Int TargetingReticleOffSet{get; private set;}

    [Header("SFX")]

    [SerializeField] EventReference _onActivationSFX;
    public EventReference OnActivationSFX => _onActivationSFX;

    public int GetBaseDamage()
    {return BaseDamage;}

    public string GetCardDescription()
    {return CardDescription;}

    public Sprite GetCardImage()
    {return CardImage;}

    public int GetCardTier()
    {return CardTier;}

    public string GetFormattedDescription()
    {
        string formattedDescription = CardDescription;

        formattedDescription = formattedDescription.Replace("BD", BaseDamage.ToString());

        for(int i = 0; i < QuantifiableEffects.Count ; i++) 
        {
            string qEffect = "Q" + i;
            formattedDescription = formattedDescription.Replace(qEffect, QuantifiableEffects[i].GetValueDynamic().ToString());
        }

        return formattedDescription;
    }    


}
