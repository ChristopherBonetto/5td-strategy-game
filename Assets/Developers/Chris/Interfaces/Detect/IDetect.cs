using UnityEngine;
using Types;

public interface IDetect
{
    EntityBehavior DetectArea(Transform inStartDetectPoint, float inViewRadius, LayerMask inWantedLayer);
}
