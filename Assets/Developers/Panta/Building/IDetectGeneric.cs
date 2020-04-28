using UnityEngine;
using System.Collections;

public interface IDetectGeneric<T>
{
    T Detect(Transform inOrigin, float inRange, LayerMask inDetectionMask);
}
