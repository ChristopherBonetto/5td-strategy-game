using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

public class Entity : MonoBehaviour
{
    private EntityStatistics m_entityStatisticsSO;
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

    public PlayerType m_EntityPlayerType { get; protected set; }

    public Actions m_CurrentUnitAction { get; protected set; }

    private float m_Timer = 0f;
    protected bool m_CanAttack = true;


    public virtual void AssignPlayer(PlayerType inPlayerType)
    {
        m_EntityPlayerType = inPlayerType;

        if(m_EntityPlayerType == PlayerType.Player)
        {
            gameObject.layer = GameController.Instance.m_playerLayer;
        }
        else if(m_EntityPlayerType == PlayerType.AI)
        {
            gameObject.layer = GameController.Instance.m_aiLayer;
        }

    }

    public virtual void AssignStats(EntityStatistics inStats)
    {
        EntityStatisticsSO = inStats;
    }


    public virtual void ChangeAction(Actions NewAction)
    {
        m_CurrentUnitAction = NewAction;
    }


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
