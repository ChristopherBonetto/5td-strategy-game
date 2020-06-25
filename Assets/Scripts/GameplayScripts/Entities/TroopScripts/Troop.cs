using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;
using DG.Tweening;
using System.Linq;
using UnityEditor;
using Unity.Collections.LowLevel.Unsafe;

public enum TroopStates
{
    Idle,
    GoToEnemy,
    GoToAlly,
    GoToDestination
}

[RequireComponent(typeof(NavMeshAgent))]
public class Troop : EntityBehavior, ICanMove
{
    public NavMeshAgent Agent;
    public UnitsStatsSO m_troopStats;
    public override EntityStatsSO EntityStats
    {
        get { return m_troopStats; }
        set { m_troopStats = (UnitsStatsSO)value; }
    }
    public override int CurrentHp
    {
        get { return TakeTroopHealth(); }
    }
    private float m_currentCarryCapacity;
    public float CurrentCarryCapacity
    {
        get
        {
            return TakeTroopCarryCapacity();
        }
    }

    public TroopStates m_currentTroopState = TroopStates.Idle;
    public TroopStates CurrentTroopState { get { return m_currentTroopState; } }

    [SerializeField] private List<Unit> m_unitList;
    public List<Unit> UnitList
    {
        get { return m_unitList; }
        set { m_unitList = value; }
    }

    public Queue<Unit> DeathUnit = new Queue<Unit>();

    public Unit AliveUnit
    {
        get
        {
            Unit unit = null;
            foreach (Unit unit1 in UnitList)
            {
                if (unit1.UnitHp > 0)
                {
                    unit = unit1;
                    break;
                }
            }
            return unit;
        }
    }

    [SerializeField] private float m_formationRadius;
    public Vector3[] m_formationPosition { get; private set; } = new Vector3[4];

    private Vector3 m_destination;

    public BattleHandler m_currentBattle = null;
    private bool m_canEscape = true;

    private BuildingBehaviour m_buildingHandled;
    public BuildingBehaviour BuildingHandled { get => m_buildingHandled; private set { m_buildingHandled = value; } }


    [Header("Regen")]
    [SerializeField]
    private float m_waitTimeToStartRegen = 10;
    private float m_lastTimeGetHit;

    protected override void OnDisable()
    {
        base.OnDisable();
        Deselected();
    }

    private void Update()
    {
        ResurrectDeathUnits();

        if(EntityPlayerType == PlayerType.AI && !IsBusy && !m_behaviorTree.isActiveAndEnabled && m_currentBattle == null)
        {
            Debug.Log("ciaoooo");
        }
    }

    /// <summary>
    /// This will be called after the troop is instantiated by the wave controller.
    /// </summary>
    public void SetTargetCastle(BuildingBehaviour castle, Vector3 engangePoint)
    {
        m_behaviorTree.SetVariableValue("CastleEngagePoint", engangePoint);
        m_behaviorTree.SetVariableValue("TargetCastle", castle);
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

        RefreshUnitsVisual(m_troopStats.UnitType, m_troopStats.UnitQuantity);

        AssignTreeStats();
    }

    public void AssignTreeStats()
    {
        var engageRange = (SharedFloat)m_behaviorTree.GetVariable("EngageRange");
        engageRange.Value = m_troopStats.EngageRange;
        var attackRange = (SharedFloat)m_behaviorTree.GetVariable("AttackRange");
        attackRange.Value = m_troopStats.AttackRange;
        var movimentSpeed = (SharedFloat)m_behaviorTree.GetVariable("MovimentSpeed");
        movimentSpeed.Value = m_troopStats.UnitSpeed;
    }

    public new UnitsStatsSO GetStats()
    {
        return m_troopStats as UnitsStatsSO;
    }

    public int TakeTroopHealth()
    {
        int health = 0;

        for (int i = 0; i < UnitList.Count; i++)
        {
            if(UnitList[i].gameObject.activeSelf)
            health += (int)UnitList[i].UnitHp;
        }

        return health;
    }

    public int TakeTroopCarryCapacity()
    {
        int capacity = 0;

        if (AliveUnit == null)
        {
            Debug.Log("This troop don't have units");
            return capacity;
        }

        capacity = UnitList.Count * m_troopStats.CarryCapacity;

        return capacity;
    }

    #endregion

    #region Units

    public void RefreshUnitsVisual(UnitType inType, int inValue)
    {
        
        for (int i = 0; i < UnitList.Count; i++)
        {
            if (UnitList[i].VisualObj != null)
            {
                UnitList[i].VisualObj.SetActive(false);
                UnitList[i].VisualObj.transform.parent = null;
                UnitList[i].VisualObj = null;
            }

            if (i < inValue)
            {
                UnitList[i].gameObject.SetActive(true);
                GameObject visualUnit = ObjectPooler.Instance.GetUnitObject(inType);
                UnitList[i].VisualObj = visualUnit;
                UnitList[i].VisualObj.transform.parent = UnitList[i].transform;
                UnitList[i].VisualObj.transform.localPosition = Vector3.zero;
                UnitList[i].VisualObj.transform.rotation = UnitList[i].UnitAgent.transform.rotation;
                UnitList[i].VisualObj.SetActive(true);

                UnitList[i].AssignValuesToTree();
                UnitList[i].RefreshHp();

                UnitList[i].transform.DOScale(new Vector3(1, 1, 1), 1f).SetEase(Ease.OutBack);

                if (DeathUnit.Contains(UnitList[i]))
                {
                    UnitList[i].gameObject.SetActive(false);
                }
            }
            else
            {
                UnitList[i].gameObject.SetActive(false);
            }
        }

        SetFormationPositions(m_formationRadius);
    }

    public void DismissUnitInTroop(Unit inUnit)
    {
        if (inUnit.VisualObj != null)
        {
            inUnit.VisualObj.SetActive(false);
            inUnit.VisualObj.transform.parent = null;
            inUnit.VisualObj = null;
        }

        inUnit.AssignFocusToUnit((BuildingBehaviour)null);

        inUnit.gameObject.SetActive(false);
        if (!DeathUnit.Contains(inUnit)) DeathUnit.Enqueue(inUnit);
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
            UnitList[i].UnitFormationPos = inPos[i];
            if (UnitList[i].UnitAgent.isActiveAndEnabled)
                //UnitList[i].UnitAgent.SetDestination(transform.position + inPos[i]);
                UnitList[i].UnitAgent.Warp(transform.position + inPos[i]);
        }
    }

    public void ResetFormation()
    {
        for (int i = 0; i < UnitList.Count; i++)
        {
            if (UnitList[i].gameObject.activeSelf)
            {
                UnitList[i].UnitAgent.enabled = true;
                UnitList[i].UnitAgent.isStopped = false;
                Vector3 destination = transform.position + m_formationPosition[i];
                UnitList[i].UnitAgent.destination = destination;
            }
        }
    }

    public void ResetUnits()
    {
        for (int i = 0; i < UnitList.Count; i++)
        {
            if (UnitList[i].gameObject.activeSelf)
            {
                UnitList[i].StopTree(true);
                //unit.ResetUnitRotation();
                UnitList[i].AssignFocusToUnit((BuildingBehaviour)null);

                UnitList[i].UnitAgent.enabled = true;
                UnitList[i].UnitAgent.isStopped = false;
                Vector3 destination = Agent.pathEndPosition + m_formationPosition[i];
                UnitList[i].UnitAgent.SetDestination(destination);
                UnitList[i].transform.forward = transform.forward;
            }
        }
    }

    #endregion


    #endregion

    #region Commands

    //Command from interfaces
    public void MoveFromTo(Vector3 endPosition)
    {
        if(m_currentBattle != null)
        {
            m_currentBattle.FinishFight();

            if (m_canEscape) { StartCoroutine(IsBusyDelay(2f)); }
        }

        ResetTree();

        m_focusEntity = null;
        m_behaviorTree.SetVariableValue("FocusEntity", m_focusEntity);

        m_destination = endPosition;
        m_behaviorTree.SetVariableValue("Destination", m_destination);
        Agent.destination = endPosition;

        SetNewTroopState(TroopStates.GoToDestination);
    }

    public void ResetTree()
    {
        StopTree(true);
        StopTree(false);
        AssignTreeStats();
    }

    IEnumerator IsBusyDelay(float inValue)
    {
        m_canEscape = false;
        IsBusy = true;

        yield return new WaitForSeconds(inValue);

        IsBusy = false;
        m_canEscape = true;
    }

    public void AssignGameObjectEntity(GameObject inObj)
    {
        EntityBehavior entity = inObj.GetComponent<EntityBehavior>();
        if(entity != null)
        {
            AssignFocusEntity(entity);
        }
    }
    public override void AssignFocusEntity(EntityBehavior inEntity)
    {
        if (m_buildingHandled != null)
        {
            //m_buildingHandled.Drop(this.transform.position);
            return;
        }

        if (m_currentBattle != null)
        {
            m_currentBattle.FinishFight();
        }
        else
        {
            ResetTree();
        }

        m_focusEntity = null;

        if(inEntity == null)
        {
            ResetTree();
            return;
        }

        if (inEntity.EntityPlayerType != this.EntityPlayerType && !inEntity.IsBusy)
        {
            if (inEntity is Troop)
            {
                Troop enemyTroop = inEntity as Troop;

                if (enemyTroop.GetStats().CanTakeDamage && m_troopStats.CanAttack)
                {
                    m_focusEntity = enemyTroop;
                    SetNewTroopState(TroopStates.GoToEnemy);
                    //Debug.Log(gameObject.name + " GO TO ATTACK : " + enemyTroop.name);
                }
            }

            if (inEntity is BuildingBehaviour)
            {
                BuildingBehaviour enemyBuilding = inEntity as BuildingBehaviour;

                if (enemyBuilding.GetStats().CanTakeDamage && m_troopStats.CanAttack)
                {
                    m_focusEntity = enemyBuilding;
                    SetNewTroopState(TroopStates.GoToEnemy);
                    //Debug.Log(gameObject.name + " GO TO ATTACK : " + enemyBuilding.name);
                }
            }
        }
        else
        {
            if (inEntity is BuildingBehaviour && !inEntity.IsBusy)
            {
                BuildingBehaviour building = inEntity as BuildingBehaviour;
                Debug.Log(CurrentCarryCapacity);
                if(CurrentCarryCapacity >= building.GetStats().Weight)
                {
                    m_focusEntity = inEntity;
                    SetNewTroopState(TroopStates.GoToAlly);
                    //Debug.Log(gameObject.name + " GO TO : " + m_focusEntity.name);
                }
            }
        }

        if (FocusEntity != null)
        {
            var focusEntity = (SharedEntity)m_behaviorTree.GetVariable("FocusEntity");
            focusEntity.Value = FocusEntity;
        }
        else
        {
            SetIdleState();
        }
    }

    //Custom commands for troop/entity
    public override void Attack()
    {
        // Remember to get the focus entity or it will run a null reference.
        var focusEntity = (SharedEntity)m_behaviorTree.GetVariable("FocusEntity");
        FocusEntity = focusEntity.Value;

        new Fight(this, FocusEntity);

        if (gameObject.layer == LayerMask.GetMask("Player"))
            HFEventManager.TriggerEvent(HFEventID.OnUnitFight);
    }

    public void SetIdleState()
    {
        StopTree(true);
        
        FocusEntity = null;
        var focusEntity = (SharedEntity)m_behaviorTree.GetVariable("FocusEntity");
        focusEntity.Value = FocusEntity;
        IsBusy = false;

        ResetUnits();

        m_currentBattle = null;

        StopTree(false);
        AssignTreeStats();
        SetNewTroopState(TroopStates.Idle);
    }

    public void Lift()
    {
        if (FocusEntity != null)
        {
            BuildingHandled = FocusEntity as BuildingBehaviour;

            BuildingHandled.transform.parent = this.transform;
            BuildingHandled.Carry(Agent.transform.position + new Vector3(0, 3, 0));

            for (int i = 0; i < UnitList.Count; i++)
            {
                UnitList[i].UnitAgent.enabled = false;
                UnitList[i].transform.position = transform.position + m_formationPosition[i];
                UnitList[i].transform.rotation = this.transform.rotation;
                
                if (!DeathUnit.Contains(UnitList[i]))
                {
                    
                    UnitList[i].visualScript.TriggerAnimation("Lift");
                    UnitList[i].visualScript.TriggerTopLayer(1);

                }
            }

            Agent.SetDestination(FocusEntity.transform.position);

            //HFEventManager.TriggerEvent(HFEventID.OnUnitLift);

            AttachAndPlaySound(m_troopStats.LiftSound);

            HFEventManager.TriggerEvent(HFEventID.OnTutorialQuestCompleted, TutorialID.Carry_Turret);
        }
    }

    public void Drop()
    {
        if (BuildingHandled != null)
        {
            if (BuildingHandled.Drop(Agent.destination.SnapLocation()))
            {

                for (int i = 0; i < UnitList.Count; i++)
                    if (!DeathUnit.Contains(UnitList[i]))
                    {

                        UnitList[i].visualScript.TriggerAnimation("Drop");
                        UnitList[i].visualScript.TriggerTopLayer(0);
                    }
                   

                BuildingHandled.transform.parent = null;

                BuildingHandled = null;
            
                FocusEntity = null;

                ResetFormation();

                SetIdleState();

                //HFEventManager.TriggerEvent(HFEventID.OnUnitDropBuilding);
                HFEventManager.TriggerEvent(HFEventID.OnTutorialQuestCompleted, TutorialID.Reposition_Turret);
            }
        }
    }

    protected override void DisableEntity()
    {
        m_behaviorTree.enabled = false;

        FocusEntity = null;
        m_behaviorTree.SetVariableValue("FocusEntity", null);

        if (m_currentBattle != null)
        {
            m_currentBattle.FinishFight();
        }

        Drop();
        Deselected();

        if (this == InputReaderManager.Instance.CurrentEntity)
        {
            InputReaderManager.Instance.RemoveSelection();
        }

        foreach (Unit unit in UnitList.ToList())
        {
            DismissUnitInTroop(unit);
        }

        m_troopStats = null;

        DeathUnit.Clear();
        this.gameObject.SetActive(false);
        //Return to the pool
    }

    protected override void PauseEntity(bool inValue)
    {
        if (m_isFreezed) return;

        base.PauseEntity(inValue);

        Agent.isStopped = inValue;

        foreach (Unit unit in UnitList)
        {
            unit.UnitAgent.isStopped = inValue;

            if (m_currentBattle != null)
            {
                unit.StopTree(inValue);
                unit.UnitTree.ResetValuesOnRestart = !inValue;
            }
        }
    }

    public override void FreezeMode(bool inValue)
    {
        base.FreezeMode(inValue);

        Agent.isStopped = inValue;

        foreach(Unit unit in UnitList)
        {
            if (unit.UnitAgent.isActiveAndEnabled)
            {
                unit.UnitAgent.isStopped = inValue;
            }
            if(m_currentBattle != null)
            {
                unit.StopTree(inValue);
                unit.UnitTree.ResetValuesOnRestart = !inValue;
            }
        }
    }
    

    #endregion

    #region Troop Hp

    public void TroopTakeDamage(Unit inUnit)
    {
        DismissUnitInTroop(inUnit);
        m_lastTimeGetHit = Time.time;

        if (CurrentHp == 0)
        {
            if(m_currentBattle != null)
            {
                m_currentBattle.FinishFight();
            }
            Death();
        }
    }

    public override void Death()
    {
        HFEventManager.TriggerEvent<EntityBehavior>(HFEventID.OnEntityDeath,this);
        //Destroy(this.gameObject);

        if (EntityPlayerType == PlayerType.Player)
        {
            GameController.Instance.RemoveFromDictionary(this);
        }

        DisableEntity();
    }

    private void ResurrectDeathUnits()
    {
        if (EntityPlayerType == PlayerType.Player && !IsBusy && DeathUnit.Count > 0 && Time.time > m_lastTimeGetHit + m_waitTimeToStartRegen)
        {
            Unit unit = DeathUnit.Dequeue();

            GameObject visualUnit = ObjectPooler.Instance.GetUnitObject(GetStats().UnitType);
            unit.VisualObj = visualUnit;
            unit.VisualObj.transform.parent = unit.transform;
            unit.VisualObj.transform.localPosition = Vector3.zero;
            unit.VisualObj.transform.rotation = unit.UnitAgent.transform.rotation;
            unit.VisualObj.SetActive(true);
            unit.gameObject.SetActive(true);

            unit.AssignValuesToTree();
            unit.RefreshHp();

            unit.UpdateUnitVisualState(InputReaderManager.Instance.CurrentEntity);

            unit.transform.DOScale(new Vector3(1, 1, 1), 1f).SetEase(Ease.OutBack);
        }
    }

    #endregion

    public override void Specialization(UnitType type)
    {
        if (!GameController.Instance.CheckResourcesAvailability(GameController.Instance.Collection.UnitsDictionary[type].OriginalUnitStats.Cost))
        {
            HFEventManager.TriggerEvent(HFEventID.OnError, "You don't have enough resources");
            return;
        }

        GameController.Instance.AddResources(-GameController.Instance.Collection.UnitsDictionary[type].OriginalUnitStats.Cost);
        AssignStats(GameController.Instance.Collection.UnitsDictionary[type].UnitStatsCopy);

        base.Specialization(type);

        foreach (Unit unit in m_unitList)
        {
            unit?.UpdateUnitVisualState(true);
        }
    }

    #region Click Interface

    public override void Click()
    {
        base.Click();
        foreach(Unit unit in UnitList)
        {
            unit?.UpdateUnitVisualState(true);
        }

        HFEventManager.TriggerEvent(HFEventID.OnTutorialQuestCompleted, TutorialID.Select_Unit);
    }
    public override void Deselected()
    {
        base.Deselected();
        foreach (Unit unit in UnitList)
        {
            unit?.UpdateUnitVisualState(false);
        }
    }

    #endregion
}
