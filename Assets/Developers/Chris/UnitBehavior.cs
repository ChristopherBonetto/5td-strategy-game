using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitBehavior : MonoBehaviour, ICanMove
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
        TakeAgentComponent();
    }
    private void Update()
    {
        Debug.Log(IsMoving());
    }

    public void AssignTroop(TroopBehavior inTroop)
    {
        m_troopRef = inTroop;
    }

    public void DeassignTroop()
    {
        m_troopRef = null;
    }


    public void TakeAgentComponent()
    {
        UnitAgent = gameObject.GetComponent<NavMeshAgent>();

        if (UnitAgent == null)
        {
            UnitAgent = gameObject.AddComponent<NavMeshAgent>();
        }
    }

    public void MoveFromTo(Vector3 endPosition)
    {
        Stop(false);
        UnitAgent.SetDestination(endPosition);
    }

    public void Stop(bool inBool)
    {
        if(UnitAgent.isStopped != inBool)
        UnitAgent.isStopped = inBool;
    }

    public bool IsMoving()
    {
        if (!UnitAgent.hasPath && UnitAgent.velocity.sqrMagnitude < 0.1f || UnitAgent.isStopped)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void Clicked()
    {

    }
}
