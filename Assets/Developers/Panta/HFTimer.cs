using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HFTimer
{
    public float MaxTime;  // It's supposed that this value doesn't change later.
    public float CurrentTime;


    public HFTimer(float _maxTime)
    {
        MaxTime = _maxTime;
        CurrentTime = _maxTime;
    }

    #region METHODS

    public IEnumerator DecreaseTime()
    {
        while(CurrentTime > 0)
        {
            // Decrease time in a loop until it reach 0;
            CurrentTime -= Time.deltaTime;
            yield return false;
        }

        // Reset.
        CurrentTime = MaxTime;
        yield return true;
    }

    public IEnumerator DecreaseTime(Action callback)
    {
        while (CurrentTime > 0)
        {
            // Decrease time in a loop until it reach 0;
            CurrentTime -= Time.deltaTime;
            yield return false;
        }

        // Invoke callback
        callback?.Invoke();
        Debug.Log("Callback success");

        // Reset.
        CurrentTime = MaxTime;
        yield return true;
    }

    public IEnumerator DecreaseTime<T>(Action<T> callback, T arg1)
    {
        while (CurrentTime > 0)
        {
            // Decrease time in a loop until it reach 0;
            CurrentTime -= Time.deltaTime;
            yield return false;
        }

        // Invoke callback
        callback?.Invoke(arg1);
        Debug.Log("Callback success");

        // Reset.
        CurrentTime = MaxTime;
        yield return true;
    }

    public IEnumerator DecreaseTime<T, U>(Action<T, U> callback, T arg1, U arg2)
    {
        while (CurrentTime > 0)
        {
            // Decrease time in a loop until it reach 0;
            CurrentTime -= Time.deltaTime;
            yield return false;
        }

        // Invoke callback
        callback?.Invoke(arg1, arg2);
        Debug.Log("Callback success");

        // Reset.
        CurrentTime = MaxTime;
        yield return true;
    }
    #endregion
}
