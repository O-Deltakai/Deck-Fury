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

    [Tooltip("This string will be evaluated as a math expression to determine the value of the effect.")]
    [SerializeField] string _expression;
    [Tooltip("If true, the expression will be evaluated as an int, otherwise it will be evaluated as a float")]
    [SerializeField] bool _expressionIsInt;

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

    public int EvaluateExpressionInt()
    {
        if(_expression == "")
        {
            return IntegerQuantity;
        }else
        {
            return (int)MathExpressionEvaluator.Evaluate(_expression);
        }
    }

    public float EvaluateExpressionFloat()
    {
        if(_expression == "")
        {
            return (int)FloatQuantity;
        }else
        {
            return MathExpressionEvaluator.Evaluate(_expression);
        }
    }

    public object GetValueDynamic()
    {
        if(IntegerQuantity == 0 && FloatQuantity != 0 &&  string.IsNullOrWhiteSpace(_expression))
        {
            return FloatQuantity;
        }else
        if(FloatQuantity == 0 && IntegerQuantity != 0 && string.IsNullOrWhiteSpace(_expression))
        {
            return IntegerQuantity;
        }else
        if(FloatQuantity == 0 && IntegerQuantity == 0 && !string.IsNullOrWhiteSpace(_expression))
        {
            if(_expressionIsInt)
            {
                return EvaluateExpressionInt();
            }else
            {
                return EvaluateExpressionFloat();
            }
        }
        else
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
    public string CardDescriptionProperty => CardDescription;


[Header("Combat Attributes")]
    [SerializeField] int BaseDamage;
    [field:SerializeField] public AttackElement AttackElement {get; private set;}
    [field:SerializeField] public CardType CardType {get; private set;}
    [field:SerializeField] public StatusEffectType oldStatusEffectType {get; private set;}
    [field:SerializeField] public StatusEffect statusEffect {get; private set;}

    [SerializeField, Range(1, 3)] int CardTier = 1;
    [field:SerializeField] public List<QuantifiableEffect> QuantifiableEffects {get;private set;}

    [Header("Upgrades")]
    [Tooltip("Determines if the card is an upgraded variant of another card.")]
    [SerializeField] bool _isUpgraded = false;
    /// <summary>
    /// Determines if the card is an upgraded variant of another card.
    /// </summary>
    public bool IsUpgraded => _isUpgraded;
    [SerializeField] CardSO baseCard;
    public CardSO BaseCard => baseCard;
    [SerializeField] List<CardUpgradeData> upgrades;
    public List<CardUpgradeData> Upgrades => upgrades;
    public bool HasUpgrades => upgrades.Count > 0;


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
    [Tooltip("If dynamic reticle is true, the reticle is considered to have its own targeting logic and will not use the aimpoint controller for targeting.")]
    [SerializeField] bool _dynamicReticle = false;
    public bool DynamicReticle => _dynamicReticle;
    [field:SerializeField] public bool ReticleIsStatic{get; private set;}
    [field:SerializeField] public GameObject TargetingReticle{get; private set;}
    [field:SerializeField] public Vector2Int TargetingReticleOffSet{get; private set;}


    [Header("SFX")]

    [SerializeField] EventReference _onActivationSFX;
    public EventReference OnActivationSFX => _onActivationSFX;

    void OnValidate()
    {
        if(!EffectPrefab) { return; }

        if (!EffectPrefab.TryGetComponent<CardEffect>(out _))
        {
            Debug.LogError("Effect Prefab for " + CardName + " does not have a CardEffect component attached to it. Please attach a CardEffect component to the EffectPrefab.");
        }
    }


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
