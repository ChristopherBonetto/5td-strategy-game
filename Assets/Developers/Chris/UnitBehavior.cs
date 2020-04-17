using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitBehavior : MonoBehaviour
{
    private NavMeshAgent m_unitAgent;
    public NavMeshAgent UnitAgent
    {
        get
        {
            return m_unitAgent;
        }
        set
        {
            m_unitAgent = value;
        }
    }

    private TroopBehavior m_troopRef;
    public TroopBehavior TroopRef { get => m_troopRef; }

    private void Awake()
    {
        AssignAgentComponent();
    }

    public void AssignTroop(TroopBehavior inTroop)
    {
        m_troopRef = inTroop;
    }

    public void DeassignTroop()
    {
        m_troopRef = null;
    }

    public void AssignAgentComponent()
    {
        UnitAgent = gameObject.GetComponent<NavMeshAgent>();

        if (UnitAgent == null)
        {
            UnitAgent = gameObject.AddComponent<NavMeshAgent>();
        }
    }
}
