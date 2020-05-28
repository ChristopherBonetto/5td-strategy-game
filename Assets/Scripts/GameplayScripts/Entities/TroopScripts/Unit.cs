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

    public Unit m_focusUnit;
    public BuildingBehaviour m_focusBuilding;

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
            AssignValuesToTree(m_troopRef, 2, m_unitStats.UnitSpeed, m_unitStats.AttackSpeed);
        }
        else
        {
            m_troopRef = null;
            m_unitStats = null;
            AssignValuesToTree(null, 2, 2, 2);
        }
    }

    public void AssignFocusUnit(Unit inUnit)
    {
        StopTree(true);
        m_focusBuilding = null;

        if(inUnit != null)
        {
            m_focusUnit = inUnit;
            var focusObj = (SharedGameObject)UnitTree.GetVariable("FocusObject");
            focusObj.Value = inUnit.gameObject;
        }
        else
        {
            m_focusUnit = null;
            var focusObj = (SharedGameObject)UnitTree.GetVariable("FocusObject");
            focusObj.Value = null;
        }

        StopTree(false);
    }

    public void AssignFocusBuilding(BuildingBehaviour building)
    {
        StopTree(true);
        m_focusUnit = null;

        if (building != null)
        {
            m_focusBuilding = building;
            var focusObj = (SharedBuilding)UnitTree.GetVariable("FocusBuilding");
            focusObj.Value = building;
        }
        else
        {
            m_focusBuilding = null;
            var focusObj = (SharedBuilding)UnitTree.GetVariable("FocusBuilding");
            focusObj.Value = null;
        }

        StopTree(false);
    }

    private void AssignValuesToTree(Troop inTroop, float inAttackRange, float inMovSpeed, float inAttackSpeed)
    {
        var troopRef = (SharedTroop)UnitTree.GetVariable("TroopRef");
        troopRef.Value = inTroop;
        var movSpeed = (SharedFloat)UnitTree.GetVariable("MovSpeed");
        movSpeed.Value = inMovSpeed;
        var attackSpeed = (SharedFloat)UnitTree.GetVariable("AttackSpeed");
        attackSpeed.Value = inAttackSpeed;
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
        if (m_focusUnit)
        {
            if (TroopRef.GetStats().AttackType == AttackType.MELEE)
            {
                m_unitAttackType.SingleAttack(m_focusUnit.gameObject, m_unitStats.Damage);
            }
            else if (TroopRef.GetStats().AttackType == AttackType.RANGED)
            {
                m_unitAttackType.SingleAttack(m_focusUnit.gameObject, m_unitStats.Damage);
            }
        }
        else if (m_focusBuilding)
        {
            if (TroopRef.GetStats().AttackType == AttackType.MELEE)
            {
                m_unitAttackType.SingleAttack(m_focusBuilding.gameObject, m_unitStats.Damage);

                Debug.Log($"Damagin castle: {m_focusBuilding.CurrentHp}");
            }
            else if (TroopRef.GetStats().AttackType == AttackType.RANGED)
            {
                m_unitAttackType.SingleAttack(m_focusBuilding.gameObject, m_unitStats.Damage);
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
        if(m_focusUnit != null)
        {
            m_focusUnit.AssignFocusUnit(null);
        }
        m_focusUnit = null;
        m_focusBuilding = null;
        TroopRef.TroopTakeDamage(this);
    }



    #endregion
       
}
