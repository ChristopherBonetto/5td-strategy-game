using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Types;

public class NewTroopBehavior : EntityBehavior, ICanMove, ITakeUpgrade
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


    private Vector3[] m_formationPosition = new Vector3[4];

    protected int Xsize;
    protected int Zsize;


    protected NavMeshAgent m_troopAgent;

    public List<NewUnitBehavior> m_units = new List<NewUnitBehavior>();

    protected bool m_moveCoroutineIsActive = false;
    protected bool m_stopBecauseInCombat = false;

    protected IDetect m_detectInterface;


    [Header("carry Field"), Tooltip("Declare where the building will be after carry it")]
    public Transform CarryPoint;


    private void Awake()
    {
        m_troopAgent = gameObject.GetComponent<NavMeshAgent>();
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        m_detectInterface = new DetectBehaviors();
    }

    private void Update()
    {
        
    }

    // Update is called once per frame

    #region New troop methods

    public override void AssignStats(EntityStatsSO inStats)
    {
        base.AssignStats(inStats);
        m_troopAgent.speed = m_troopStats.UnitSpeed;
        CreateUnits(m_troopStats.UnitType, m_troopStats.TroopsQuantity);
    }

    public void CreateUnits(UnitType inType, int inValue)
    {
        m_units = new List<NewUnitBehavior>(inValue);

        for (int i = 0; i < inValue; i++)
        {
            GameObject tempUnit = ObjectPooler.SharedInstance.GetUnityObject(inType);

            NewUnitBehavior tempRef = tempUnit.GetComponent<NewUnitBehavior>();

            if (tempRef == null)
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
        foreach (NewUnitBehavior unit in m_units)
        {
            DeassignUnit(unit);
        }
        m_units = null;

        m_troopStats = null;

        gameObject.SetActive(false);
        //Return to the pool
    }

    public NewUnitBehavior AssignUnit(NewUnitBehavior inUnit)
    {
        inUnit.JoinTroop(this);
        return inUnit;
    }

    public void DeassignUnit(NewUnitBehavior inUnit)
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
        if (m_troopStats == null || m_units.Count == 0)
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
        for (int i = 0; i < m_units.Count; i++)
        {
            m_units[i].transform.localPosition = inPos[i];
        }
    }


    #endregion

    public void MoveFromTo(Vector3 endPosition)
    {
        m_troopAgent.SetDestination(endPosition);
    }

    public void Stop(bool inBool)
    {
        if (m_troopAgent.isStopped != inBool)
            m_troopAgent.isStopped = inBool;
    }

    public bool IsMoving()
    {
        if (!m_troopAgent.hasPath && m_troopAgent.velocity.sqrMagnitude < 0.1f || m_troopAgent.isStopped)
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
        if (Vector3.Distance(transform.position, FocusEntity.transform.position) <= (m_troopAgent.stoppingDistance + FocusEntity.transform.localScale.x + EntityStats.EngageRange))
        {
            return true;
        }
        return false;
    }

    public override void Select()
    {
        base.Select();
    }

    public override void AssignFocusEntity(EntityBehavior inEntity)
    {
        if (inEntity.EntityPlayerType != this.EntityPlayerType)
        {
            if (inEntity.EntityStats.CanTakeDamage)
            {
                FocusEntity = inEntity;
            }
        }
    }

    #region Troop health

    //Prende la vita totale
    public int TakeTroopHealth()
    {
        int health = 0;

        if (m_units.Count == 0)
        {
            Debug.Log("This troop don't have units");
            return health;
        }

        for (int i = 0; i < m_units.Count; i++)
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
    public void TroopTakeDamage(NewUnitBehavior inUnit)
    {
        inUnit.LeaveTroop();

        if (CurrentHp == 0)
        {
            UnlockEntity();
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
                StartCoroutine(Respawn(m_troopStats.RespawnTime));
            }
        }
    }

    public override void Death()
    {
        base.Death();
    }

    //Respawna le unita dopo un timer
    IEnumerator Respawn(float inDestinationTime)
    {
        transform.position = new Vector3(0, 0.5f, 0);
        yield return new WaitForSeconds(inDestinationTime);
        CreateUnits(m_troopStats.UnitType, m_troopStats.TroopsQuantity);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && m_troopStats != null)
        {
            UnityEditor.Handles.color = new Color(1, 0, 0, 0.1f);
            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, m_troopStats.AttackRange);
        }
    }
}
