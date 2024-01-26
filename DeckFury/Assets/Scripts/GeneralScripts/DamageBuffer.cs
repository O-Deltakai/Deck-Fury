using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBufferList
{
    public event Action OnBufferListEmpty;
    public event Action OnBufferListChanged;

    public event Action<DamageBuffer> OnBufferRemoved;

    List<DamageBuffer> _damageBuffers = new List<DamageBuffer>();
    public IReadOnlyList<DamageBuffer> DamageBuffers => _damageBuffers;

    public void Add(DamageBuffer damageBuffer)
    {
        _damageBuffers.Add(damageBuffer);
        OnBufferListChanged?.Invoke();
    }

    public void Remove(DamageBuffer damageBuffer)
    {
        _damageBuffers.Remove(damageBuffer);
        if(_damageBuffers.Count == 0)
        {
            OnBufferListEmpty?.Invoke();
        }
        OnBufferListChanged?.Invoke();
        OnBufferRemoved?.Invoke(damageBuffer);
        damageBuffer.TriggerRemovalEvent();
    }

    public void Clear()
    {
        foreach (var DamageBuffer in _damageBuffers)
        {
            OnBufferRemoved?.Invoke(DamageBuffer);
            DamageBuffer.TriggerRemovalEvent();
        }

        _damageBuffers.Clear();
        OnBufferListEmpty?.Invoke();
        OnBufferListChanged?.Invoke();
    }

     

}


public class DamageBuffer 
{
    public event Action OnBufferRemoved;   
    public GameObject source; 

    public void TriggerRemovalEvent()
    {
        OnBufferRemoved?.Invoke();
    }

}
