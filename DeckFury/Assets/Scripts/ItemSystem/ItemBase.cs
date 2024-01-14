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


    public abstract void ActivateEffect();


}
