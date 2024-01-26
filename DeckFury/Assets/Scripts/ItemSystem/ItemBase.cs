using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for items and their effects
/// </summary>
public abstract class ItemBase : MonoBehaviour
{

    public delegate void PersistentInitializeEventHandler(ItemBase item);
    public event PersistentInitializeEventHandler OnPersistentInitialize;

    public delegate void InitializeEventHandler(ItemBase item);
    public event InitializeEventHandler OnInitialize;

    public delegate void ProcEventHandler(ItemBase item);
    public event ProcEventHandler OnProcItem;
    public event Action OnProc;


    public StageManager stageManager;
    public PlayerController player;
    public ItemSO itemSO;

    protected bool _persistentInitialized = false;
    public bool PersistentInitialized => _persistentInitialized;


    protected bool _initialized = false;
    public bool Initialized => _initialized;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        
    }

/// <summary>
/// PersistentInitialize will only get called once within the item's lifetime and is used to set persistent states an item
/// may have.
/// </summary>
    public virtual void PersistentInitialize()
    {
        OnPersistentInitialize?.Invoke(this);
        _persistentInitialized = true;
    }

/// <summary>
/// Method called to initialize an item at the start of a battle. If the One Time Effect condition is true in its itemSO, this method
/// will only be called once in its lifetime.
/// </summary>
    public virtual void Initialize()
    {
        OnInitialize?.Invoke(this);
        _initialized = true;
    } 

/// <summary>
/// Should an item have any active triggers that only proc upon certain conditions being met or actions during battle (or outside battle)
/// the Proc method will be called.
/// </summary>
    public virtual void Proc()
    {
        OnProc?.Invoke();
        OnProcItem?.Invoke(this);
    }

/// <summary>
/// Method that is called when exiting battle
/// </summary>
    public virtual void Deactivate()
    {
        _initialized = false;
    }


}
