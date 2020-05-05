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

    private TroopBehaviour m_troopRef;
    public TroopBehaviour TroopRef { get => m_troopRef; }


    private void Awake()
    {
        TakeAgentComponent();
    }


    #region Join Leave troop

    public void JoinTroop(TroopBehaviour inTroop)
    {
        gameObject.SetActive(true);
        gameObject.transform.parent = inTroop.transform;
        gameObject.layer = inTroop.gameObject.layer;

        m_troopRef = inTroop;
        EntityStats = inTroop.m_troopStats;

        EntityPlayerType = inTroop.EntityPlayerType;

        UnitAgent.speed = TroopRef.m_troopStats.UnitSpeed;
        RefreshHp();
    }

    public void LeaveTroop()
    {
        TroopRef.m_units.Remove(this);

        transform.parent = null;

        FocusEntity = null;

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
        UnitAgent.SetDestination(endPosition);
    }

    public void Stop(bool inBool)
    {
        if (UnitAgent.isActiveAndEnabled)
        {
            if (UnitAgent.isStopped != inBool)
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
        //if (m_unitStats.CanAttack)
        //{
        //    if (m_attackType.CanAttack(m_unitStats.AttackSpeed))
        //    {
        //        if (m_unitStats.AttackType == AttackType.MELEE)
        //        {
        //            if (m_attackType.SingleAttack(FocusEntity, m_unitStats.Damage))
        //            {
        //                if (TroopRef.currentBattle != null)
        //                {
        //                    //m_troopRef.currentBattle.TakeOtherTarget(this);
        //                }
        //            }
        //        }
        //    }
        //}
    }
}  

