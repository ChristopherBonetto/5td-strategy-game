using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Types;
using HF.Unit;


public class TroopBehavior : EntityBehavior, ICanMove, ITakeUpgrade
{

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

    [Header("Formation")]
    public List<UnitBehavior> m_units = new List<UnitBehavior>();
    public float FormationRadius;
    private Vector3[] m_formationPosition = new Vector3[4];


    protected NavMeshAgent m_agent;
    protected bool m_moveCoroutineIsActive = false;
    protected bool m_stopBecauseInCombat = false;
    protected IDetect m_detectInterface;


    [Header("carry Field"), Tooltip("Declare where the building will be after carry it")]
    public Transform CarryPoint;
    public bool IsCarrying { get; set; }

    public BattleHandler currentBattle;

    private void Awake()
    {
        TakeAgentComponent();
    }
    public override void Start()
    {
        base.Start();

        m_detectInterface = new DetectBehaviors();
    }


    public void Update()
    {
        if(!inCombat && !IsCarrying)
        {
            if (m_agent.hasPath)
            {
                if (m_agent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    CheckToInterctWithNearbyEntity();
                    CommandUnitsToFollowMe();
                }
            }
        }
    }

    #region Troop Behavior

    public void CheckToInterctWithNearbyEntity()
    {
        if (FocusEntity != null)
        {
            if (!CheckFocussedObjectDistance())
            {
                Debug.Log("Go to destination");
                m_agent.SetDestination(FocusEntity.transform.position);
            }
            else
            {
                Interact();
            }
        }
        else
        {
            //FocusEntity = m_detectInterface.DetectArea(this.transform, EntityStats.EngageRange, ~gameObject.layer);
        }
    }


    public void Interact()
    {
        if(FocusEntity is TroopBehavior)
        {
            ChangeInCombat(true);
            Debug.Log("WOOOOOW");
            TroopBehavior troop = (TroopBehavior)FocusEntity;
            new Fight(this, troop);
        }
        else if(FocusEntity is BuildingBehaviour)
        {
        }
    }

    //Serve per far scappare dal fight.
    public override void UnlockEntity()
    {
        if(currentBattle != null)
        {
            currentBattle.FinishFight();
        }
    }

    #endregion

    #region Troop commands units

    public override void AssignStats(EntityStatsSO inStats)
    {
        base.AssignStats(inStats);
        m_agent.speed = m_troopStats.UnitSpeed;
        CreateUnits(m_troopStats.UnitType, m_troopStats.TroopsQuantity);
    }

    public void CreateUnits(UnitType inType, int inValue)
    {
        m_units = new List<UnitBehavior>(inValue);

        for (int i = 0; i < inValue; i++)
        {
            GameObject tempUnit = ObjectPooler.Instance.GetUnityObject(inType);

            UnitBehavior tempRef = tempUnit.GetComponent<UnitBehavior>();

            if(tempRef == null)
            {
                Debug.Log(inType + "didn't have UnitBehavior script, pls add next time");
                return;
            }
            m_units.Add(AssignUnit(tempRef));
        }
        SetFormationPositions(1.5f);
    }

    public void CommandUnitsToFollowMe()
    {
        if (IsMoving())  // Chris: spostato nell'update xke comune tra i metodi.  Begin-End Modification @Panta (Add !IsCarrying)
        {
            if (!m_moveCoroutineIsActive)
            {
                StartCoroutine(MoveUnits(0.15f));
            }
        }
    }

    public void ResetStats()
    {
        foreach(UnitBehavior unit in m_units)
        {
            DeassignUnit(unit);
        }
        m_units = null;

        m_troopStats = null;

        gameObject.SetActive(false);
        //Return to the pool
    }

    public UnitBehavior AssignUnit(UnitBehavior inUnit)
    {
        inUnit.JoinTroop(this);
        return inUnit;
    }

    public void DeassignUnit(UnitBehavior inUnit)
    {
        if (!m_units.Contains(inUnit))
        {
            return;
        }
        inUnit.LeaveTroop();
    }

    #endregion

    #region Troop Formation

    public void SetFormationPositions(float inRadius = 1)
    {
        if(m_troopStats == null || m_units.Count == 0)
        {
            return;
        }


        // Begin Modification @Panta
        // Here we store each offset position.
        // Note the case 2 and 4 are different cause of angle offset.
        // In case 2 the offset is -90 degree, while in case 4 is -45 degree.

        switch (m_units.Count)
        {
            case 1:
                m_formationPosition[0] = Vector3.zero;
                break;

            case 2:
                // Reassign value to each position.
                for (int i = 0; i < m_units.Count; i++)
                {
                    // Calculate the angle in radian (not degree)
                    float angle = Mathf.PI * 2 / m_units.Count * i - (90 * Mathf.Deg2Rad);
                    angle += transform.eulerAngles.y * Mathf.Deg2Rad;

                    m_formationPosition[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * inRadius;
                }
                break;

            case 4:
                // Reassign value to each position.
                for (int i = 0; i < m_units.Count; i++)
                {
                    // Calculate the angle in radian (not degree)
                    float angle = Mathf.PI * 2 / m_units.Count * i - (45 * Mathf.Deg2Rad);
                    angle += transform.eulerAngles.y * Mathf.Deg2Rad;

                    m_formationPosition[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * inRadius;
                }
                break;

            default:
                // Reassign value to each position.
                for (int i = 0; i < m_units.Count; i++)
                {
                    // Calculate the angle in radian (not degree)
                    float angle = Mathf.PI * 2 / m_units.Count * i;
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
        for(int i = 0; i < m_units.Count; i++)
        {
            m_units[i].transform.localPosition = inPos[i];
        }
    }

    public void ResetFormation()
    {
        for (int i = 0; i < m_units.Count; i++)
        {
            Vector3 destination = transform.position + m_formationPosition[i];
            m_units[i].MoveFromTo(destination);
        }
    }

    #endregion

    #region Troop movement

    //Muovi la truppa e le unita. Da usare per uscire dal fight siccome resetta tutto.
    public void MoveFromTo(Vector3 endPosition)
    {
        //cercare un altro controllo per farlo uscire dal fight
        //UnlockEntity();

        m_agent.SetDestination(endPosition);
    }

    //TO DO : Cambiare la coroutine se troppo pesante con un sistema che controlli se questo agent si sta muovendo
    // Altra opzione: sistema di waypoints, cosi le truppe farebbero path piu brevi e sono si dividerebbero.
    // Altra opzione: dare direttamente il punto.
    private IEnumerator MoveUnits(float inDestinationTime)
    {
        m_moveCoroutineIsActive = true;
        int unitCounter = 0;

        while (IsMoving())
        {
            if (Timer(inDestinationTime))
            {
                m_units[unitCounter].MoveFromTo(m_agent.transform.position + m_formationPosition[unitCounter]);
                unitCounter++;
                
                if(unitCounter > m_units.Count -1 )
                {
                    unitCounter = 0;
                }
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
        ResetFormation();

        Debug.Log("destination reached");
        m_moveCoroutineIsActive = false;
    }

    public void TakeAgentComponent()
    {
        m_agent = gameObject.GetComponent<NavMeshAgent>();

        if (m_agent == null)
        {
            m_agent = gameObject.AddComponent<NavMeshAgent>();
        }
    }

    public void Stop(bool inBool)
    {
        if (m_agent.isStopped != inBool)
            m_agent.isStopped = inBool;
    }

    public bool IsMoving()
    {
        if (!m_agent.hasPath && m_agent.velocity.sqrMagnitude < 0.1f || m_agent.isStopped)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    protected virtual bool CheckFocussedObjectDistance()
    {
        float distance = m_agent.stoppingDistance + FocusEntity.transform.localScale.x + m_troopStats.EngageRange;

        if (m_agent.remainingDistance <= distance)
        {
            Debug.Log("STOP");
            Stop(true);
            return true;
        }
        return false;
    }

    #endregion

    #region Troop Click

    public override void Click()
    {
        base.Click();
    }

    //Come la truppa interagisce con le altre entity.
    public override void AssignFocusEntity(EntityBehavior inEntity)
    {
        if(inEntity.EntityPlayerType != this.EntityPlayerType)
        {
            //GESTIRE A STATI
            if(inEntity is UnitBehavior)
            {
                Debug.Log("ATTACK");
                FocusEntity = (inEntity as UnitBehavior).TroopRef;
            }
            else if(inEntity is BuildingBehaviour)
            {
                FocusEntity = inEntity as BuildingBehaviour;
            }
            m_agent.SetDestination(inEntity.transform.position);
        }
        
    }

    #endregion

    #region Troop health

    //Prende la vita totale
    public int TakeTroopHealth()
    {
        int health = 0;

        if(m_units.Count == 0)
        {
            Debug.Log("This troop don't have units");
            return health;
        }

        for(int i = 0; i < m_units.Count; i++)
        {
            health += m_units[i].CurrentHp;
        }
        
        return health;
    }

    //Non usato
    public override bool TakeDamage(int Damage = 0)
    {
        return true;
    }

    //Come prende danno la truppa
    public void TroopTakeDamage(UnitBehavior inUnit)
    {
        inUnit.LeaveTroop();

        if(CurrentHp == 0)
        {
            UnlockEntity();
            if(EntityPlayerType == PlayerType.AI)
            {
                Death();
                return;
            }
            else
            {
                if(InputReaderManager.Instance.CurrentEntity == this)
                {
                    InputReaderManager.Instance.CurrentEntity = null;
                }
                StartCoroutine("Respawn");
            }
        }
    }

    public override void Death()
    {
        base.Death();
    }

    //Respawna le unita dopo un timer
    IEnumerator Respawn()
    {
        transform.position = new Vector3(0, 0.5f, 0);
        yield return new WaitForSeconds(m_troopStats.RespawnTime);
        CreateUnits(m_troopStats.UnitType, m_troopStats.TroopsQuantity);
    }

    #endregion

    // Begin Modification @Panta
    #region Carrying

    public void EnableCarryAction(bool enable)
    {
        // Maybe check if units are fighting 

        if (enable)
        {
            FormationRadius = 1f;

            SetFormationPositions();
            ResetFormation();

            for (int i = 0; i < m_units.Count; i++)
            {
                m_units[i].UnitAgent.enabled = false;
            }
        }
        else
        {
            FormationRadius = 2f;

            SetFormationPositions();
            ResetFormation();

            for (int i = 0; i < m_units.Count; i++)
            {
                m_units[i].UnitAgent.enabled = true;
            }
        }
    }

    #endregion
    // End Modification @Panta

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && m_troopStats != null)
        {
            UnityEditor.Handles.color = new Color(1, 0, 0, 0.1f);
            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, m_troopStats.EngageRange);
        }
    }
}
