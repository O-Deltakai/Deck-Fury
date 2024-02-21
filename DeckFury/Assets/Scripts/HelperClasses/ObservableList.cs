using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A slight modification of the base List class where adding or removing an item will raise an event with the item that was
/// added/removed as the parameter.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObservableList<T> : List<T> 
{
    public delegate void AddItemEventHandler(T item);
    public event AddItemEventHandler OnAddItem;

    public delegate void RemoveItemEventHandler(T item);
    public event RemoveItemEventHandler OnRemoveItem;

    public new void Add(T item)
    {
        base.Add(item);
        OnAddItem?.Invoke(item);
    }

    public new void Remove(T item)
    {
        base.Remove(item);
        OnRemoveItem?.Invoke(item);
    }


}
