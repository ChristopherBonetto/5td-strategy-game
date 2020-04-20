using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Types;

public class UnitBehavior : MonoBehaviour, ICanMove, ITakeDamage
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

    public int m_currentUnitHp;
    public int CurrentUnitHp { get => m_currentUnitHp; }

    private GameObject m_unitFocusObject = null;
    public GameObject UnitFocusObj
    {
        get
        {
            return m_unitFocusObject;
        }
        set
        {
            m_unitFocusObject = value;
        }
    }
    private float m_Timer = 0f;
    protected bool m_canAttack = true;

    private void Awake()
    {
        TakeAgentComponent();
    }
    private void Update()
    {
        if (UnitFocusObj != null)
        {
            if (CheckFocussedObjectDistance() && m_canAttack)
            {

            }
        }
        //if (CheckFocussedObjectDistance() && m_CanAttack)
        //{
        //    Attack();
        //}

        //if (!m_CanAttack)
        //{
        //    m_CanAttack = Timer(UnitStatisticsSO.TimeToAttack);
        //}

        //if (UnitFocusObj != null)
        //{
        //    gameObject.transform.LookAt(new Vector3(UnitFocusObj.transform.position.x, gameObject.transform.position.y, UnitFocusObj.transform.position.z));
        //}
    }

    //public virtual void Attack()
    //{
    //    ChangeUnitState(Actions.Attack);

    //    CanTakeDamage = FocusObject.GetComponent<IDamageable>() as IDamageable;
    //    if (CanTakeDamage != null)
    //    {
    //        CanTakeDamage.TakeDamage(UnitStatisticsSO.Attack);
    //        m_CanAttack = false;
    //    }
    //}

    protected virtual bool CheckFocussedObjectDistance()
    {
        if (Vector3.Distance(transform.position, UnitFocusObj.transform.position) <= m_unitAgent.stoppingDistance + UnitFocusObj.transform.localScale.x + m_unitStats.AttackRange)
        {
            if (UnitAgent.velocity.sqrMagnitude == 0)
            {
                return true;
            }
        }
        return false;
    }

    public virtual void RefreshToMaxHp()
    {
        m_currentUnitHp = m_unitStats.MaxHp;
    }

    //public virtual bool TakeDamage(int Damage)
    //{
    //    Damage = Mathf.Clamp(Damage, 0, UnitStatisticsSO.HealthMax + UnitStatisticsSO.Defence);

    //    if (m_UnitCurrentHp <= Damage)
    //    {
    //        m_UnitCurrentHp -= Damage;
    //        UIManager.Instance.DeactivateAllPanels();
    //        Death();
    //        return true;
    //    }
    //    else
    //    {
    //        m_UnitCurrentHp -= Damage;
    //        if (gameObject == MouseSelectionManager.Instance.CurrentSelectedObject)
    //        {
    //            ShowInfoPanels();
    //        }
    //        return false;
    //    }
    //}

    //public virtual void Death()
    //{
    //    if (gameObject == MouseSelectionManager.Instance.CurrentSelectedObject)
    //    {
    //        MouseSelectionManager.Instance.ClearSelection();
    //    }
    //    CivilizationScriptBehaviour.Instance.AddRemoveCivilizationValue(-UnitStatisticsSO.PopolationsValue);
    //    UIManager.Instance.RefreshPopulation();
    //    Destroy(this.gameObject);
    //}

    public virtual bool Timer(float destinationTime)
    {
        m_Timer += Time.deltaTime;

        if (m_Timer >= destinationTime)
        {
            m_Timer = 0f;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TakeDamage(int Damage)
    {
        return true;
    }

    public void Death()
    {
    }

    #region Join Leave troop

    public void JoinTroop(TroopBehavior inTroop)
    {
        m_troopRef = inTroop;
        m_unitStats = inTroop.m_troopStats;

        UnitAgent.speed = m_unitStats.UnitSpeed;
        RefreshToMaxHp();
    }

    public void LeaveTroop()
    {
        m_unitStats = null;
        m_troopRef = null;
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

    public void Select()
    {
        m_troopRef.Select();
    }

    public void Interact(EntityBehavior inEntity)
    {
    }

    #endregion
}
