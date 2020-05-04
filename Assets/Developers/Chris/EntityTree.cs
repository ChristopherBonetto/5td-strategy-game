using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using BehaviorDesigner.Runtime;
using System;

public class EntityTree : MonoBehaviour
{
    public EntityStatsSO m_entityStats;

    public PlayerType EntityPlayerType;

    protected float m_timer = 0f;

    public BehaviorTree BT;

    public void Awake()
    {
        BT = gameObject.GetComponent<BehaviorTree>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ResetEntity();
        }
    }

    private void ResetEntity()
    {
        m_entityStats = null;
        BT.ExternalBehavior = null;
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
        BT.ExternalBehavior = inStats.ExTree;
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
