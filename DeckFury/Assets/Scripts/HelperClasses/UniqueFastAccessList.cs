using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages a collection of unique elements with fast index access and efficient removal. Ideal for scenarios requiring frequent access and updates to a unique set of items.
/// </summary>
/// <typeparam name="T">Type of elements. Must be unique across the list.</typeparam>
/// <remarks>
/// Adding an existing item throws ArgumentException. Accessing or removing items with invalid index throws ArgumentOutOfRangeException. Not thread-safe.
/// </remarks>
public class UniqueFastAccessList<T> : IEnumerable<T>
{
    private List<T> items = new();
    private Dictionary<T, int> itemToIndexMap = new();

    public int Count => items.Count;

    public UniqueFastAccessList(List<T> values) 
    {
        foreach (var value in values)
        {
            Add(value);
        }
    }

    public UniqueFastAccessList(UniqueFastAccessList<T> other) 
    {
        foreach (var value in other.items)
        {
            Add(value);
        }
    }

    // Add an item to the list
    public void Add(T item)
    {
        if (itemToIndexMap.ContainsKey(item))
        {
            throw new ArgumentException("Item already exists in the list.");
        }

        items.Add(item);
        itemToIndexMap[item] = items.Count - 1;
    }

    // Get an item by index
    public T Get(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }
        return items[index];
    }

    // Remove an item by index
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        T itemToRemove = items[index];
        
        // Move the last item to the removed position and update the map if not removing the last item
        if (index < items.Count - 1)
        {
            T lastItem = items[items.Count - 1];
            items[index] = lastItem;
            itemToIndexMap[lastItem] = index;
        }

        // Remove the last item
        items.RemoveAt(items.Count - 1);
        itemToIndexMap.Remove(itemToRemove);

        // No need to update other indices because we're either removing the last item
        // or moving the last item into the position of the removed item
    }

    public void Clear()
    {
        items.Clear();
        itemToIndexMap.Clear();
    }
    
    public bool Contains(T item) => itemToIndexMap.ContainsKey(item);

    public IEnumerator<T> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
