using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using Types;

public class Unit : MonoBehaviour, ITakeDamage
{
    private NavMeshAgent m_unitAgent;
    public NavMeshAgent UnitAgent { get => m_unitAgent; }

    private BehaviorTree m_unitTree;
    public BehaviorTree UnitTree { get => m_unitTree; }

    private Troop m_troopRef;
    public Troop TroopRef { get => m_troopRef; }

    private UnitsStatsSO m_unitStats;

    private int m_unitHp;
    public int UnitHp { get => m_unitHp; }

    private IAttackTypes m_unitAttackType;

    private Unit m_focusUnit;

    private void Awake()
    {
        Initialize();
    }

    #region Initialize

    private void Initialize()
    {
        m_unitAgent = gameObject.GetComponent<NavMeshAgent>();

        m_unitTree = gameObject.GetComponent<BehaviorTree>();
        StopTree(true);

        m_unitAttackType = new AttackBehaviors();

        var unitRef = (SharedUnit)UnitTree.GetVariable("UnitRef");
        unitRef.Value = this;
    }

    #endregion

    #region Assignments

    public void AssignUnitInTroop(Troop inValue)
    {
        if(inValue != null)
        {
            m_troopRef = inValue;
            m_unitStats = m_troopRef.GetStats();
            AssignValuesToTree(2, m_unitStats.UnitSpeed);
        }
        else
        {
            m_troopRef = null;
            m_unitStats = null;
            AssignValuesToTree(2, 2);
        }
    }

    public void AssignFocusUnit(Unit inUnit)
    {
        m_focusUnit = inUnit;
        
        if(m_focusUnit != null)
        {
            var unitRef = (SharedGameObject)UnitTree.GetVariable("FocusTarget");
            unitRef.Value = m_focusUnit.gameObject;
        }
        else
        {
            var unitRef = (SharedGameObject)UnitTree.GetVariable("FocusTarget");
            unitRef.Value = null;
        }
    }

    private void AssignValuesToTree(float inAttackRange, float inMovSpeed)
    {
        var movSpeed = (SharedFloat)UnitTree.GetVariable("MovSpeed");
        movSpeed.Value = inMovSpeed;
        var attackRange = (SharedFloat)UnitTree.GetVariable("AttackRange");
        attackRange.Value = inAttackRange;
    }

    #endregion

    #region Behavior Tree

    public void StopTree(bool inValue)
    {
        m_unitTree.enabled = !inValue;
    }

    #endregion

    #region Attack

    public void UnitAttack()
    {
        if (m_unitAttackType.CanAttack(TroopRef.GetStats().AttackSpeed))
        {
            if (TroopRef.GetStats().AttackType == AttackType.MELEE)
            {
                if (m_unitAttackType.SingleAttack(m_focusUnit.gameObject, m_unitStats.Damage))
                {
                    AssignFocusUnit(null);
                }
            }
        }
    }

    public void CheckAnotherTarget()
    {
        if (TroopRef.m_currentBattle != null)
        {
            TroopRef.m_currentBattle.TakeOtherTarget(this);
        }
    }

    #endregion

    #region Health

    public void RefreshHp()
    {
        m_unitHp = TroopRef.GetStats().MaxHp;
    }

    public bool TakeDamage(int Damage)
    {
        Damage = Mathf.Clamp(Damage, 1, UnitHp + m_troopRef.GetStats().Armor);

        if (UnitHp <= Damage)
        {
            Death();
            return true;
        }
        else
        {
            m_unitHp -= Damage;
            return false;

        }
    }

    public void Death()
    {
        TroopRef.TroopTakeDamage(this);
    }

    #endregion
}
