using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Types;

public class TroopBehavior : EntityBehavior, ICanMove, ITakeUpgrade
{
    public TroopBehavior(UnitsStatsSO inStat)
    {
        m_unitStats = inStat;
    }

    public UnitsStatsSO m_unitStats;
    public UnitBehavior[] m_troopUnits;

    private Vector3[] m_formationPosition = new Vector3[4];

    public NavMeshAgent Agent;

    private int Xsize;
    private int Zsize;

    private bool m_agentInMovement;
    public bool AgentInMovement
    {
        get
        {
            return m_agentInMovement;
        }
        set
        {
            if(m_agentInMovement == true)
            {
                if(value == false)
                {
                    Stop(true);
                }
            }
            m_agentInMovement = value;
        }
    }


    private void Awake()
    {
        TakeAgentComponent();
    }

    private void Update()
    {
        AgentInMovement = IsMoving();
    }


    
    public override void AssignStats(EntityStatsSO inStats)
    {
        if(inStats is UnitsStatsSO)
        {
            base.AssignStats(inStats);
            m_unitStats = (UnitsStatsSO)inStats;
            CreateUnit(m_unitStats.UnitType, m_unitStats.TroopsQuantity);
        }
        else
        {
            Debug.LogWarning("This unit can take stats from: " + inStats.Name);
        }
    }


    public void CreateUnit(UnitType inType, int inValue)
    {
        m_troopUnits = new UnitBehavior[inValue];

        for (int i = 0; i < m_troopUnits.Length; i++)
        {
            GameObject tempUnit = ObjectPooler.SharedInstance.GetUnityObject(inType);

            UnitBehavior tempRef = tempUnit.GetComponent<UnitBehavior>();

            if(tempRef == null)
            {
                Debug.Log(inType + "didn't have UnitBehavior script, pls add next time");
                return;
            }
            m_troopUnits[i] = AssignUnit(tempRef);
        }
        CreateSquareFormation(1f);
    }

    public void ResetStats()
    {
        foreach(UnitBehavior unit in m_troopUnits)
        {
            DeassignUnit(unit);
        }
        m_troopUnits = null;

        m_unitStats = null;

        gameObject.SetActive(false);
        //Return to the pool
    }

    public UnitBehavior AssignUnit(UnitBehavior inUnit)
    {
        inUnit.gameObject.SetActive(true);
        inUnit.AssignTroop(this);
        inUnit.gameObject.transform.parent = this.transform;
        inUnit.gameObject.layer = gameObject.layer;
        return inUnit;
    }

    public void DeassignUnit(UnitBehavior inUnit)
    {
        inUnit.transform.parent = null;
        inUnit.gameObject.SetActive(false);
    }

    public void CreateSquareFormation(float inOffset = 1)
    {
        if(m_unitStats == null || m_troopUnits.Length == 0)
        {
            return;
        }

        Xsize = Mathf.RoundToInt(m_troopUnits[0].transform.localScale.x);
        Zsize = Mathf.RoundToInt(m_troopUnits[0].transform.localScale.z);
        m_formationPosition = new Vector3[4];

        switch (m_troopUnits.Length)
        {
            case 1:
                m_formationPosition[0] = new Vector3(transform.position.x, transform.position.y, inOffset + Zsize / 2);
                break;

            case 2:
                m_formationPosition[0] = new Vector3(-inOffset - Xsize / 2, transform.position.y, transform.position.z);
                m_formationPosition[1] = new Vector3(inOffset + Xsize / 2, transform.position.y, transform.position.z);
                break;

            case 3:
                m_formationPosition[0] = new Vector3(-inOffset - Xsize / 2, transform.position.y, inOffset + Zsize / 2);
                m_formationPosition[1] = new Vector3(inOffset + Xsize / 2, transform.position.y, inOffset + Zsize / 2);
                m_formationPosition[2] = new Vector3(transform.position.x, transform.position.y, -inOffset - Zsize / 2);
                break;

            case 4:
                m_formationPosition[0] = new Vector3(-inOffset - Xsize / 2, transform.position.y, inOffset + Zsize / 2);
                m_formationPosition[1] = new Vector3(inOffset + Xsize / 2, transform.position.y, inOffset + Zsize / 2);
                m_formationPosition[2] = new Vector3(-inOffset - Xsize / 2, transform.position.y, -inOffset - Zsize / 2);
                m_formationPosition[3] = new Vector3(inOffset + Xsize / 2, transform.position.y, -inOffset - Zsize / 2);
                break;

        }
        AssignFormation(m_formationPosition);
    }

    public void CreateTriangleFormation(float inOffSet)
    {

    }

    public void AssignFormation(Vector3[] inPos)
    {
        for(int i = 0; i < m_troopUnits.Length; i++)
        {
            m_troopUnits[i].transform.localPosition = inPos[i];
        }
    }




    public void MoveFromTo(Vector3 endPosition)
    {
        if (Agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.Log("Invalid destination");
            return;
        }

        Stop(false);
        Agent.SetDestination(endPosition);
        
        for(int i = 0; i < m_troopUnits.Length; i++)
        {
            m_troopUnits[i].MoveFromTo(endPosition + m_formationPosition[i]);
        }
    }

    public bool IsMoving()
    {
        if(!Agent.hasPath && Agent.velocity.sqrMagnitude < 0.1f || Agent.isStopped)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void Stop(bool inBool)
    {
        if(Agent.isStopped != inBool)
        {
            Agent.isStopped = inBool;

            for (int i = 0; i < m_troopUnits.Length; i++)
            {
                m_troopUnits[i].UnitAgent.isStopped = inBool;
            }
        }
    }

    public void TakeAgentComponent()
    {
        Agent = gameObject.GetComponent<NavMeshAgent>();

        if (Agent == null)
        {
            Agent = gameObject.AddComponent<NavMeshAgent>();
        }
    }

    public override void Clicked()
    {
        base.Clicked();
    }

    
}
