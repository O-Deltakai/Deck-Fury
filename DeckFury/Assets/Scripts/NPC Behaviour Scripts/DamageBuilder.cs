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
public class DamageBuilder
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

    public DamageBuilder OverrideHitFlashColor(Color color)
    {
        hitFlashColor = color;
        return this;
    }

    public DamageBuilder AddPreDamageActions(DamageContext context)
    {
        preDamageActions.AddRange(context.actionHandlers);
        return this;
    }
    public DamageBuilder AddPostDamageActions(DamageContext context)
    {
        postDamageActions.AddRange(context.actionHandlers);
        return this;
    }


    void Reset()
    {
        hitFlashColor = null;
        hitSFX = null;
        preDamageActions = new();
        postDamageActions = new();
    }


}
