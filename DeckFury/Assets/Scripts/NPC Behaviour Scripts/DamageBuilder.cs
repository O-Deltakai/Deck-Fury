using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public struct DamageContext
{
    public AttackPayload attackPayload;
    public List<ActionHandler> actionHandlers;
}

[Serializable]
public class EntityDamageBuilder
{

    /// <summary>
    /// List of actions that will be performed before main damage calculation begins
    /// </summary>
    List<ActionHandler> preDamageActions = new();
    List<ActionHandler> postDamageActions = new();

    [SerializeField] Color baseHitFlashColor = Color.white;
    [SerializeField] EventReference basehitSFX;

    AttackPayload sourceAttackPayload;
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
    public void Build(StageEntity entity, AttackPayload attackPayload)
    {
        foreach (var action in preDamageActions)
        {
            action.Invoke();
        }

        sourceAttackPayload = attackPayload;


        foreach (var action in postDamageActions)
        {
            action.Invoke();
        }

        Reset();
    }


    void Reset()
    {
        hitFlashColor = null;
        hitSFX = null;
    }


}
