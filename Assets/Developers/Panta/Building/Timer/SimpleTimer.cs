using System.Collections;
using UnityEngine;
using System;

/// <summary>
/// Simple timer class, support timer on update, 
/// timer on coroutine.
/// </summary>
public class SimpleTimer : ITimer
{
    /// <summary>
    /// Calculate the time elapsed,
    /// return true if time is elapsed, else false.
    /// </summary>
    public bool Timer(ref float timeElapsed, ref float timeToElaps)
    {
        if (timeElapsed > timeToElaps)
        {
            timeElapsed = 0;
            return true;
        }

        timeElapsed += Time.deltaTime;
        return false;
    }

    /// <summary>
    /// Perfom an action after delay.
    /// </summary>
    public IEnumerator ActionTimer(float timeElapsed, float timeToElaps, Action callback)
    {
        while(!Timer(ref timeElapsed, ref timeToElaps))
        {
            yield return null;
        }

        callback?.Invoke();
    }
}
