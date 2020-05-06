using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;

public enum TroopStates
{
    Idle,
    GoToAttack,
    GoToLift
}

public class Troop : EntityBehavior
{
    private UnitsStatsSO m_troopStats;

    public override EntityBehavior FocusEntity
    {
        get
        {
            return m_focusEntity;
        }
        set
        {
            m_focusEntity = value;
            ChangeStateBasedOnFocusObj(m_focusEntity);
        }
    }

    private TroopStates m_currentTroopState = TroopStates.Idle;
    public TroopStates CurrentTroopState { get => m_currentTroopState; }

    private List<Unit> m_unitList;
    public List<Unit> UnitList
    {
        get { return m_unitList; }
        set
        {
            m_unitList = value;
            var tempList = (SharedUnitList)m_behaviorTree.GetVariable("Units");
            tempList.Value = m_unitList;
        }
    }

    private NavMeshAgent m_troopAgent;

    public override void Awake()
    {
        base.Awake();

        m_troopAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(UnitList.Count > 0)
            {
                DeassignUnitInTroop(UnitList[0]);
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SetNewTroopState(TroopStates.GoToAttack);
        }
    }

    #region States

    public void ChangeStateBasedOnFocusObj(EntityBehavior inEntity)
    {
        if(inEntity == null)
        {
            SetNewTroopState(TroopStates.Idle);
        }
        else
        {
            if (inEntity.EntityPlayerType != this.EntityPlayerType)
            {
                if (inEntity is Troop)
                {
                    SetNewTroopState(TroopStates.GoToAttack);
                }
            }
            else
            {
                //if(inEntity is Building)
            }
        }
        var focusGameObject = (SharedGameObject)m_behaviorTree.GetVariable("FocusObject");
        focusGameObject.Value = FocusEntity.gameObject;
    }
    public void SetNewTroopState(TroopStates inNewState)
    {
        m_currentTroopState = inNewState;
    }

    #endregion

    #region Stats

    public override void AssignStats(EntityStatsSO inStats)
    {
        m_troopStats = inStats as UnitsStatsSO;

        m_troopAgent.speed = m_troopStats.UnitSpeed;

        CreateUnits(m_troopStats.UnitType, m_troopStats.UnitQuantity);

        var troopRef = (SharedTroop)m_behaviorTree.GetVariable("TroopRef");
        troopRef.Value = this;
        var engageRange = (SharedFloat)m_behaviorTree.GetVariable("EngageRange");
        engageRange.Value = m_troopStats.EngageRange;
        var canAttack = (SharedBool)m_behaviorTree.GetVariable("CanAttack");
        canAttack.Value = m_troopStats.CanAttack;
        var movimentSpeed = (SharedFloat)m_behaviorTree.GetVariable("MovimentSpeed");
        movimentSpeed.Value = m_troopStats.UnitSpeed;

        m_behaviorTree.EnableBehavior();
    }

    public UnitsStatsSO GetStats()
    {
        return m_troopStats as UnitsStatsSO;
    }

    #endregion

    #region Units

    public void CreateUnits(UnitType inType, int inValue)
    {
        UnitList = new List<Unit>(inValue);

        for (int i = 0; i < inValue; i++)
        {
            GameObject tempUnit = ObjectPooler.Instance.GetUnitObject(inType);

            Unit tempRef = tempUnit.GetComponent<Unit>();

            if (tempRef == null)
            {
                Debug.Log(inType + "didn't have UnitBehavior script, pls add next time");
                return;
            }
            AssignUnitInTroop(tempRef);
        }
        //SetFormationPositions(FormationRadius);
    }

    public void AssignUnitInTroop(Unit inUnit)
    {
        UnitList.Add(inUnit);
        inUnit.gameObject.transform.parent = this.transform;
        inUnit.gameObject.layer = gameObject.layer;
        inUnit.transform.localPosition = Vector3.zero;
        inUnit.gameObject.SetActive(true);
        
        //m_troopRef = inTroop;
        //EntityStats = inTroop.m_troopStats;

        //EntityPlayerType = inTroop.EntityPlayerType;

        //UnitAgent.speed = TroopRef.m_troopStats.UnitSpeed;
        //RefreshHp();
    }

    public void DeassignUnitInTroop(Unit inUnit)
    {
        UnitList.Remove(inUnit);

        inUnit.gameObject.transform.parent = null;

        //FocusEntity = null;

        //m_troopRef = null;
        inUnit.gameObject.SetActive(false);
    }

    #endregion
}
