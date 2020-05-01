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

    private void Awake()
    {
        TakeAgentComponent();
    }

    public void Update()
    {
        if (UnitAgent.isActiveAndEnabled && FocusEntity != null)
        {
            if (!CheckFocussedObjectDistance())
            {
                m_unitAgent.SetDestination(FocusEntity.transform.position);
            }
            else
            {
                Interact();
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
        if (!UnitAgent.isActiveAndEnabled) return;
        
        Stop(false);
        UnitAgent.SetDestination(endPosition);
    }

    public void Stop(bool inBool)
    {
        if (UnitAgent.isActiveAndEnabled)
        {
            if(UnitAgent.isStopped != inBool)
                UnitAgent.isStopped = inBool;
        }
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
    public override void Click()
    {
        m_troopRef.Click();
    }

    public void Interact()
    {
        if (FocusEntity is UnitBehavior)
        {
            Attack();
        }
    }

    //Come interagisce l'unita con un'altra.
    public override void AssignFocusEntity(EntityBehavior inEntity)
    {
        
    }


    #endregion

    public override void UnlockEntity()
    {
        base.UnlockEntity();
        FocusEntity = null;
        Stop(false);
    }

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
                        if(TroopRef.currentBattle != null)
                        {
                            m_troopRef.currentBattle.TakeOtherTarget(this);
                        }
                    }
                }
            }
        }
    }
}
