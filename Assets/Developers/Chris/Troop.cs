using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;
using HF.Unit;

public enum TroopStates
{
    Idle,
    GoToTroop,
    GoToBuilding,
    GoToDestination
}

public class Troop : EntityBehavior, ICanMove
{
    private UnitsStatsSO m_troopStats;

    private TroopStates m_currentTroopState = TroopStates.Idle;
    public TroopStates CurrentTroopState { get { return m_currentTroopState; } }

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

    private Vector3 m_destination;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(UnitList.Count > 0)
            {
                DeassignUnitInTroop(UnitList[0]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetNewTroopState(TroopStates.Idle);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetNewTroopState(TroopStates.GoToTroop);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetNewTroopState(TroopStates.GoToBuilding);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetNewTroopState(TroopStates.GoToDestination);
        }
    }

    #region States

    public void SetNewTroopState(TroopStates inNewState)
    {
        m_currentTroopState = inNewState;
    }

    #endregion

    #region Stats

    public override void AssignStats(EntityStatsSO inStats)
    {
        m_troopStats = inStats as UnitsStatsSO;

        CreateUnits(m_troopStats.UnitType, m_troopStats.UnitQuantity);

        var troopRef = (SharedTroop)m_behaviorTree.GetVariable("TroopRef");
        troopRef.Value = this;
        var engageRange = (SharedFloat)m_behaviorTree.GetVariable("EngageRange");
        engageRange.Value = m_troopStats.EngageRange;
        var canAttack = (SharedBool)m_behaviorTree.GetVariable("CanAttack");
        canAttack.Value = m_troopStats.CanAttack;
        var movimentSpeed = (SharedFloat)m_behaviorTree.GetVariable("MovimentSpeed");
        movimentSpeed.Value = m_troopStats.UnitSpeed;

        m_behaviorTree.enabled = true;
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

    #region Commands

    public void MoveFromTo(Vector3 endPosition)
    {
        m_behaviorTree.enabled = false;

        m_focusEntity = null;
        m_destination = endPosition;
        var destination = (SharedVector3)m_behaviorTree.GetVariable("Destination");
        destination.Value = m_destination;
        SetNewTroopState(TroopStates.GoToDestination);

        m_behaviorTree.enabled = true;
    }

    public override void AssignFocusEntity(EntityBehavior inEntity)
    {
        m_behaviorTree.enabled = false;

        if (inEntity == null)
        {
            SetNewTroopState(TroopStates.Idle);

            var focusGameObject = (SharedGameObject)m_behaviorTree.GetVariable("FocusObject");
            focusGameObject.Value = null;
        }
        else
        {
            if (inEntity.EntityPlayerType != this.EntityPlayerType)
            {
                if (inEntity is Troop)
                {
                    m_focusEntity = inEntity;
                    SetNewTroopState(TroopStates.GoToTroop);
                }
            }
            else
            {
                if(inEntity is BuildingBehaviour)
                {
                    m_focusEntity = inEntity;
                    SetNewTroopState(TroopStates.GoToBuilding);
                }
            }
            var focusGameObject = (SharedGameObject)m_behaviorTree.GetVariable("FocusObject");
            focusGameObject.Value = FocusEntity.gameObject;

            
        }

        m_behaviorTree.enabled = true;
    }

    #endregion
}
