using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for items and their effects
/// </summary>
public abstract class ItemBase : MonoBehaviour
{
    
    public StageManager stageManager;
    public PlayerController player;
    public ItemSO itemSO;

    protected virtual void Start()
    {
        
    }

/// <summary>
/// Method called to initialize an item at the start of a battle. If the One Time Effect condition is true in its itemSO, this method
/// will only be called once in its lifetime.
/// </summary>
    public virtual void Initialize(){} 

/// <summary>
/// Should an item have any active triggers that only proc upon certain conditions being met or actions during battle (or outside battle)
/// the Proc method will be called.
/// </summary>
    public virtual void Proc(){}


}
