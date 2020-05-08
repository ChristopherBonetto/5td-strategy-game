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

    public List<Unit> m_unitList;
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
    [SerializeField] private float m_formationRadius;
    private Vector3[] m_formationPosition = new Vector3[4];

    private Vector3 m_destination;

    public BattleHandler m_currentBattle = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetIdleState();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DismissUnitInTroop(UnitList[0]);
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
        SetFormationPositions(m_formationRadius);
    }

    public void AssignUnitInTroop(Unit inUnit)
    {
        UnitList.Add(inUnit);
        inUnit.StopTree(true);
        inUnit.gameObject.transform.parent = this.transform;
        inUnit.gameObject.layer = gameObject.layer;
        inUnit.transform.localPosition = Vector3.zero;
        inUnit.gameObject.SetActive(true);
        inUnit.AssignUnitInTroop(this);
        
        inUnit.RefreshHp();
    }

    public void DismissUnitInTroop(Unit inUnit)
    {
        inUnit.StopTree(true);

        UnitList.Remove(inUnit);
        inUnit.AssignUnitInTroop(null);

        inUnit.gameObject.transform.parent = null;
        inUnit.gameObject.SetActive(false);
    }

    #region Units Formation

    public void SetFormationPositions(float inRadius = 1)
    {
        if (m_troopStats == null || UnitList.Count == 0)
        {
            return;
        }
        // Begin Modification @Panta
        // Here we store each offset position.
        // Note the case 2 and 4 are different cause of angle offset.
        // In case 2 the offset is -90 degree, while in case 4 is -45 degree.
        switch (UnitList.Count)
        {
            case 1:
                m_formationPosition[0] = Vector3.zero;
                break;

            case 2:
                // Reassign value to each position.
                for (int i = 0; i < UnitList.Count; i++)
                {
                    // Calculate the angle in radian (not degree)
                    float angle = Mathf.PI * 2 / UnitList.Count * i - (90 * Mathf.Deg2Rad);
                    angle += transform.eulerAngles.y * Mathf.Deg2Rad;

                    m_formationPosition[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * inRadius;
                }
                break;

            case 4:
                // Reassign value to each position.
                for (int i = 0; i < UnitList.Count; i++)
                {
                    // Calculate the angle in radian (not degree)
                    float angle = Mathf.PI * 2 / UnitList.Count * i - (45 * Mathf.Deg2Rad);
                    angle += transform.eulerAngles.y * Mathf.Deg2Rad;

                    m_formationPosition[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * inRadius;
                }
                break;

            default:
                // Reassign value to each position.
                for (int i = 0; i < UnitList.Count; i++)
                {
                    // Calculate the angle in radian (not degree)
                    float angle = Mathf.PI * 2 / UnitList.Count * i;
                    angle += transform.eulerAngles.y * Mathf.Deg2Rad;

                    m_formationPosition[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * inRadius;
                }
                break;
        }
        // End modification @Panta
        AssignFormation(m_formationPosition);
    }

    public void AssignFormation(Vector3[] inPos)
    {
        for (int i = 0; i < UnitList.Count; i++)
        {
            UnitList[i].UnitAgent.Warp(transform.position + inPos[i]);
        }
    }

    public void ResetFormation()
    {
        for (int i = 0; i < UnitList.Count; i++)
        {
            if (UnitList[i].gameObject.active)
            {
                UnitList[i].UnitAgent.isStopped = false;
                Vector3 destination = transform.position + m_formationPosition[i];
                UnitList[i].UnitAgent.destination = destination;
            }
        }
    }

    #endregion


    #endregion

    #region Commands

    public void MoveFromTo(Vector3 endPosition)
    {
        if(m_currentBattle != null)
        {
            m_currentBattle.EscapeFight();
        }

        m_behaviorTree.enabled = false;

        m_focusEntity = null;
        var focusEntity = (SharedGameObject)m_behaviorTree.GetVariable("FocusObject");
        focusEntity.Value = null;
        m_destination = endPosition;
        var destination = (SharedVector3)m_behaviorTree.GetVariable("Destination");
        destination.Value = m_destination;
        SetNewTroopState(TroopStates.GoToDestination);

        m_behaviorTree.enabled = true;
    }

    public override void AssignFocusEntity(EntityBehavior inEntity)
    {
        m_behaviorTree.enabled = false;
        m_focusEntity = null;

        if (inEntity.EntityPlayerType != this.EntityPlayerType && !inEntity.IsBusy)
        {
            if (inEntity is Troop)
            {
                Troop enemyTroop = inEntity as Troop;

                if (enemyTroop.GetStats().CanTakeDamage && m_troopStats.CanAttack)
                {
                    m_focusEntity = enemyTroop;
                    SetNewTroopState(TroopStates.GoToTroop);
                    Debug.Log(gameObject.name + " GO TO ATTACK : " + enemyTroop.name);
                }
            }
        }
        else
        {
            if (inEntity is BuildingBehaviour && !inEntity.IsBusy)
            {
                m_focusEntity = inEntity;
                SetNewTroopState(TroopStates.GoToBuilding);
                Debug.Log(gameObject.name + " GO TO : " + m_focusEntity.name);
            }
        }

        if (FocusEntity != null)
        {
            var focusEntity = (SharedGameObject)m_behaviorTree.GetVariable("FocusObject");
            focusEntity.Value = FocusEntity.gameObject;
        }
        else
        {
            SetIdleState();
        }
        m_behaviorTree.enabled = true;
    }

    #endregion

    public override void Attack()
    {
        new Fight(this, FocusEntity as Troop);
    }

    public void SetIdleState()
    {
        m_behaviorTree.enabled = false;

        SetNewTroopState(TroopStates.Idle);
        m_focusEntity = null;
        var focusEntity = (SharedGameObject)m_behaviorTree.GetVariable("FocusObject");
        focusEntity.Value = null;
        IsBusy = false;

        foreach(Unit unit in UnitList)
        {
            unit.StopTree(true);
            unit.AssignFocusUnit(null);
        }

        m_currentBattle = null;

        ResetFormation();

        m_behaviorTree.enabled = true;
    }

    public void DismissTroop()
    {
        foreach (Unit unit in UnitList)
        {
            DismissUnitInTroop(unit);
        }
        UnitList = null;

        m_troopStats = null;

        m_behaviorTree.enabled = false;
        gameObject.SetActive(false);
        //Return to the pool
    }

    #region Troop Hp

    public void TroopTakeDamage(Unit inUnit)
    {
        DismissUnitInTroop(inUnit);

        if (CurrentHp == 0)
        {
            if (EntityPlayerType == PlayerType.AI)
            {
                Death();
                return;
            }
            else
            {
                if (InputReaderManager.Instance.CurrentEntity == this)
                {
                    InputReaderManager.Instance.CurrentEntity = null;
                }
                StartCoroutine(Respawn());
            }
        }
    }

    public override void Death()
    {
        SetIdleState();
        StopTree(true);
        gameObject.SetActive(false);
    }

    //Respawna le unita dopo un timer
    IEnumerator Respawn()
    {
        transform.position = new Vector3(0, 0.5f, 0);
        yield return new WaitForSeconds(m_troopStats.RespawnTime);
        CreateUnits(m_troopStats.UnitType, m_troopStats.UnitQuantity);
    }

    #endregion
}
