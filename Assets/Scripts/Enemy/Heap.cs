using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }
    
    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        ++currentItemCount;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        --currentItemCount;

        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);

        return firstItem;
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    void SortDown(T item)
    {
        int childLeftIndex;
        int childRightIndex;
        int swapIndex;

        while(true)
        {
            childLeftIndex = item.HeapIndex * 2 + 1;
            childRightIndex = childLeftIndex + 1;

            if(childLeftIndex < currentItemCount)
            {
                swapIndex = childLeftIndex;

                if(childRightIndex < currentItemCount)
                {
                    if(items[childLeftIndex].CompareTo(items[childRightIndex]) < 0)
                    {
                        //if right index is smaller than left, swap it.
                        swapIndex = childRightIndex;
                    }
                }

                if(item.CompareTo(items[swapIndex]) < 0)
                {
                    //if swap index is smaller than item, swap it.
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    void SortUp(T item)
    {
        int parentIndex;
        T parentItem;

        while(true)
        {
            parentIndex = (item.HeapIndex - 1) / 2;
            parentItem = items[parentIndex];

            if(item.CompareTo(parentItem) > 0) // item cost > parentItem cost
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            if (parentIndex == 0)
                break;
        }
    }

    void Swap(T A, T B)
    {
        items[A.HeapIndex] = B;
        items[B.HeapIndex] = A;

        int temp = A.HeapIndex;
        A.HeapIndex = B.HeapIndex;
        B.HeapIndex = temp;
    }
}

public interface IHeapItem<T>:IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
