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


    protected void InitializeAwakeVariables()
    {
        stageManager = StageManager.Instance;
        //If cardSO does not have its ObjectSummons pooled, will set the ObjectSummonList to be the same as that defined within the cardSO.
        if(!cardSO.ObjectSummonsArePooled)
        {
            ObjectSummonList = cardSO.ObjectSummonList;
        }

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


        attackPayload.statusEffectType.Add(cardSO.StatusEffect);

    }

    protected virtual void Awake()
    {
        InitializeAwakeVariables();
    }

    //All card effects should begin anchored on the world transform of the player
    protected void InitializeStartingStates()
    {
        transform.position = player.worldTransform.position;
    }

    private void OnEnable() 
    {
        if(player != null)
        {
            transform.position = player.worldTransform.position;   
        }
    }

    protected virtual void Start()
    {
        InitializeStartingStates();
    }

    //The main method that gets called whenever the card is used. Should contain all necessary statements and calls
    //to make the card work.
    public abstract void ActivateCardEffect();


    //Method to call when it is time to disable the effect prefab
    protected abstract IEnumerator DisableEffectPrefab();


}
