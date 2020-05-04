using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : Entity
{
    public UnitsStatsSO m_troopStats;
    public override EntityStatsSO EntityStats
    {
        get { return m_troopStats; }
        set { m_troopStats = (UnitsStatsSO)value; }
    }

    public NavMeshAgent UnitAgent;

    private void Awake()
    {
        UnitAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    public override void AssignStats(EntityStatsSO inStats)
    {
        base.AssignStats(inStats);
    }
}
