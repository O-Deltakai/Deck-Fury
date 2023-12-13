using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object pool system meant for prefabs that implement the IPoolable interface.
/// </summary>
public class GameObjectPool : MonoBehaviour
{
    /// <summary>
    /// A delegate that can be used to set up an object before it is activated. Useful for situations where your object needs to be initialized
    /// with fresh values before being activated.
    /// </summary>
    /// <param name="pooledObject"></param>
    public delegate void SetUpObjectDelegate(GameObject pooledObject);

    public GameObject prefab; // the prefab to instantiate
    Stack<GameObject> readyPool = new Stack<GameObject>();



    void Start()
    {
        ValidatePrefab();

    }

    public void AddObject(int amount)
    {
        if(!ValidatePrefab()){ return; }

        for(int i = 0; i < amount; i++) 
        {
            GameObject instance = Instantiate(prefab, transform);
            IPoolable poolableObject = instance.GetComponent<IPoolable>();

            poolableObject.DisableObject();
            readyPool.Push(instance);

            poolableObject.OnDisableObject += ReadyObject;
            
        }
    }

    public GameObject UseObject(SetUpObjectDelegate setupDelegate = null)
    {
        if(!ValidatePrefab()){ return null; }

        GameObject instance;
        if(readyPool.Count  == 0)
        {
            AddObject(1);
        }
        instance = readyPool.Pop();

        setupDelegate?.Invoke(instance);

        instance.GetComponent<IPoolable>().ActivateObject();

        return instance;

    }

    void ReadyObject(GameObject pooledObject)
    {
        readyPool.Push(pooledObject);
    }


    bool ValidatePrefab()
    {
        // Check if the prefab has a MonoBehaviour that implements IPoolable
        if (prefab.GetComponent<IPoolable>() == null)
        {
            Debug.LogError("The provided prefab does not contain a MonoBehaviour that implements IPoolable on its root object.", this);
            return false; 
        }else
        {
            return true;
        }        
    }

}
