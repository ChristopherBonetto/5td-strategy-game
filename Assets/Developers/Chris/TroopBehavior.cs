using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class TroopBehavior : EntityBehavior, ICanMove
{
    public TroopBehavior(UnitsStatsSO inStat)
    {
        m_unitStats = inStat;
    }

    public UnitsStatsSO m_unitStats;
    private UnitBehavior[] m_troopUnits;


    private NavMeshAgent m_troopAgent;
    public NavMeshAgent TroopAgent
    {
        get
        {
            return m_troopAgent;
        }
        set
        {
            m_troopAgent = value;
        }
    }


    private void Awake()
    {
        AssignAgentComponent();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            ResetStats();
            gameObject.SetActive(false);
        }
    }

    public void AssignAgentComponent()
    {
        TroopAgent = gameObject.GetComponent<NavMeshAgent>();

        if (TroopAgent == null)
        {
            TroopAgent = gameObject.AddComponent<NavMeshAgent>();
        }
    }

    public override void AssignStats(EntityStatsSO inStats)
    {
        if(inStats is UnitsStatsSO)
        {
            base.AssignStats(inStats);
            m_unitStats = (UnitsStatsSO)inStats;
            CreateTroop(m_unitStats.Prefab, m_unitStats.TroopsQuantity);
        }
        else
        {
            Debug.LogWarning("This unit can take stats from: " + inStats.Name);
        }
    }


    public void CreateTroop(GameObject inObj, int inValue)
    {
        m_troopUnits = new UnitBehavior[inValue];

        for (int i = 0; i < inValue; i++)
        {
            GameObject tempUnit = Instantiate(inObj, Vector3.zero, Quaternion.identity, this.transform);
            UnitBehavior tempRef = tempUnit.GetComponent<UnitBehavior>();

            if(tempRef == null)
            {
                Debug.Log(inObj.name + "didn't have UnitBehavior script, pls add next time");
                tempRef = tempUnit.AddComponent<UnitBehavior>();
            }
            m_troopUnits[i] = tempRef;
            m_troopUnits[i].AssignTroop(this);
        }
    }

    public void ResetStats()
    {
        foreach(UnitBehavior unit in m_troopUnits)
        {
            Destroy(unit.gameObject);
        }
        m_troopUnits = null;

        m_unitStats = null;

        gameObject.SetActive(false);
        //Return to the pool
    }


    public void MoveFromTo(Vector3 endPosition)
    {
        //TroopAgent.destination = endPosition;

        foreach (UnitBehavior unit in m_troopUnits)
        {
            unit.UnitAgent.destination = endPosition;
        }
    }
}
