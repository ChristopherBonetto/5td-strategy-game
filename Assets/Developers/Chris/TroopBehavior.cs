using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Types;

public class TroopBehavior : EntityBehavior, ICanMove, ITakeUpgrade
{
    public UnitsStatsSO m_troopStats;
    public override EntityStatsSO EntityStats
    {
        get
        {
            return m_troopStats;
        }
        set
        {
            m_troopStats = (UnitsStatsSO)value;
        }
    }

    public override int CurrentHp
    {
        get
        {
            return TakeTroopHealth();
        }
    }

    public List<UnitBehavior> m_units = new List<UnitBehavior>();

    private Vector3[] m_formationPosition = new Vector3[4];

    protected int Xsize;
    protected int Zsize;

    protected NavMeshAgent m_agent;

    protected bool m_moveCoroutineIsActive = false;
    protected bool m_isGoingToInteract = false;

    protected IDetect m_detectInterface;


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
        if (!m_isBusy)
        {
            EntityBehavior tempEntity = m_detectInterface.DetectArea(this.transform, EntityStats.EngageRange, ~gameObject.layer);
        }

        if(FocusEntity != null)
        {
            if (!m_isGoingToInteract && !m_isBusy)
            {
                StartCoroutine(GoToInteract());
            }
        }

        if (IsMoving())
        {
            if (!m_moveCoroutineIsActive)
            {
                StartCoroutine(MoveUnits(0.5f));
            }
        }
    }

    #region Troop management

    public override void AssignStats(EntityStatsSO inStats)
    {
        base.AssignStats(inStats);
        CreateUnits(m_troopStats.UnitType, m_troopStats.TroopsQuantity);
    }

    public void CreateUnits(UnitType inType, int inValue)
    {
        m_units = new List<UnitBehavior>(inValue);

        for (int i = 0; i < inValue; i++)
        {
            GameObject tempUnit = ObjectPooler.SharedInstance.GetUnityObject(inType);

            UnitBehavior tempRef = tempUnit.GetComponent<UnitBehavior>();

            if(tempRef == null)
            {
                Debug.Log(inType + "didn't have UnitBehavior script, pls add next time");
                return;
            }
            m_units.Add(AssignUnit(tempRef));
        }
        CreateSquareFormation(1f);
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

    public void CreateSquareFormation(float inOffset = 1)
    {
        if(m_troopStats == null || m_units.Count == 0)
        {
            return;
        }

        Xsize = Mathf.RoundToInt(m_units[0].transform.localScale.x);
        Zsize = Mathf.RoundToInt(m_units[0].transform.localScale.z);
        m_formationPosition = new Vector3[4];

        switch (m_units.Count)
        {
            case 1:
                m_formationPosition[0] = new Vector3(0, transform.position.y, 0);
                break;

            case 2:
                m_formationPosition[0] = new Vector3(-inOffset - Xsize / 2, transform.position.y, 0);
                m_formationPosition[1] = new Vector3(inOffset + Xsize / 2, transform.position.y, 0);
                break;

            case 3:
                m_formationPosition[0] = new Vector3(-inOffset - Xsize / 2, transform.position.y, inOffset + Zsize / 2);
                m_formationPosition[1] = new Vector3(inOffset + Xsize / 2, transform.position.y, inOffset + Zsize / 2);
                m_formationPosition[2] = new Vector3(0, transform.position.y, -inOffset - Zsize / 2);
                break;

            case 4:
                m_formationPosition[0] = new Vector3(-inOffset - Xsize / 2, transform.position.y, inOffset + Zsize / 2);
                m_formationPosition[1] = new Vector3(inOffset + Xsize / 2, transform.position.y, inOffset + Zsize / 2);
                m_formationPosition[2] = new Vector3(-inOffset - Xsize / 2, transform.position.y, -inOffset - Zsize / 2);
                m_formationPosition[3] = new Vector3(inOffset + Xsize / 2, transform.position.y, -inOffset - Zsize / 2);
                break;

        }
        AssignFormation(m_formationPosition);
    }

    public void CreateTriangleFormation(float inOffSet)
    {

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

    //Muovi la truppa e le unita.
    public void MoveFromTo(Vector3 endPosition)
    {
        ResetTroopFocusObj();
        m_agent.SetDestination(endPosition);
    }

    public void ResetTroopFocusObj()
    {
        FocusEntity = null;
        Stop(false);

        foreach(UnitBehavior unit in m_units)
        {
            unit.FocusEntity = null;
            unit.Stop(false);
        }
    }

    private IEnumerator MoveUnits(float inDestinationTime)
    {
        m_moveCoroutineIsActive = true;
        int unitCounter = 0;

        while (IsMoving())
        {
            if (Timer(inDestinationTime))
            {
                m_units[unitCounter].MoveFromTo(m_agent.destination + m_formationPosition[unitCounter]);
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

        Debug.Log("wow");
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
        if ( Vector3.Distance(transform.position, FocusEntity.transform.position) <= m_agent.stoppingDistance + FocusEntity.transform.localScale.x + EntityStats.EngageRange)
        {
            return true;
        }
        return false;
    }

    #endregion

    #region Troop Interactions

    public override void Select()
    {
        base.Select();
    }

    //Come la truppa interagisce con le altre entity.
    public override void Interact(EntityBehavior inEntity)
    {
        if(inEntity is TroopBehavior)
        {
            TroopBehavior tempTroop = inEntity.GetComponent<TroopBehavior>();
            tempTroop.FocusEntity = this;

            for(int i = 0; i < m_units.Count; i++)
            {
                m_units[i].FocusEntity = tempTroop.m_units[i];
            }
            Debug.Log("interact with " + inEntity.name);
        }
    }


    private IEnumerator GoToInteract()
    {
        m_isGoingToInteract = true;

        while(!IsBusy)
        {
            if (!CheckFocussedObjectDistance())
            {
                m_agent.SetDestination(FocusEntity.transform.position);
            }
            else
            {
                ChangeIsBusy(true);
                Stop(true);
            }
            yield return new WaitForSeconds(1);
        }

        Interact(FocusEntity);
        yield return new WaitForEndOfFrame();
        m_isGoingToInteract = false;
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
            if(EntityPlayerType == PlayerType.AI)
            {
                gameObject.SetActive(false);
                return;
            }
            else
            {
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

    private void OnDrawGizmos()
    {
        if(!IsBusy)
        Gizmos.DrawWireSphere(transform.position, EntityStats.EngageRange);
    }
}
