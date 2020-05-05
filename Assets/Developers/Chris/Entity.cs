using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using BehaviorDesigner.Runtime;
using System;

public class Entity : MonoBehaviour
{
    private EntityStatsSO m_entityStats;
    public virtual EntityStatsSO EntityStats
    {
        get
        {
            return m_entityStats;
        }
        set
        {
            m_entityStats = value;
        }
    }

    public PlayerType EntityPlayerType;

    protected float m_timer = 0f;

    protected Entity m_focusEntity;
    public virtual Entity FocusEntity
    {
        get { return m_focusEntity; }
        set { m_focusEntity = value; }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ResetEntity();
        }
        
    }

    protected void ResetEntity()
    {
        m_entityStats = null;
        //BT.ExternalBehavior = null;
        gameObject.name = "Entity";
        gameObject.SetActive(false);
    }

    public virtual void AssignPlayer(PlayerType inPlayerType)
    {
        EntityPlayerType = inPlayerType;

        if (EntityPlayerType == PlayerType.Player)
        {
            gameObject.layer = GameController.Instance.m_playerLayer;
        }
        else if (EntityPlayerType == PlayerType.AI)
        {
            gameObject.layer = GameController.Instance.m_aiLayer;
        }
    }

    public virtual void AssignStats(EntityStatsSO inStats)
    {
        gameObject.name = inStats.Name + "Troops";
        EntityStats = inStats;
    }

    public virtual bool Timer(float destinationTime)
    {
        m_timer += Time.deltaTime;

        if (m_timer >= destinationTime)
        {
            m_timer = 0f;
            return true;
        }
        else
        {
            return false;
        }
    }


}
