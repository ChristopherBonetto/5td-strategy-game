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
using System.Threading;

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
    #region Variables

    #region Component Var

    public NavMeshAgent Agent;

    #endregion

    #region Stats Var

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

    #endregion

    #region Troop State Var

    public TroopStates m_currentTroopState = TroopStates.Idle;
    public TroopStates CurrentTroopState { get { return m_currentTroopState; } }

    #endregion

    #region Units Var

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

    public int AliveUnitsCount { get => UnitList.Count - DeathUnit.Count; }

    [SerializeField] private float m_formationRadius;
    public Vector3[] m_formationPosition { get; private set; } = new Vector3[4];

    #endregion

    #region Movement Var

    private Vector3 m_destination;
    public Vector3 Destination { get => m_destination; }

    public Vector3 m_engagePointForCastle;
    public Vector3 EngagePointForCastle { get => m_engagePointForCastle; }

    #endregion

    #region Target Var

    private BuildingBehaviour m_buildingHandled;
    public BuildingBehaviour BuildingHandled { get => m_buildingHandled; private set { m_buildingHandled = value; } }

    public CastleStarter m_targetCastle;
    public CastleStarter TargetCastle { get => m_targetCastle; }

    private Coroutine m_resetCoroutine = null;

    #endregion

    #region Generic Var

    [Header("Regen")]
    [SerializeField]
    private float m_waitTimeToStartRegen = 10;
    private float m_lastTimeGetHit;

    #endregion

    #endregion

    #region Behaviour Cycle

    protected override void OnDisable()
    {
        base.OnDisable();
        Deselected();

        IsBusy = false;
        FocusEntity = null;
        
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        AssignDest(transform.position);
        SetNewTroopState(TroopStates.Idle);
    }

    private void Update()
    {
        ResurrectDeathUnits();
    }

    #endregion

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
        if(engageRange.Value != null && m_troopStats != null) engageRange.Value = m_troopStats.EngageRange;

        var attackRange = (SharedFloat)m_behaviorTree.GetVariable("AttackRange");
        if (attackRange.Value != null && m_troopStats != null) attackRange.Value = m_troopStats.AttackRange;

        var movimentSpeed = (SharedFloat)m_behaviorTree.GetVariable("MovimentSpeed");
        if(movimentSpeed.Value != null && m_troopStats != null) movimentSpeed.Value = m_troopStats.UnitSpeed;
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

        for (int i = 0; i < UnitList.Count; i++)
        {
            if (UnitList[i].gameObject.activeSelf)
                capacity += m_troopStats.CarryCapacity;
        }

        return capacity;
    }

    #endregion

    #region Units

    public void RefreshUnitsVisual(UnitType inType, int inQuantity)
    {
        for (int i = 0; i < UnitList.Count; i++)
        {
            if (UnitList[i].VisualObj != null)
            {
                UnitList[i].VisualObj.SetActive(false);
                UnitList[i].VisualObj.transform.parent = null;
                UnitList[i].VisualObj = null;
            }

            if (i < inQuantity)
            {
                UnitList[i].gameObject.SetActive(true);
                GameObject visualUnit = ObjectPooler.Instance.GetUnitObject(inType);
                UnitList[i].VisualObj = visualUnit;
                UnitList[i].VisualObj.transform.parent = UnitList[i].transform;
                UnitList[i].VisualObj.transform.localPosition = Vector3.zero;
                UnitList[i].VisualObj.transform.rotation = UnitList[i].UnitAgent.transform.rotation;
                UnitList[i].VisualObj.SetActive(true);

                UnitList[i].SetUnitHp(UnitList[i].PreviousHp);

                UnitList[i].AssignValuesToTree();

                UnitList[i].UpdateUnitVisualState(this == InputReaderManager.Instance.CurrentEntity);

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

            UnitList[i].transform.name = "Unit_" + i + "_" + m_troopStats.UnitType;
        }

        SetFormationPositions(m_formationRadius);
    }

    public void DismissUnitInTroop(Unit inUnit)
    {
        if (inUnit.VisualObj != null)
        {
            inUnit.UpdateUnitVisualState(false);
            inUnit.VisualObj.SetActive(false);
            inUnit.VisualObj.transform.parent = null;
            inUnit.VisualObj = null;
        }

        inUnit.AssignFocusToUnit((BuildingBehaviour)null);
        inUnit.StopTree(true);

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
            {
                //UnitList[i].UnitAgent.SetDestination(transform.position + inPos[i]);
                NavMeshHit hit;

                if (NavMesh.SamplePosition(inPos[i], out hit, 1, NavMesh.AllAreas))
                {
                    if (hit.position != null)
                    {
                        UnitList[i].UnitAgent.Warp(hit.position);
                    }
                    else
                    {
                        TroopTakeDamage(UnitList[i]);
                    }
                }
            }
        }
    }

    public void ResetFormation()
    {
        AssignDest(Agent.transform.position);

        for (int i = 0; i < UnitList.Count; i++)
        {
            if (UnitList[i].gameObject.activeSelf)
            {
                UnitList[i].UnitAgent.enabled = true;
                UnitList[i].UnitAgent.isStopped = false;
                Vector3 destination = transform.position + m_formationPosition[i];

                if(UnitList[i].UnitAgent.isActiveAndEnabled)
                UnitList[i].UnitAgent.destination = destination;
            }
        }
    }

    public void ResetUnits()
    {
        for (int i = 0; i < UnitList.Count; i++)
        {
            UnitList[i].AssignFocusToUnit((BuildingBehaviour)null);

            if (UnitList[i].gameObject.activeSelf)
            {
                UnitList[i].StopTree(true);
                //unit.ResetUnitRotation();

                UnitList[i].UnitAgent.enabled = true;
                UnitList[i].UnitAgent.isStopped = false;
                Vector3 destination = Agent.pathEndPosition + m_formationPosition[i];
                UnitList[i].transform.forward = transform.forward;

                if (UnitList[i].UnitAgent.isActiveAndEnabled)
                    UnitList[i].UnitAgent.SetDestination(destination);
            }
        }
    }

    #endregion


    #endregion

    #region Commands

    //Command from interfaces
    public void MoveFromTo(Vector3 endPosition)
    {
        if (!m_isFreezed)
        {
            if (FocusEntity != null && FocusEntity is Troop) { ResetTroop(3f, endPosition); }
        }

        FocusEntity = null;
        AssignDest(endPosition);
        SetNewTroopState(TroopStates.GoToDestination);
    }

    private void AssignDest(Vector3 inDest)
    {
        if (Agent.isActiveAndEnabled && Agent.isOnNavMesh)
        {
            m_destination = inDest;
            var dest = (SharedVector3)m_behaviorTree.GetVariable("Destination");
            if (dest != null) dest.Value = m_destination;
            Agent.SetDestination(inDest);
        }
    }

    IEnumerator Reset(float inDestinationTime = 0.3f, Vector3? inResetDestination = null)
    {
        IsBusy = true;

        if (FocusEntity != null && FocusEntity is Troop && FocusEntity.FocusEntity == this)
        {
            Troop troop = FocusEntity as Troop;

            foreach (Unit unit in troop.UnitList)
            {
                unit.StopTree(true);
                unit.AssignFocusToUnit((Unit)null);

                if (unit.isActiveAndEnabled)
                {
                    unit.UnitAgent.enabled = true;
                    unit.UnitAgent.ResetPath();
                    unit.UnitAgent.isStopped = false;
                }
            }
            troop.FocusEntity = null;
            troop.IsBusy = false;
            troop.StopTree(false);
        }

        foreach (Unit unit in UnitList)
        {
            unit.StopTree(true);
            unit.AssignFocusToUnit((Unit)null);

            if (unit.isActiveAndEnabled)
            {
                unit.UnitAgent.enabled = true;
                unit.UnitAgent.ResetPath();
                unit.UnitAgent.isStopped = false;
            }
        }

        FocusEntity.FocusEntity = null;
        FocusEntity = null;

        if(inResetDestination != null)
        {
            StopTree(false);
            AssignTreeStats();
            MoveFromTo(inResetDestination.Value);
        }
        else
        {
            StopTree(false);
            AssignTreeStats();
            SetNewTroopState(TroopStates.Idle);
        }
        
        yield return new WaitForSeconds(inDestinationTime);

        m_resetCoroutine = null;
        IsBusy = false;
    }

    public void AssignGameObjectEntity(GameObject inObj)
    {
        EntityBehavior entity = inObj.GetComponent<EntityBehavior>();
        if(entity != null)
        {
            AssignFocusEntity(entity);
        }
    }
    public override bool AssignFocusEntity(EntityBehavior inEntity)
    {
        if (!m_isFreezed)
        {
            ResetTree();
        }

        if (m_buildingHandled != null || FocusEntity != null)
        {
            //m_buildingHandled.Drop(this.transform.position);
            return false;
        }

        FocusEntity = null;

        if (inEntity.EntityPlayerType != this.EntityPlayerType && !inEntity.IsBusy)
        {
            if (inEntity is Troop)
            {
                Troop enemyTroop = inEntity as Troop;

                if (enemyTroop.GetStats().CanTakeDamage && m_troopStats.CanAttack)
                {
                    AssignDest(inEntity.transform.position);
                    FocusEntity = enemyTroop;
                    SetNewTroopState(TroopStates.GoToEnemy);
                    //Debug.Log(gameObject.name + " GO TO ATTACK : " + enemyTroop.name);
                    return true;
                }
            }

            if (inEntity is BuildingBehaviour)
            {
                BuildingBehaviour enemyBuilding = inEntity as BuildingBehaviour;

                if (enemyBuilding.GetStats().CanTakeDamage && m_troopStats.CanAttack)
                {
                    FocusEntity = enemyBuilding;
                    SetNewTroopState(TroopStates.GoToEnemy);
                    //Debug.Log(gameObject.name + " GO TO ATTACK : " + enemyBuilding.name);
                    return true;
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
                    AssignDest(inEntity.transform.position);
                    FocusEntity = inEntity;
                    SetNewTroopState(TroopStates.GoToAlly);
                    //Debug.Log(gameObject.name + " GO TO : " + m_focusEntity.name);
                    return true;
                }
            }
        }

        if(FocusEntity == null)
        {
            Debug.LogWarning("Assigned null FOCUS ENTITY");
            //SetIdleState();
            return false;
        }
        return false;
    }

    /// <summary>
    /// This will be called after the troop is instantiated by the wave controller.
    /// </summary>
    public void AssignTargetCastle(BuildingBehaviour castle, Vector3 engagePoint)
    {
        m_targetCastle = castle as CastleStarter;
        m_engagePointForCastle = engagePoint;
    }

    public void Lift()
    {
        if (FocusEntity != null)
        {
            if (FocusEntity.IsBusy)
            {
                SetIdleState();
                return;
            }

            BuildingHandled = FocusEntity as BuildingBehaviour;

            //Vector3 dropPosition = Vector3.zero;

            //for (int i = 0; i < UnitList.Count; i++)
            //{
            //    if (UnitList[i].gameObject.activeSelf)
            //        dropPosition = new Vector3(dropPosition.x + UnitList[i].transform.position.x, 0, dropPosition.z + UnitList[i].transform.position.z);
            //}

            //dropPosition = dropPosition / UnitList.Count;
            //dropPosition.y = 3f;

            BuildingHandled.Carry(Agent.transform.position + new Vector3(0, 3f, 0));
            BuildingHandled.transform.parent = this.transform;


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

            SetIdleState();

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
            
                SetIdleState();

                //HFEventManager.TriggerEvent(HFEventID.OnUnitDropBuilding);
                HFEventManager.TriggerEvent(HFEventID.OnTutorialQuestCompleted, TutorialID.Reposition_Turret);
            }
        }
    }

    #endregion

    #region Reset and Pause methods

    public void SetIdleState()
    {
        FocusEntity = null;
        IsBusy = false;
        AssignDest(transform.position);

        ResetUnits();

        if (!m_behaviorTree.isActiveAndEnabled)
        {
            ResetTree();
        }
        if (!Agent.isActiveAndEnabled)
        {
            Agent.enabled = true;
            Agent.isStopped = false;
        }

        SetNewTroopState(TroopStates.Idle);
    }

    public void ResetTree()
    {
        StopTree(true);
        StopTree(false);
        AssignTreeStats();
    }

    protected override void DisableEntity()
    {
        m_behaviorTree.enabled = false;

        FocusEntity = null;
        IsBusy = false;

        Drop();
        Deselected();

        if (this == InputReaderManager.Instance.CurrentEntity)
        {
            InputReaderManager.Instance.RemoveSelection();
        }

        int index = 0;
        foreach (Unit unit in UnitList.ToList())
        {
            DismissUnitInTroop(unit);
            unit.transform.position = transform.position + m_formationPosition[index];
            index++;
        }

        m_troopStats = null;

        DeathUnit.Clear();

        this.gameObject.SetActive(false);
        m_isFreezed = false;
        //Return to the pool
    }

    protected override void PauseEntity(bool inValue)
    {
        if (m_isFreezed) return;

        //DA RIVEDERE PERCHE COSI SE è IN FIGHT SI RIATTIVA IL COMANDANTE CHE VA PER I CAVOLI SUOI. QUINDI O SI FA UN CONTROLLO SU FOCUS ENTITY O MEGLIO SI FA UN CHECK PIU
        //MIRATO DOVE COTROLLA UN PO TUTTO
        base.PauseEntity(inValue);

        if (Agent.isActiveAndEnabled)
        {
            Agent.isStopped = inValue;
        }

        foreach (Unit unit in UnitList)
        {
            if (unit.UnitAgent.isActiveAndEnabled)
            {
                unit.UnitAgent.isStopped = inValue;
            }
            if(unit.visualScript != null)
            {
                if (unit.visualScript.UnitAnimator != null)
                    unit.visualScript.UnitAnimator.enabled = !inValue;
            }
            if (FocusEntity != null)
            {
                unit.StopTree(inValue);
                unit.UnitTree.ResetValuesOnRestart = !inValue;
            }
        }
    }

    public override void FreezeMode(bool inValue)
    {
        base.FreezeMode(inValue);

        if (Agent.isActiveAndEnabled)
        {
            Agent.isStopped = inValue;
        }

        foreach(Unit unit in UnitList)
        {
            if (unit.UnitAgent.isActiveAndEnabled)
            {
                unit.UnitAgent.isStopped = inValue;
            }
            if (unit.visualScript != null)
            {
                if(unit.visualScript.UnitAnimator != null)
                   unit.visualScript.UnitAnimator.enabled = !inValue;
            }
            if (FocusEntity != null)
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
        else if (EntityPlayerType == PlayerType.AI) 
        {
            HFGem gem = ObjectPooler.Instance.GetPooledObject("Gem").GetComponent<HFGem>();
            gem.SetAmount(m_troopStats.GemDropAmount);
            gem.transform.position = transform.position.SnapLocation();
            gem.gameObject.SetActive(true);
        }

        DisableEntity();
    }

    IEnumerator TroopDie()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        FocusEntity = null;
        IsBusy = true;

        foreach (Unit unit in UnitList)
        {
            unit.AssignFocusToUnit((Unit)null);
            unit.UnitAgent.isStopped = false;
            unit.UnitAgent.ResetPath();
            unit.StopTree(true);
        }

        yield return new WaitForSeconds(1f);
        StopTree(true);
        IsBusy = false;
        gameObject.GetComponent<Collider>().enabled = true;
        gameObject.SetActive(false);
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
            unit.SetUnitHp(1);

            unit.UpdateUnitVisualState(this == InputReaderManager.Instance.CurrentEntity);

            unit.transform.DOScale(new Vector3(1, 1, 1), 1f).SetEase(Ease.OutBack);
        }
    }

    #endregion

    #region Specialization
    public override void Specialization(UnitType type)
    {
        UnitsStatsSO tempStats = GameController.Instance.Collection.UnitsDictionary[type].UnitStatsCopy;

        if (!GameController.Instance.CheckResourcesAvailability(tempStats.Cost))
        {
            HFEventManager.TriggerEvent(HFEventID.OnError, "You don't have enough resources");
            return;
        }

        GameController.Instance.AddResources(-tempStats.Cost);
        
        foreach(Unit unit in UnitList)
        {
            float HPperc = (float)(unit.UnitHp / m_troopStats.MaxHp);
            unit.PreviousHp = HPperc;
        }

        AssignStats(tempStats);

        base.Specialization(type);

        AttachAndPlaySound(m_troopStats.UpgradeSound);
    }

    #endregion

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

    #region AttackReworked

    //Custom commands for troop/entity
    public override void Attack()
    {
        if (m_isFreezed)
            return;

        if (FocusEntity is Troop)
        {
            TroopAttack(FocusEntity as Troop);
        }
        else if(FocusEntity is BuildingBehaviour)
        {
            BuildingAttack(FocusEntity as BuildingBehaviour);
        }

        //Must be changed with EntityPlayerType == PlayerType.Player
        if (gameObject.layer == LayerMask.GetMask("Player"))
            HFEventManager.TriggerEvent(HFEventID.OnUnitFight);
    }

    public void TroopAttack(Troop inTroop)
    {
        if(m_troopStats.AttackType == AttackType.MELEE)
        {
            inTroop.IsBusy = true;
            this.IsBusy = true;

            inTroop.StopTree(true);
            inTroop.Agent.isStopped = true;

            this.StopTree(true);
            this.Agent.isStopped = true;

            inTroop.FocusEntity = this as Troop;

            List<Unit> alive = new List<Unit>();
            for (int i = 0; i < UnitList.Count; i++)
            {
                if (!DeathUnit.Contains(UnitList[i]) && UnitList[i].gameObject.activeSelf)
                    alive.Add(UnitList[i]);
            }
            List<Unit> myTarget = inTroop.GetUnitAsTargetList(alive.Count);

            for (int i = 0; i < alive.Count; i++)
            {
                
                alive[i].AssignFocusToUnit(myTarget[i]);
                alive[i].StopTree(false);
            }


            List<Unit> aliveEnemy = new List<Unit>();
            for (int i = 0; i < inTroop.UnitList.Count; i++)
            {
                if (!inTroop.DeathUnit.Contains(inTroop.UnitList[i]) && inTroop.UnitList[i].gameObject.activeSelf)
                    aliveEnemy.Add(inTroop.UnitList[i]);
            }
            List<Unit> myTargetEnemy = GetUnitAsTargetList(aliveEnemy.Count);

            for (int i = 0; i < aliveEnemy.Count; i++)
            {
                aliveEnemy[i].AssignFocusToUnit(myTargetEnemy[i]);
                aliveEnemy[i].StopTree(false);
            }


            //for (int i = 0; i < UnitList.Count; i++)
            //{
            //    if (!DeathUnit.Contains(UnitList[i]))
            //    {
            //        GiveAnotherTargetToUnit(UnitList[i]);
            //        UnitList[i].StopTree(false);
            //    }
            //}

            //for (int i = 0; i < inTroop.UnitList.Count; i++)
            //{
            //    if (!inTroop.DeathUnit.Contains(inTroop.UnitList[i]))
            //    {
            //        inTroop.GiveAnotherTargetToUnit(inTroop.UnitList[i]);
            //        inTroop.UnitList[i].StopTree(false);
            //    }
            //}
        }
        else if(m_troopStats.AttackType == AttackType.RANGED)
        {
            this.Agent.isStopped = true;

            List<Unit> alive = new List<Unit>();
            for (int i = 0; i < UnitList.Count; i++)
            {
                if (!DeathUnit.Contains(UnitList[i]) && UnitList[i].gameObject.activeSelf)
                    alive.Add(UnitList[i]);
            }
            List<Unit> myTarget = inTroop.GetUnitAsTargetList(alive.Count);

            for (int i = 0; i < alive.Count; i++)
            {
                alive[i].AssignFocusToUnit(myTarget[i]);
                alive[i].StopTree(false);
            }

            //for (int i = 0; i < UnitList.Count; i++)
            //{
            //    if (!DeathUnit.Contains(UnitList[i]))
            //    {
            //        GiveAnotherTargetToUnit(UnitList[i]);
            //        UnitList[i].StopTree(false);
            //    }
            //}
        }
    }

    public List<Unit> GetUnitAsTargetList(int _quantity)
    {
        List<Unit> alive = new List<Unit>();
        for (int i = 0; i < UnitList.Count; i++)
        {
            if (!DeathUnit.Contains(UnitList[i]) && UnitList[i].gameObject.activeSelf)
            {
                alive.Add(UnitList[i]);
            }
        }

        int index = 0;
        List<Unit> tmp = new List<Unit>();
        for (int i = 0; i < _quantity; i++)
        {
            tmp.Add(alive[index]);

            index++;
            index = (int)Mathf.Repeat(index, alive.Count);
        }
        Debug.Log(index + " " + alive.Count);

        return tmp;
    }

    public void GiveAnotherTargetToUnit(Unit inUnit)
    {
        float distance = 1000f;
        Unit tempUnit = null;

        Troop troop = FocusEntity as Troop;

        if (troop == null) return;

        for (int i = 0; i < troop.UnitList.Count; i++)
        {
            if (troop.UnitList[i].gameObject.activeSelf)
            {
                if (Vector3.Distance(inUnit.transform.position, troop.UnitList[i].transform.position) < distance)
                {
                    tempUnit = troop.UnitList[i];
                }
            }
        }

        if (tempUnit != null)
        {
            inUnit.AssignFocusToUnit(tempUnit);
        }
        else
        {
            inUnit.FocusBuilding = null;
            inUnit.FocusUnit = null;
            inUnit.StopTree(true);
        }
    }

    public void BuildingAttack(BuildingBehaviour inBuilding)
    {
        Agent.isStopped = true;
        StopTree(true);

        FocusEntity = inBuilding as BuildingBehaviour;

        for (int i = 0; i < UnitList.Count; i++)
        {
            if (!DeathUnit.Contains(UnitList[i]))
            {
                UnitList[i].AssignFocusToUnit(inBuilding as BuildingBehaviour);
                UnitList[i].StopTree(false);
            }
        }
    }

    protected override void MyTargetIsDeath(EntityBehavior inEntity)
    {
        if(inEntity == FocusEntity)
        {
            ResetTroop(0.3f, null);
        }
    }

    public void ResetTroop(float inDestinationTime = 0.3f, Vector3? inDestPos = null)
    {
        if(m_resetCoroutine == null)
        {
            m_resetCoroutine = StartCoroutine(Reset(inDestinationTime, inDestPos));
        }
    }

    #endregion

    #region GameState event

    protected override void GameStateChanged(GameStates inState)
    {
        base.GameStateChanged(inState);

        if(inState == GameStates.EndLevel)
        {
            foreach(Unit unit in UnitList)
            {
                unit.AssignFocusToUnit((Unit)null);
                unit.StopTree(true);
            }
        }
    }

    #endregion
}
