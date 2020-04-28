using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Types;

public class UnitBehavior : EntityBehavior, ICanMove
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

    private UnitsStatsSO m_unitStats;
    public override EntityStatsSO EntityStats
    {
        get
        {
            return m_unitStats;
        }
        set
        {
            m_unitStats = (UnitsStatsSO)value;
        }
    }

    protected bool m_isGoingToInteract = false;

    private void Awake()
    {
        TakeAgentComponent();
    }

    public void Update()
    {
        if (FocusEntity != null)
        {
            if (Timer(0.3f))
            {
                if (!CheckFocussedObjectDistance())
                {
                    m_unitAgent.SetDestination(FocusEntity.transform.position);
                }
                else
                {
                    //interact
                    Stop(true);
                    AssignInteraction(FocusEntity);
                }
            }
        }
    }

    //Controlla la distanza dall'oggetto focus.
    protected virtual bool CheckFocussedObjectDistance()
    {
        if (Vector3.Distance(transform.position, FocusEntity.transform.position) <= m_unitAgent.stoppingDistance + FocusEntity.transform.localScale.x + m_unitStats.AttackRange)
        {
            return true;
        }
        return false;
    }


    #region Join Leave troop

    public void JoinTroop(TroopBehavior inTroop)
    {
        gameObject.SetActive(true);
        gameObject.transform.parent = inTroop.transform;
        gameObject.layer = inTroop.gameObject.layer;

        m_troopRef = inTroop;
        EntityStats = inTroop.m_troopStats;

        EntityPlayerType = inTroop.EntityPlayerType;

        UnitAgent.speed = m_unitStats.UnitSpeed;
        RefreshHp();
    }

    public void LeaveTroop()
    {
        TroopRef.m_units.Remove(this);

        transform.parent = null;

        FocusEntity = null;

        m_unitStats = null;
        m_troopRef = null;
        gameObject.SetActive(false);
    }

    #endregion

    #region Move interface

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

    #endregion

    #region Click interface

    //Esegue il select della truppa
    public override void Select()
    {
        m_troopRef.Select();
    }

    //Come interagisce l'unita con un'altra.
    public override void AssignInteraction(EntityBehavior inEntity)
    {
        if (inEntity is UnitBehavior)
        {
            
            Attack();
        }
    }


    #endregion

    //Se muore chiede alla truppa cosa deve fare.
    public override void Death()
    {
        TroopRef.TroopTakeDamage(this);
    }


    //TO DO: NEW COMMAND
    public override void Attack()
    {
        if (m_unitStats.CanAttack)
        {
            if (m_attackType.CanAttack(m_unitStats.AttackSpeed))
            {
                if (m_unitStats.AttackType == AttackType.MELEE)
                {
                    if(m_attackType.SingleAttack(FocusEntity, m_unitStats.Damage))
                    {
                        //TO DO: Chiedi alla truppa un altro bersaglio del suo oggetto focus.
                        FocusEntity = null;
                    }
                }
            }
        }
    }
}
