using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for items and their effects
/// </summary>
public abstract class ItemBase : MonoBehaviour
{
    public delegate void InitializeEventHandler(ItemBase item);
    public event InitializeEventHandler OnInitialize;

    public delegate void ProcEventHandler(ItemBase item);
    public event ProcEventHandler OnProc;


    public StageManager stageManager;
    public PlayerController player;
    public ItemSO itemSO;

    protected bool _initialized = false;
    public bool Initialized => _initialized;

    protected virtual void Start()
    {
        
    }

/// <summary>
/// Method called to initialize an item at the start of a battle. If the One Time Effect condition is true in its itemSO, this method
/// will only be called once in its lifetime.
/// </summary>
    public virtual void Initialize()
    {
        OnInitialize?.Invoke(this);
    } 

/// <summary>
/// Should an item have any active triggers that only proc upon certain conditions being met or actions during battle (or outside battle)
/// the Proc method will be called.
/// </summary>
    public virtual void Proc()
    {
        OnProc?.Invoke(this);
    }

/// <summary>
/// Method that is called when exiting battle
/// </summary>
    public virtual void Deactivate(){}


}
