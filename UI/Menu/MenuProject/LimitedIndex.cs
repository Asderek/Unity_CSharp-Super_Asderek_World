using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to represent a index without getting negative or overflow the max value
/// </summary>
public class LimitedIndex {
    
    protected int max;
    protected int currentIndex;

    /// <summary>
    /// Class to represent a index without getting negative or overflow the max value
    /// </summary>
    /// <param name="maximumValue">Values will be accepted between zero and maximum value minus one</param>
    public LimitedIndex(int maximumValue)
    {
        max = maximumValue;
        if (max == 0)
            currentIndex = -1;
    }

    /// <summary>
    /// Returns the maximum value of that LimitedIndex
    /// </summary>
    public int Max()
    {
        return max;
    }

    /// <summary>
    /// Returns the current value of that LimitedIndex
    /// </summary>
    public int Get()
    {
        return currentIndex;
    }

    /// <summary>
    /// Returns the index value deviated apart the current index
    /// </summary>
    /// <param name="deviation">Deviation value</param>
    public virtual int IndexMovedBy(int deviation)
    {
        if (max == 0)
            return -1;

        int aux = currentIndex;

        aux += deviation;
        if (aux < 0)
            aux = 0;
        else if (aux >= max)
            aux = max - 1;

        return aux;
    }


    /// <summary>
    /// Update this index deviating apart the current index
    /// </summary>
    /// <param name="deviation">Deviation value</param>
    public virtual void Update(int deviation)
    {
        currentIndex = IndexMovedBy(deviation);
    }

    /// <summary>
    /// Update this index with the given value
    /// </summary>
    /// <param name="newIndex">New index value</param>
    public virtual void Set(int newIndex)
    {
        if (newIndex < 0)
        {
            if (max == 0)
                currentIndex = -1;
            else
                currentIndex = 0;
        }
        else if (newIndex >= max)
            currentIndex = max - 1;
        else
            currentIndex = newIndex;
    }

    /// <summary>
    /// Returns a new LimitedIndex deviated apart the current index
    /// </summary>
    /// <param name="deviation">Deviation value</param>
    public virtual LimitedIndex OffsetCopy(int deviation)
    {
        LimitedIndex index = new LimitedIndex(max);
        index.Set(IndexMovedBy(deviation));
        return index;
    }

    /// <summary>
    /// Return Index[current/max]
    /// </summary>
    public override string ToString()
    {
        return "Index[current/max]: " + currentIndex + "/" + max;
    }

    /// <summary>
    /// Increment the index
    /// </summary>
    /// <param name="obj">The object.</param>
    public static LimitedIndex operator ++(LimitedIndex obj)
    {
        obj.Update(1);
        return obj;
    }

    /// <summary>
    /// Decrement the index
    /// </summary>
    /// <param name="obj">The object.</param>
    public static LimitedIndex operator --(LimitedIndex obj)
    {
        obj.Update(-1);
        return obj;
    }

    /// <summary>
    /// Implements the operator - between two LimitedIndex.
    /// </summary>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    public static int operator -(LimitedIndex one, LimitedIndex two)
    {
        return Mathf.Abs(one.Get() - two.Get());
    }
}
