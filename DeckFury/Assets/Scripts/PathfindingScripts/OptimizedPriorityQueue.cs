using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class OptimizedPriorityQueue<T> where T : IComparable<T>
{
    private readonly List<(T item, int weight)> items = new List<(T item, int weight)>();

    public void Enqueue(T item, int weight)
    {
        items.Add((item, weight));
        BubbleUp(items.Count - 1);
    }

    public T Dequeue()
    {
        if (items.Count == 0)
        {
            throw new InvalidOperationException("The priority queue is empty.");
        }

        T item = items[0].item;
        items.RemoveAt(0);
        BubbleDown(0);
        return item;
    }

    public int Count => items.Count;

    public bool Any()
    {
        return items.Count > 0;
    }
    public bool Contains(T item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item.Equals(item))
            {
                return true;
            }
        }

        return false;
    }

    private void BubbleUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (items[index].weight < items[parentIndex].weight)
            {
                // Swap the elements at the current index and the parent index.
                (items[index], items[parentIndex]) = (items[parentIndex], items[index]);

                index = parentIndex;
            }
            else
            {
                // The current element is already in the correct position.
                break;
            }
        }
    }

    private void BubbleDown(int index)
    {
        while (index < items.Count)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;

            int minChildIndex = index;
            if (leftChildIndex < items.Count && items[leftChildIndex].weight < items[minChildIndex].weight)
            {
                minChildIndex = leftChildIndex;
            }
            if (rightChildIndex < items.Count && items[rightChildIndex].weight < items[minChildIndex].weight)
            {
                minChildIndex = rightChildIndex;
            }

            if (minChildIndex != index)
            {
                // Swap the elements at the current index and the min child index.
                (items[index], items[minChildIndex]) = (items[minChildIndex], items[index]);

                index = minChildIndex;
            }
            else
            {
                // The current element is already in the correct position.
                break;
            }
        }
    }
}
