using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;


public class EntityStatusEffectManager : MonoBehaviour
{
    public delegate void StunnedEventHandler();
    public event StunnedEventHandler OnStunned;
    public event Action OnRecoverStunned;



    [SerializeField] Material SolidColorMaterial;
    [SerializeField] Material SpriteDefaultMaterial;

    SpriteRenderer entitySpriteRenderer;
    StageEntity entity;

[Header("Can Be Affected By")]
    [SerializeField] bool CanBeAffectedByStatusEffects = true;
    [SerializeField] bool CanBeStunned = true;
    [SerializeField] bool CanBleed = true;
    [SerializeField] bool CanBeArmorBroken = true;

    

[Header("Effect Objects")]
    [SerializeField] GameObject markedEffectObject;
    [SerializeField] GameObject bleedingEffectObject;
    [SerializeField] GameObject armorbreakEffectObject;
    [SerializeField] GameObject fireExplosionPrefab;

    public bool MarkedForDeath = false; 

[Header("Default Status Values")]
    [SerializeField] float BaseStunDuration = 1.5f;
    [SerializeField] float BaseMarkedDuration = 3f;
    [SerializeField] float BaseArmorbreakDuration = 10f;

[Header("Status Strength Modifiers")]
    [SerializeField, Range(0, 10)] float stunStrengthMod = 1;
    [SerializeField, Range(0, 10)] float markedStrengthMod = 1;
    [SerializeField, Range(0, 10)] float armorbreakStrengthMod = 1;
    [SerializeField, Range(0, 10)] float bleedStrengthMod = 1;



    [SerializeField] Color bleedingColor = new Color(1, 0.47f, 0.47f);

[Header("Current Status Values")]
    [SerializeField] protected bool _stunned = false;
    public bool Stunned => _stunned;

    [SerializeField] protected bool _bleeding = false;
    public bool Bleeding => _bleeding;

    [SerializeField] protected bool _armorBroken = false;
    public bool ArmorBroken => _armorBroken;


    [Header("SFX")]
    [SerializeField] EventReference triggerMarkedSFX;
    string defaultTriggerMarkedEventPath = "event:/CardSFX/TriggerMarkSFX";

    Coroutine MarkedForDeathCoroutine = null;
    Coroutine ColorFlashCoroutine;


//Bleeding Mechanics
    List<Coroutine> currentBleedingStacks = new List<Coroutine>();
    public double LeftOverBleedDamage = 0;

    [Header("Debug Options")]
    [SerializeField] bool testExsanguiate = false;



    Coroutine CR_ArmorbreakCoroutine = null;
    Coroutine CR_StunnedDurationCoroutine = null;
    Coroutine CR_StunFlashCoroutine = null;

    int _originalArmorValue;


    void Awake()
    {
        entity = GetComponent<StageEntity>();
        entitySpriteRenderer = GetComponent<SpriteRenderer>();      

    }

    void Start()
    {
        _originalArmorValue = entity.Armor;
    }

    void Update()
    {
        if(testExsanguiate)
        {
            Exsanguinate();
            testExsanguiate = false;
        }
    }


    public void TriggerStatusEffect(StatusEffectType statusEffect, AttackPayload payload, float duration = 0)
    {
        if(!CanBeAffectedByStatusEffects){return;}
        switch (statusEffect) {
            case StatusEffectType.None :
                break;

            case StatusEffectType.Stunned :
                StunnedEffect(duration);
                break;

            case StatusEffectType.Bleeding :
                BleedingEffect(payload, duration);
                break;

            case StatusEffectType.Armor_Break:
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
                BleedingEffect(payload, statusEffect.effectStrength);
                break;

            case StatusEffectType.Armor_Break:
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

        //If the entity is already stunned, reset the duration
        if(CR_StunnedDurationCoroutine != null)
        {
            StopCoroutine(CR_StunnedDurationCoroutine);
        }
        if(CR_StunFlashCoroutine != null)
        {
            StopCoroutine(CR_StunFlashCoroutine);
            CR_StunFlashCoroutine = null;
        }


        CR_StunFlashCoroutine = StartCoroutine(FlashColor(Color.yellow, actualDuration));

        entity.CanAct = false;
        entity.CanInitiateMovementActions = false;
        OnStunned?.Invoke();

        CR_StunnedDurationCoroutine = StartCoroutine(StunnedDuration(actualDuration));
    }
    IEnumerator StunnedDuration(float duration)
    {
        _stunned = true;
        yield return new WaitForSeconds(duration);
        entity.CanAct = true;
        entity.CanInitiateMovementActions = true;
        _stunned = false;       
        OnRecoverStunned?.Invoke(); 

        CR_StunnedDurationCoroutine = null;
    }

    
    void BleedingEffect(AttackPayload payload, double strength = 1, double totalDuration = 3, double tickrate = 0.25f)
    {
        if(!CanBleed){return;}
        int totalDamage = (int)(payload.damage * 1.5 * strength);
        if(totalDamage <= 0)
        {
            return;
        }

        entitySpriteRenderer.color = bleedingColor;

        Coroutine bleedStack = StartCoroutine(BleedOverTime(payload, totalDamage, (float)totalDuration, (float)tickrate));
        _bleeding = true;
        currentBleedingStacks.Add(bleedStack);
        StartCoroutine(RemoveBleedStackTimer(bleedStack, (float)totalDuration));
        

        if(bleedingEffectObject){ bleedingEffectObject.SetActive(true); }

    }

    IEnumerator BleedOverTime(AttackPayload payload, int totalDamage, float duration, float tickRate)
    {
        LeftOverBleedDamage += totalDamage;

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
            AttackPayload bleedDamage = new AttackPayload(damageToApply)
            {
                attackElement = AttackElement.Pure,
                attacker = payload.attacker,
                canTriggerMark = false
            };
            entity.HurtEntity(bleedDamage, Color.red);

            // Reduce the accumulated damage by the applied amount
            LeftOverBleedDamage -= damageToApply;
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
            _bleeding = false;
        }
    }

/// <summary>
/// Stops all bleeding stacks and applies the remaining damage to the entity
/// </summary>
    public void Exsanguinate()
    {
        if(currentBleedingStacks.Count == 0)
        {
            return;
        }

        foreach (Coroutine stack in currentBleedingStacks)
        {
            StopCoroutine(stack);
        }
        currentBleedingStacks.Clear();

        int damageToApply = (int)Mathf.Floor((float)LeftOverBleedDamage);
        AttackPayload bleedDamage = new AttackPayload(damageToApply)
        {
            attackElement = AttackElement.Pure,
            attacker = null,
            canTriggerMark = false
        };

        entity.HurtEntity(bleedDamage, Color.red);

        LeftOverBleedDamage = 0;
        _bleeding = false;
        if(bleedingEffectObject){ bleedingEffectObject.SetActive(false); }
    }

    void ArmorBreakEffect(float strength = 1)
    {
        if (_originalArmorValue <= 0) { return; }

        if(CR_ArmorbreakCoroutine != null)
        {
            StopCoroutine(CR_ArmorbreakCoroutine);
        }

        CR_ArmorbreakCoroutine = StartCoroutine(ArmorBreakDuration(BaseArmorbreakDuration, strength));
    }

    IEnumerator ArmorBreakDuration(float duration, float strength)
    {
        entity.Armor = 0;
        armorbreakEffectObject.SetActive(true);
        _armorBroken = true;
        yield return new WaitForSeconds(duration * strength);

        entity.Armor = _originalArmorValue;
        armorbreakEffectObject.SetActive(false);

        CR_ArmorbreakCoroutine = null;
        _armorBroken = false;
    }

    void MarkedForDeathEffect(float strength = 1, float duration = 0)
    {
        float actualDuration = duration;
        if(actualDuration <= 0)
        {
            actualDuration = BaseMarkedDuration;
        }

        MarkedForDeath = true;

        if(MarkedForDeathCoroutine != null)
        {
            StopCoroutine(MarkedForDeathCoroutine);
        }
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


    public AttackPayload TriggerMarkEffect(AttackPayload payload, bool greaterMarked = false)
    {
        CancelMarked();
        //if(!MarkedForDeath){return payload;}
        AttackPayload markedPayload = payload;

        switch (payload.attackElement) 
        {
            case AttackElement.Neutral:
                markedPayload.canTriggerMark = false;
                markedPayload.statusEffects.Clear();
                entity.HurtEntity(markedPayload);    
            break;

            case AttackElement.Blade:
            BleedingEffect(payload);
            break;

            case AttackElement.Fire:
            TriggerFireExplosion(payload);
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


        if(triggerMarkedSFX.IsNull)
        {
            triggerMarkedSFX = RuntimeManager.PathToEventReference(defaultTriggerMarkedEventPath);
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

        while (gracePeriod>=0)
        {    

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

    void TriggerFireExplosion(AttackPayload attackPayload)
    {
        if(!fireExplosionPrefab)
        {
            Debug.LogWarning("Fire explosion prefab has not been set, cannot trigger marked effect for fire.", this);
            return;
        }

        MarkedFireExplosion fireExplosion = Instantiate(fireExplosionPrefab, entity.worldTransform.position, Quaternion.identity)
                                                        .GetComponent<MarkedFireExplosion>();
        AttackPayload modifiedPayload = new AttackPayload
        {
            damage = attackPayload.damage / 2,
            attackElement = AttackElement.Fire,
            canTriggerMark = false
        };

        entity.HurtEntity(modifiedPayload);

        fireExplosion.Trigger(modifiedPayload);                                                

    }


}
