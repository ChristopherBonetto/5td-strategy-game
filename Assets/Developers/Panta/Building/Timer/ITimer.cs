using System.Collections;
using System;

public interface ITimer
{
    bool Timer(ref float timeElapsed, ref float timeToElaps);
    IEnumerator ActionTimer(float timeElapsed, float timeToElaps, Action callback);
}
