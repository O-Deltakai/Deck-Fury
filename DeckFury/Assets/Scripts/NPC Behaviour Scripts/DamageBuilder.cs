using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public delegate void DamageActionHandler(EntityDamageBuilder damageBuilder, StageEntity entity, ref AttackPayload? payload);

public struct DamageContext
{
    public List<DamageActionHandler> actionHandlers;
}

[Serializable]
public class EntityDamageBuilder
{
#region Events

    /// <summary>
    /// Event that is triggered when the entity takes damage. The first parameter is the attack payload that caused the damage, the second parameter is the amount of damage taken
    /// </summary>
    public event Action<AttackPayload, int> OnDamageTaken;
    
    public event Action OnTakeCritDamage;
    public event Action OnResistDamage;


#endregion

    /// <summary>
    /// List of actions that will be performed before main damage calculation begins
    /// </summary>
    List<DamageActionHandler> preDamageActions = new();
    /// <summary>
    /// List of actions that will be performed after main damage calculation is complete
    /// </summary>
    List<DamageActionHandler> postDamageActions = new();

    /// <summary>
    /// The current attack payload that is being processed
    /// </summary>
    AttackPayload? _currentPayload;
    public AttackPayload CurrentPayload => _currentPayload.Value;


    Color? hitFlashColor;
    public Color? HitFlashColor => hitFlashColor;
    EventReference? hitSFX;
    public EventReference? HitSFX => hitSFX;

    public EntityDamageBuilder SetHitFlashColor(Color color)
    {
        hitFlashColor = color;
        return this;
    }

    public EntityDamageBuilder SetHitSFX(EventReference sfx)
    {
        hitSFX = sfx;
        return this;
    }

    public EntityDamageBuilder AddPreDamageActions(DamageContext context)
    {
        preDamageActions.AddRange(context.actionHandlers);
        return this;
    }
    public EntityDamageBuilder AddPostDamageActions(DamageContext context)
    {
        postDamageActions.AddRange(context.actionHandlers);
        return this;
    }

    /// <summary>
    /// Applies the final damage calculation. Should always be called last in the DamageBuilder chain.
    /// </summary>
    public void Apply(StageEntity entity, AttackPayload payload)
    {
        EntityStatusEffectManager statusEffectManager = entity.StatusManager;
        _currentPayload = payload;

        //Trigger pre-damage calculation actions
        foreach (var action in preDamageActions)
        {
            action.Invoke(this, entity, ref _currentPayload);
        }

        if(statusEffectManager.MarkedForDeath && CurrentPayload.canTriggerMark)
        {
            statusEffectManager.TriggerMarkEffect(CurrentPayload);
        }

        if(_currentPayload.Value.statusEffects != null)
        {
            foreach(StatusEffect statusEffect in CurrentPayload.statusEffects)
            {
                if(statusEffect.statusEffectType == StatusEffectType.None)
                {
                    continue;
                }
                statusEffectManager.TriggerStatusEffect(CurrentPayload, statusEffect);
            }
        }

        int damageToHPAfterModifiers = 0;
    #region Damage Calculation Algorithms
        int ApplyBreakingDamage()
        {
            int originalShieldHP = entity.ShieldHP;
            int shieldDamageMultiplier = 2;
            bool wentThroughShields = false;

            entity.ShieldHP -= CurrentPayload.damage * shieldDamageMultiplier;

            if(entity.ShieldHP < 0)
            {
                //If the attack was breaking damage, half the remaining damage so that damage taken to HP is still 1x efficiency
                damageToHPAfterModifiers = (int)Math.Round(Math.Abs(entity.ShieldHP * 0.5), MidpointRounding.AwayFromZero);                
                entity.ShieldHP = 0;
                if(originalShieldHP > 0)
                {
                    wentThroughShields = true;
                }
            }else
            {   //Shield took all the damage, no damage to HP
                damageToHPAfterModifiers = 0;
                OnDamageTaken?.Invoke(CurrentPayload, originalShieldHP);
                return 0;
            }

        
            //Calculate damage after weakness/resistance modifiers
            if(entity.CheckWeakness(CurrentPayload.attackElement))
            {
                damageToHPAfterModifiers = (int)(damageToHPAfterModifiers * entity.WeaknessModifier);
            }
            if(entity.CheckResistance(CurrentPayload.attackElement))
            {
                damageToHPAfterModifiers = (int)(damageToHPAfterModifiers * entity.ResistModifier);
            }

            //Breaking damage ignores armor but not defense
            damageToHPAfterModifiers = (int)(damageToHPAfterModifiers/entity.Defense);
            if(wentThroughShields)
            {
                OnDamageTaken?.Invoke(CurrentPayload, damageToHPAfterModifiers + originalShieldHP);
            }else
            {
                OnDamageTaken?.Invoke(CurrentPayload, damageToHPAfterModifiers);
            }             
            

            entity.CurrentHP -= damageToHPAfterModifiers;
            return damageToHPAfterModifiers;
        }

        int ApplyPureDamage()
        {
            damageToHPAfterModifiers = CurrentPayload.damage;
            OnDamageTaken?.Invoke(CurrentPayload, damageToHPAfterModifiers);     

            entity.CurrentHP -= damageToHPAfterModifiers;
            return damageToHPAfterModifiers;
        }

        int ApplyRegularDamage()
        {
            int originalShieldHP = entity.ShieldHP;
            bool wentThroughShields = false;

            entity.ShieldHP -= CurrentPayload.damage;
            if(entity.ShieldHP < 0)
            {
                damageToHPAfterModifiers = Math.Abs(entity.ShieldHP);
                entity.ShieldHP = 0;
                if(originalShieldHP > 0)
                {
                    wentThroughShields = true;
                }
            }else
            {//Shield took all the damage, no damage to HP
                damageToHPAfterModifiers = 0;
                OnDamageTaken?.Invoke(CurrentPayload, originalShieldHP);
                return 0;                
            }

            //Check resist/weakness to attack element to calculate final damage
            if(entity.CheckWeakness(CurrentPayload.attackElement))
            {
                damageToHPAfterModifiers = (int)(damageToHPAfterModifiers * entity.WeaknessModifier);
                OnTakeCritDamage?.Invoke();
            }
            if(entity.CheckResistance(CurrentPayload.attackElement))
            {
                damageToHPAfterModifiers = (int)(damageToHPAfterModifiers * entity.ResistModifier);
                OnResistDamage?.Invoke();
            }       

            damageToHPAfterModifiers = (int)(damageToHPAfterModifiers * ((100 - entity.Armor) * 0.01) * entity.Defense);
            if(wentThroughShields)
            {
                OnDamageTaken?.Invoke(CurrentPayload, originalShieldHP + damageToHPAfterModifiers);
            }else
            {
                OnDamageTaken?.Invoke(CurrentPayload, damageToHPAfterModifiers);
            }
            

            entity.CurrentHP -= damageToHPAfterModifiers;
            return damageToHPAfterModifiers;
        }
    #endregion End of Damage Calculation Algorithms


    //Actual damage application
        if(CurrentPayload.attackElement == AttackElement.Breaking)
        {
            ApplyBreakingDamage();
        }else if (CurrentPayload.attackElement == AttackElement.Pure)
        {
            ApplyPureDamage();
        }else
        {
            ApplyRegularDamage();
        }

    //Trigger post-damage calculation actions
        foreach (var action in postDamageActions)
        {
            action.Invoke(this, entity, ref _currentPayload);
        }

        Reset();
    }


    void Reset()
    {
        _currentPayload = null;
        hitFlashColor = null;
        hitSFX = null;
    }

    public void UnsubscribeAll()
    {
        OnDamageTaken = null;
        OnTakeCritDamage = null;
        OnResistDamage = null;
    }


}
