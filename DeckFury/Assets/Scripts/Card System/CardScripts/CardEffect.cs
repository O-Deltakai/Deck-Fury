using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Abstract class containing common functionality and variables between all EffectPrefabs for cards
public abstract class CardEffect : MonoBehaviour
{
    public StageManager stageManager;
    public PlayerController player;
    public CardSO cardSO;
    public List<GameObject> ObjectSummonList = new List<GameObject>();
    public AttackPayload attackPayload = new AttackPayload();

    public bool greaterMark = false;

    protected void InitializeAwakeVariables()
    {
        stageManager = StageManager.Instance;
        //If cardSO does not have its ObjectSummons pooled, will set the ObjectSummonList to be the same as that defined within the cardSO.
        if(!cardSO.ObjectSummonsArePooled)
        {
            ObjectSummonList = cardSO.ObjectSummonList;
        }

        InitializeAttackPayload();


    }

    protected void InitializeAttackPayload()
    {
        if(player != null)
        {
            attackPayload.attacker = player.gameObject;
            attackPayload.attackerSprite = player.gameObject.GetComponent<SpriteRenderer>().sprite;
        }

        attackPayload.damage = cardSO.GetBaseDamage();
        attackPayload.attackElement = cardSO.AttackElement;
        
        if(cardSO.AttackElement == AttackElement.Pure)
        {
            attackPayload.canTriggerMark = false;
        }else
        {
            attackPayload.canTriggerMark = true;
        }

        if(cardSO.statusEffect.statusEffectType != StatusEffectType.None)
        {
            attackPayload.statusEffects = new List<StatusEffect>
            {
                cardSO.statusEffect
            };
        }
    }

    protected virtual void Awake()
    {
        InitializeAwakeVariables();
    }

    //All card effects should begin anchored on the world transform of the player
    protected void InitializeStartingStates()
    {
        if(player != null)
        {
            attackPayload.attacker = player.gameObject;
            attackPayload.attackerSprite = player.gameObject.GetComponent<SpriteRenderer>().sprite;
        }        
        transform.position = player.worldTransform.position;
    }

    private void OnEnable() 
    {
        if(player != null)
        {
            attackPayload.attacker = player.gameObject;
            attackPayload.damage = (int)(attackPayload.damage * player.GlobalDamageMultiplier * player.CardDamageMultiplier);
            transform.position = player.worldTransform.position;   
        }
    }

    protected virtual void Start()
    {
        InitializeStartingStates();
    }

    //The main method that gets called whenever the card is used. Should contain all necessary statements and calls
    //to make the card work.
    public virtual void ActivateCardEffect()
    {
        
    }


    //Method to call when it is time to disable the effect prefab
    protected virtual IEnumerator DisableEffectPrefab()
    {
        yield break;
    }


}
