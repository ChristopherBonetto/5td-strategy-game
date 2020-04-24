using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

public class DetectBehaviors : IDetect
{
    private Collider[] inDetectedColliders;
    private int inNumberOfCollidersDetected;

    public EntityBehavior DetectArea(Transform inStartDetectPoint, float inViewRadius, LayerMask inWantedLayer)
    {
        inNumberOfCollidersDetected = Physics.OverlapSphereNonAlloc(inStartDetectPoint.transform.position, inViewRadius, inDetectedColliders, inWantedLayer);

        for (int i = 0; i < inNumberOfCollidersDetected; i++)
        {
            if (inDetectedColliders[i])
            {
                ITakeDamage damageInterface = inDetectedColliders[i].GetComponent<ITakeDamage>();

                if(damageInterface != null)
                {
                    RaycastHit hit;

                    if (Physics.Raycast(inStartDetectPoint.transform.position, inDetectedColliders[i].transform.position - inStartDetectPoint.transform.position, out hit, inViewRadius))
                    {
                        if (hit.transform.name != inDetectedColliders[i].transform.name)
                        {
                            return null;
                        }
                        else
                        {
                            EntityBehavior tempEntity = hit.transform.GetComponent<EntityBehavior>();

                            if (!tempEntity.IsBusy)
                            {
                                return tempEntity;
                            }
                        }

                    }
                }
            }
        }
        return null;
    }
}
