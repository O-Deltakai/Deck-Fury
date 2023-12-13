using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface designed to work with the GameObjectPool system.
/// </summary>
public interface IPoolable 
{

    public delegate void DisableObjectEvent(GameObject poolableObject);
    public event DisableObjectEvent OnDisableObject;

    public delegate void ActivateObjectEvent(GameObject poolableObject);
    public event ActivateObjectEvent OnActivateObject;

    public void ActivateObject();
    public void DisableObject();


}
