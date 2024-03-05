using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public delegate void DamageActionHandler(StageEntity entity, AttackPayload payload);

public struct DamageContext
{
    public AttackPayload attackPayload;
    public List<DamageActionHandler> actionHandlers;
}

[Serializable]
public class EntityDamageBuilder
{

    /// <summary>
    /// List of actions that will be performed before main damage calculation begins
    /// </summary>
    List<DamageActionHandler> preDamageActions = new();
    /// <summary>
    /// List of actions that will be performed after main damage calculation is complete
    /// </summary>
    List<DamageActionHandler> postDamageActions = new();

    [SerializeField] Color baseHitFlashColor = Color.white;
    [SerializeField] EventReference basehitSFX;

    AttackPayload? sourceAttackPayload;
    public AttackPayload? SourceAttackPayload => sourceAttackPayload;
    Color? hitFlashColor;
    EventReference? hitSFX;

    public EntityDamageBuilder OverrideHitFlashColor(Color color)
    {
        hitFlashColor = color;
        return this;
    }

    public EntityDamageBuilder OverrideHitSFX(EventReference sfx)
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
    /// Builds the final damage calculation. Should always be called last in the DamageBuilder chain.
    /// </summary>
    public void Build(StageEntity entity, AttackPayload payload)
    {
        EntityStatusEffectManager statusEffectManager = entity.StatusEffectManager;

        sourceAttackPayload = payload;

        foreach (var action in preDamageActions)
        {
            action.Invoke(entity, payload);
        }

        if(statusEffectManager.MarkedForDeath && payload.canTriggerMark)
        {
            statusEffectManager.TriggerMarkEffect(payload);
        }

        if(payload.statusEffects != null)
        {
            foreach(StatusEffect statusEffect in payload.statusEffects)
            {
                if(statusEffect.statusEffectType == StatusEffectType.None)
                {
                    continue;
                }
                statusEffectManager.TriggerStatusEffect(payload, statusEffect);
            }
        }

        int damageToHPAfterModifiers = 0;

        int CalculateBreakingDamage()
        {
            int originalShieldHP = entity.ShieldHP;
            int shieldDamageMultiplier = 2;
            bool wentThroughShields = false;

            entity.ShieldHP -= payload.damage * shieldDamageMultiplier;

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
            {
                damageToHPAfterModifiers = 0;
            }

            //Breaking damage ignores armor but not defense
            if(damageToHPAfterModifiers != 0)
            {
                damageToHPAfterModifiers = (int)(damageToHPAfterModifiers/entity.Defense);                
            }

            //Calculate damage after weakness/resistance modifiers
            if(entity.CheckWeakness(payload.attackElement))
            {
                damageToHPAfterModifiers = (int)(damageToHPAfterModifiers * entity.WeaknessModifier);
            }
            if(entity.CheckResistance(payload.attackElement))
            {
                damageToHPAfterModifiers = (int)(damageToHPAfterModifiers * entity.ResistModifier);
            }

            return damageToHPAfterModifiers;
        }

        int CalculatePureDamage()
        {
            damageToHPAfterModifiers = payload.damage;

            return damageToHPAfterModifiers;
        }





        foreach (var action in postDamageActions)
        {
            action.Invoke(entity, payload);
        }

        Reset();
    }


    void Reset()
    {
        sourceAttackPayload = null;
        hitFlashColor = null;
        hitSFX = null;
    }


}
