using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour
{
    protected EntityStatistics m_entityStatisticsSO;
    public EntityStatistics EntityStatisticsSO
    {
        get
        {
            return m_entityStatisticsSO;
        }
        set
        {
            m_entityStatisticsSO = value;
        }
    }

    protected float m_Timer = 0f;

        

    public virtual bool Timer(float destinationTime)
    {
        m_Timer += Time.deltaTime;

        if (m_Timer >= destinationTime)
        {
            m_Timer = 0f;
            return true;
        }
        else
        {
            return false;
        }
    }

    
}
