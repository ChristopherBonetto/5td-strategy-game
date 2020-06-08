using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using Types;
using BehaviorDesigner.Runtime.Tasks;

public class Unit : MonoBehaviour, ITakeDamage
{
    private NavMeshAgent m_unitAgent;
    public NavMeshAgent UnitAgent { get => m_unitAgent; }

    private BehaviorTree m_unitTree;
    public BehaviorTree UnitTree { get => m_unitTree; }

    [SerializeField] private Troop m_troopRef;
    public Troop TroopRef { get => m_troopRef; }

    private int m_unitHp;
    public int UnitHp { get => m_unitHp; }

    private IAttackTypes m_unitAttackType;

    public Unit m_focusUnit;
    public BuildingBehaviour m_focusBuilding;

    public GameObject visualObj;

    private void Awake()
    {
        Initialize();
    }

    #region Initialize

    private void Initialize()
    {
        m_unitAgent = gameObject.GetComponent<NavMeshAgent>();

        m_unitTree = gameObject.GetComponent<BehaviorTree>();

        m_unitAttackType = new AttackBehaviors();

        var unitRef = (SharedUnit)UnitTree.GetVariable("UnitRef");
        unitRef.Value = this;

        StopTree(true);
    }

    #endregion

    #region Assignments

    public void AssignFocusUnit(Unit inUnit)
    {
        m_focusBuilding = null;

        if(inUnit != null)
        {
            AssignValuesToTree();
            m_focusUnit = inUnit;
            UnitTree.SetVariableValue("FocusObject", m_focusUnit.gameObject);
        }
        else
        {
            m_focusUnit = null;
            UnitTree.SetVariableValue("FocusObject", null);
        }
    }

    public void AssignFocusBuilding(BuildingBehaviour building)
    {
        m_focusBuilding = null;

        if (building != null)
        {
            AssignValuesToTree();
            m_focusBuilding = building;
            var focusObj = (SharedGameObject)UnitTree.GetVariable("FocusObject");
            focusObj.Value = building.gameObject;
        }
        else
        {
            m_focusUnit = null;
            var focusObj = (SharedGameObject)UnitTree.GetVariable("FocusObject");
            focusObj.Value = null;
        }
    }

    private void AssignValuesToTree()
    {
        UnitTree.SetVariableValue("UnitRef", this);
        UnitTree.SetVariableValue("TroopRef", m_troopRef);
        UnitTree.SetVariableValue("MovSpeed", m_troopRef.GetStats().UnitSpeed);
        UnitTree.SetVariableValue("AttackSpeed", m_troopRef.GetStats().AttackSpeed);
        UnitTree.SetVariableValue("AttackRange", (float)m_troopRef.GetStats().AttackRange);
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
            if (m_troopRef.GetStats().AttackType == AttackType.MELEE)
            {
                m_unitAttackType.SingleAttack(m_focusUnit, TroopRef.GetStats().Damage);
            }
            else if (m_troopRef.GetStats().AttackType == AttackType.RANGED)
            {
                m_unitAttackType.SingleAttack(m_focusUnit, TroopRef.GetStats().Damage);
            }
        }
        else if (m_focusBuilding)
        {
            if (m_troopRef.GetStats().AttackType == AttackType.MELEE)
            {
                m_unitAttackType.SingleAttack(m_focusBuilding, TroopRef.GetStats().Damage);
            }
            else if (m_troopRef.GetStats().AttackType == AttackType.RANGED)
            {
                m_unitAttackType.SingleAttack(m_focusBuilding, TroopRef.GetStats().Damage);
            }
        }
    }

    public void CheckAnotherTarget()
    {
        if (m_troopRef.m_currentBattle != null)
        {
            m_troopRef.m_currentBattle.TakeOtherTarget(this);
        }
    }

    #endregion

    #region Health

    public void RefreshHp()
    {
        m_unitHp = m_troopRef.GetStats().MaxHp;
    }

    public bool TakeDamage(int Damage)
    {
        if (m_troopRef == null) return true;

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
        AssignFocusUnit(null);
        AssignFocusBuilding(null);

        if(m_troopRef.EntityPlayerType == PlayerType.AI)
        {
            GameObject gem = ObjectPooler.Instance.GetPooledObject("Gem");
            gem.transform.position = this.transform.position;
            gem.SetActive(true);
        }

        m_troopRef.TroopTakeDamage(this);
    }



    #endregion
       
}
