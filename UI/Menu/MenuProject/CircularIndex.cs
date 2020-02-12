using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to represent a index to acess a vector or list as if it was circular
/// </summary>
public class CircularIndex : LimitedIndex {

    /// <summary>
    /// Class to represent a index to acess a vector or list as if it was circular
    /// </summary>
    /// <param name="maximumValue">Values will be accepted between zero and maximum value minus one</param>
    public CircularIndex(int maximumValue): base(maximumValue)
    {
    }

    /// <summary>
    /// Returns the index value deviated apart the current index
    /// </summary>
    /// <param name="deviation">Deviation value</param>
    public override int IndexMovedBy(int deviation)
    {
        if (max == 0)
            return -1;

        int aux = currentIndex;

        aux += deviation;
        aux = aux % max;

        if (aux < 0)
            aux += max;

        return aux;
    }

    /// <summary>
    /// Update this index with the given value
    /// </summary>
    /// <param name="newIndex">New index value</param>
    public override void Set(int newIndex)
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
    /// Override the method to returns a new CircularIndex deviated apart the current index
    /// </summary>
    /// <param name="deviation">Deviation value</param>
    public new CircularIndex OffsetCopy(int deviation)
    {
        CircularIndex index = new CircularIndex(max);
        index.Set(IndexMovedBy(deviation));
        return index;
    }

    /// <summary>
    /// Increment the index
    /// </summary>
    /// <param name="obj">The object.</param>
    public static CircularIndex operator ++(CircularIndex obj)
    {
        obj.Update(1);
        return obj;
    }

    /// <summary>
    /// Decrement the index
    /// </summary>
    /// <param name="obj">The object.</param>
    public static CircularIndex operator --(CircularIndex obj)
    {
        obj.Update(-1);
        return obj;
    }

    /// <summary>
    /// Calculate how many times the first index should be increment to achieve the second index
    /// </summary>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    public static int UpDiff(LimitedIndex one, LimitedIndex two)
    {
        int diff = two.Get() - one.Get();

        if (diff > 0)
        {
            return diff;
        }
        return one.Max() + diff;
        
    }

    /// <summary>
    /// Calculate how many times the first index should be decrement to achieve the second index
    /// </summary>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    public static int DownDiff(LimitedIndex one, LimitedIndex two)
    {
        return one.Max() - UpDiff(one,two);
    }

    /// <summary>
    /// Calculate the minimal difference up or down
    /// </summary>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    /// <returns></returns>
    public static int CircularDiff (LimitedIndex one, LimitedIndex two)
    {
        int diff = two.Get() - one.Get();

        if (diff > 0)
        {
            if (diff > one.Max()/2)
            {
                diff -= one.Max();
            }
        } else
        {
            if (diff < -one.Max() / 2)
            {
                diff += one.Max();
            }
        }

        return diff;
    }

    /// <summary>
    /// Implements the operator - between two CircularIndex. Equivalent to <see cref="CircularDiff(LimitedIndex, LimitedIndex)"/>
    /// </summary>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    public static int operator -(CircularIndex one, CircularIndex two)
    {
        return CircularDiff(one, two);
    }
}
