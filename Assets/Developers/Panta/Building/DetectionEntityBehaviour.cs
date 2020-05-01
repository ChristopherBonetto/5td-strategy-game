using UnityEngine;
using System.Collections;

public class DetectionEntityBehaviour<T> : IDetectGeneric<T> where T : EntityBehavior
{
    private Collider[] m_collisionCollection;
    private int m_numberOfCollisions;

    public DetectionEntityBehaviour(int inNumberOfChecks)
    {
        m_collisionCollection = new Collider[inNumberOfChecks];
    }


    /// <summary>
    /// Generic detection
    /// </summary>
    public T Detect(Transform inOrigin, float inRange, LayerMask inDetectionMask)
    {
        // Initialize variables.
        T entity = null;


        // Begin checks.
        m_numberOfCollisions = Physics.OverlapSphereNonAlloc(inOrigin.transform.position, inRange, m_collisionCollection, inDetectionMask);

        for (int i = 0; i < m_numberOfCollisions; i++)
        {
            if (m_collisionCollection[i])
            {
                if (Physics.Raycast(inOrigin.transform.position, m_collisionCollection[i].transform.position - inOrigin.transform.position, inRange, inDetectionMask))
                {
                    entity = m_collisionCollection[i].GetComponent<T>();

                    if (entity != null)
                    {
                        return entity;
                    }
                }
            }
        }

        return entity;
    }


    /// <summary>
    /// Detection with team compare.
    /// </summary>
    public EntityBehavior DetectFaction(Transform inOrigin, float inRange, LayerMask inDetectionMask, int myTeam)
    {
        EntityBehavior entity = Detect(inOrigin, inRange, inDetectionMask);

        if (entity.gameObject.layer != myTeam)
        {
            return entity;
        }

        return null;
    }
}
