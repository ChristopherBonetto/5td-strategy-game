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

    private UnitsStatsSO m_unitStats;

    private int m_unitHp;
    public int UnitHp { get => m_unitHp; }

    private IAttackTypes m_unitAttackType;

    public Unit m_focusUnit;
    public BuildingBehaviour m_focusBuilding;

    public GameObject visualObj;

    #region Events
    private void OnEnable()
    {
        HFEventManager.SubscribeTo<bool>(HFEventID.OnPauseMode, FreezeMode);
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, GameStateChanged);
    }
    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<bool>(HFEventID.OnPauseMode, FreezeMode);
        HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, GameStateChanged);
    }
    #endregion

    private void Awake()
    {
        Initialize();
    }

    protected virtual void FreezeMode(bool inValue)
    {
        StopTree(inValue);
    }

    protected virtual void GameStateChanged(GameStates inState)
    {
        if ((inState == GameStates.EndLevel || inState == GameStates.WarRoom))
        {
        }
        else if (inState == GameStates.Pause)
        {
            if(m_troopRef.m_currentBattle != null)
            {
                StopTree(true);
            }
        }
        else if (inState == GameStates.PlayingLevel)
        {
            if (m_troopRef.m_currentBattle != null)
            {
                StopTree(false);
            }
        }
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

    public void AssignFocusUnit(Unit inUnit)
    {
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
    }

    public void AssignFocusBuilding(BuildingBehaviour building)
    {
        m_focusBuilding = null;

        if (building != null)
        {
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
            if (m_troopRef.GetStats().AttackType == AttackType.MELEE)
            {
                m_unitAttackType.SingleAttack(m_focusUnit, m_unitStats.Damage);
            }
            else if (m_troopRef.GetStats().AttackType == AttackType.RANGED)
            {
                m_unitAttackType.SingleAttack(m_focusUnit, m_unitStats.Damage);
            }
        }
        else if (m_focusBuilding)
        {
            if (m_troopRef.GetStats().AttackType == AttackType.MELEE)
            {
                m_unitAttackType.SingleAttack(m_focusBuilding, m_unitStats.Damage);
                Debug.Log($"Damagin castle: {m_focusBuilding.CurrentHp}");
            }
            else if (m_troopRef.GetStats().AttackType == AttackType.RANGED)
            {
                m_unitAttackType.SingleAttack(m_focusBuilding, m_unitStats.Damage);
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
