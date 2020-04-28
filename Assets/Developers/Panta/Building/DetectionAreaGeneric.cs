using UnityEngine;
using System.Collections;

public class DetectionAreaGeneric<T> : IDetectGeneric<T> where T : MonoBehaviour
{
    private Collider[] m_collisionCollection;
    private int m_numberOfCollisions;

    public DetectionAreaGeneric(int inNumberOfChecks)
    {
        m_collisionCollection = new Collider[inNumberOfChecks];
    }

    public T Detect(Transform inOrigin, float inRange, LayerMask inDetectionMask)
    {
        m_numberOfCollisions = Physics.OverlapSphereNonAlloc(inOrigin.transform.position, inRange, m_collisionCollection, inDetectionMask);

        for (int i = 0; i < m_numberOfCollisions; i++)
        {
            if (m_collisionCollection[i])
            {
                RaycastHit hit;

                if (Physics.Raycast(inOrigin.transform.position, m_collisionCollection[i].transform.position - inOrigin.transform.position, out hit, inRange, inDetectionMask))
                {
                    if (hit.collider.name != m_collisionCollection[i].name)
                    {
                        return null;
                    }

                    T entity = hit.transform.GetComponent<T>();

                    if (entity != null)
                    {
                        return entity;
                    }
                }
            }
        }
        return null;
    }
}
