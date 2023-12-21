using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class EntityStatusEffectManager : MonoBehaviour
{
    public delegate void StunnedEventHandler();
    public event StunnedEventHandler OnStunned;



    [SerializeField] Material SolidColorMaterial;
    [SerializeField] Material SpriteDefaultMaterial;

    SpriteRenderer entitySpriteRenderer;
    StageEntity entity;

    [SerializeField] bool CanBeAffectedByStatusEffects = true;
    [SerializeField] bool CanBeStunned = true;
    [SerializeField] bool CanBleed = true;

    [Header("Effect Objects")]
    [SerializeField] GameObject markedEffectObject;
    [SerializeField] GameObject bleedingEffectObject;



    public bool MarkedForDeath = false; 

    [Header("Default Status Values")]
    [SerializeField] float BaseStunDuration = 1.5f;
    [SerializeField] float BaseMarkedDuration = 3f;

    [SerializeField] Color bleedingColor = new Color(1, 0.47f, 0.47f);


    [Header("SFX")]
    [SerializeField] EventReference triggerMarkedSFX;
    string defaultTriggerMarkedEventPath = "event:/CardSFX/TriggerMarkSFX";

    Coroutine MarkedForDeathCoroutine = null;
    Coroutine ColorFlashCoroutine;


    List<Coroutine> currentBleedingStacks = new List<Coroutine>();

    void Start()
    {

        entity = GetComponent<StageEntity>();
        entitySpriteRenderer = GetComponent<SpriteRenderer>();      
    }


    void Update()
    {
        
    }

    public void TriggerStatusEffect(StatusEffectType statusEffect, int damage = 0, float duration = 0)
    {
        if(!CanBeAffectedByStatusEffects){return;}
        switch (statusEffect) {
            case StatusEffectType.None :
                break;

            case StatusEffectType.Stunned :
                StunnedEffect(duration);
                break;

            case StatusEffectType.Bleeding :
                BleedingEffect(damage, duration);
                break;

            case StatusEffectType.ArmorBreak:
                ArmorBreakEffect();
                break;

            case StatusEffectType.Marked:
                MarkedForDeathEffect(duration);
                break;


            default :
                break;
        }
    }
    public void TriggerStatusEffect(AttackPayload payload, StatusEffect statusEffect)
    {
        if(!CanBeAffectedByStatusEffects){return;}
        switch (statusEffect.statusEffectType) 
        {
            case StatusEffectType.None :
                break;

            case StatusEffectType.Stunned :
                StunnedEffect(BaseStunDuration, statusEffect.effectStrength);
                break;

            case StatusEffectType.Bleeding :
                BleedingEffect(payload.damage, statusEffect.effectStrength);
                break;

            case StatusEffectType.ArmorBreak:
                ArmorBreakEffect(statusEffect.effectStrength);
                break;

            case StatusEffectType.Marked:
                MarkedForDeathEffect(statusEffect.effectStrength);
                break;


            default :
                break;
        }
    }



    void StunnedEffect(float duration = 0, float strength = 1)
    {
        if(!CanBeStunned){return;}

        float actualDuration;
        if(duration <= 0)
        {
            actualDuration = BaseStunDuration;
        }else
        {
            actualDuration = duration;
        }

        actualDuration *= strength; 

        ColorFlashCoroutine = StartCoroutine(FlashColor(Color.yellow, actualDuration));
        entity.CanAct = false;
        entity.CanInitiateMovementActions = false;
        OnStunned?.Invoke();
        StartCoroutine(StunnedDuration(actualDuration));
    }
    IEnumerator StunnedDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        entity.CanAct = true;
        entity.CanInitiateMovementActions = true;        
    }


    void BleedingEffect(int baseDamage, double strength = 1, double totalDuration = 3, double tickrate = 0.25f)
    {
        if(!CanBleed){return;}
        int totalDamage = (int)(baseDamage * 2 * strength);

        entitySpriteRenderer.color = bleedingColor;

        Coroutine bleedStack = StartCoroutine(BleedOverTime(totalDamage, (float)totalDuration, (float)tickrate));
        currentBleedingStacks.Add(bleedStack);
        StartCoroutine(RemoveBleedStackTimer(bleedStack, (float)totalDuration));
        

        if(bleedingEffectObject){ bleedingEffectObject.SetActive(true); }

    }

    IEnumerator BleedOverTime(int totalDamage, float duration, float tickRate)
    {
        
        double damagePerTick = totalDamage / (duration / tickRate);

        int totalTicks = Mathf.FloorToInt(duration / tickRate);

        // To handle the fractional damage and rounding
        double accumulatedDamage = 0.0;

        // Apply damage over time
        for (int i = 0; i < totalTicks; i++)
        {
            accumulatedDamage += damagePerTick;

            // Apply the integer part of the accumulated damage to health
            int damageToApply = (int)Mathf.Floor((float)accumulatedDamage);
            AttackPayload bleedDamage = new AttackPayload(damageToApply);
            bleedDamage.attackElement = AttackElement.Pure;
            entity.HurtEntity(bleedDamage, Color.red);

            // Reduce the accumulated damage by the applied amount
            accumulatedDamage -= damageToApply;


            yield return new WaitForSeconds(tickRate);
        }


        
    }

    IEnumerator RemoveBleedStackTimer(Coroutine bleedStack, float duration)
    {
        yield return new WaitForSeconds(duration);
        currentBleedingStacks.Remove(bleedStack);
        if(currentBleedingStacks.Count == 0)
        {
            if(bleedingEffectObject){ bleedingEffectObject.SetActive(false); }
        }
    }

    void ArmorBreakEffect(float strength = 1)
    {

    }

    void MarkedForDeathEffect(float strength = 1, float duration = 0)
    {
        float actualDuration = duration;
        if(actualDuration <= 0)
        {
            actualDuration = BaseMarkedDuration;
        }

        MarkedForDeath = true;
        MarkedForDeathCoroutine = StartCoroutine(MarkedDuration(actualDuration));

        if(markedEffectObject)
        {
            markedEffectObject.SetActive(true);
        }

    }
    IEnumerator MarkedDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        MarkedForDeath = false;

        if(markedEffectObject)
        {
            markedEffectObject.SetActive(false);
        }

    }
    void CancelMarked()
    {
        if(MarkedForDeathCoroutine != null)
        {
            StopCoroutine(MarkedForDeathCoroutine);
        }
        MarkedForDeath = false;
        if(markedEffectObject){ markedEffectObject.SetActive(false); }
    }


    public AttackPayload TriggerMarkEffect(AttackPayload payload)
    {
        //if(!MarkedForDeath){return payload;}
        AttackPayload markedPayload = payload;

        switch (payload.attackElement) 
        {
            case AttackElement.Neutral:
                markedPayload.canTriggerMark = false;
                entity.HurtEntity(markedPayload);    
            break;

            case AttackElement.Blade:
            BleedingEffect(payload.damage);
            break;

            case AttackElement.Fire:
            break;

            case AttackElement.Water:
            break;

            case AttackElement.Electric:
            StunnedEffect();
            break;

            case AttackElement.Breaking:
            ArmorBreakEffect();
            break;

            case AttackElement.Pure:
                return markedPayload;
            



            default :
                
            break;
        }

        CancelMarked();

        if(triggerMarkedSFX.IsNull)
        {
            triggerMarkedSFX = EventReference.Find(defaultTriggerMarkedEventPath);
        }
        RuntimeManager.PlayOneShot(triggerMarkedSFX, transform.position);


        return markedPayload;
    }



    public void SetSolidColor(Color color)
    {
        entitySpriteRenderer.material = SolidColorMaterial;
        entitySpriteRenderer.color = color;
    }

    public void SetNormalSprite()
    {
        entitySpriteRenderer.material = SpriteDefaultMaterial;
        entitySpriteRenderer.color = Color.white;
    }

    public IEnumerator FlashColor(Color color, float duration, float tickrate = 0.1f)
    {
        float gracePeriod = duration;

        while (gracePeriod>=0){
            

            SetSolidColor(color);
            gracePeriod -= tickrate;

            yield return new WaitForSeconds(tickrate);
            SetNormalSprite();
            gracePeriod -= tickrate;

            yield return new WaitForSeconds(tickrate);
            
        }
    }
    public void CancelFlashColor()
    {
        StopCoroutine(ColorFlashCoroutine);
        SetNormalSprite();
    }


}
