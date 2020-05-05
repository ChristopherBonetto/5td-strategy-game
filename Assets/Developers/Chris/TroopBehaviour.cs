using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Types;
using HF.Unit;

public enum TroopStates
{
    None,
    FreeMovement,
    GoToAttack,
    GoToLift
}

public class TroopBehaviour : EntityBehavior, ICanMove, ITakeUpgrade
{
    [SerializeField] private Transform m_destinationPoint;

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
    public UnitBehavior Captain;
    public List<UnitBehavior> m_units = new List<UnitBehavior>();
    public float FormationRadius;
    private Vector3[] m_formationPosition = new Vector3[4];


    protected bool m_moveCoroutineIsActive = false;

    protected IDetect m_detectInterface;


    [Header("carry Field"), Tooltip("Declare where the building will be after carry it")]
    public Transform CarryPoint;
    public bool IsCarrying { get; set; }

    public BattleHandler currentBattle;

    public TroopStates CurrentTroopState;

    private BuildingBehaviour m_buildingToLift = null;
    private TroopBehaviour m_troopToAttack = null;


    public override EntityBehavior FocusEntity
    {
        get
        {
            return m_focusEntity;
        }
        set
        {
            m_focusEntity = value;
            RefreshState(m_focusEntity);
        }
    }

    public void RefreshState(EntityBehavior inEntity)
    {
        if(inEntity == null)
        {
            m_troopToAttack = null;
            m_buildingToLift = null;
            ChangeTroopState(TroopStates.FreeMovement);
        }
        else
        {
            if (inEntity.EntityPlayerType != this.EntityPlayerType)
            {
                if (inEntity is TroopBehaviour)
                {
                    m_buildingToLift = null;
                    ChangeTroopState(TroopStates.GoToAttack);
                }
            }
            else
            {
                if (inEntity is BuildingBehaviour)
                {
                    m_troopToAttack = null;
                    ChangeTroopState(TroopStates.GoToLift);
                }
            }
        }
        
    }

    public void ChangeTroopState(TroopStates inState)
    {
        switch (inState)
        {
            case TroopStates.None:
                break;
            case TroopStates.FreeMovement:
                break;
            case TroopStates.GoToAttack:
                break;
            case TroopStates.GoToLift:
                break;
        }
    }



    public override void Start()
    {
        base.Start();

        m_detectInterface = new DetectBehaviors();
    }



    #region Troop commands units

    public override void AssignStats(EntityStatsSO inStats)
    {
        base.AssignStats(inStats);
        CreateUnits(m_troopStats.UnitType, m_troopStats.UnitQuantity);
    }

    public void CreateUnits(UnitType inType, int inValue)
    {
        m_units = new List<UnitBehavior>(inValue);

        for (int i = 0; i < inValue; i++)
        {
            GameObject tempUnit = ObjectPooler.Instance.GetUnityObject(inType);

            UnitBehavior tempRef = tempUnit.GetComponent<UnitBehavior>();

            if (tempRef == null)
            {
                Debug.Log(inType + "didn't have UnitBehavior script, pls add next time");
                return;
            }
            if(i == 0)
            {
                Captain = tempRef;
            }
            m_units.Add(AssignUnit(tempRef));
        }
        SetFormationPositions(FormationRadius);
    }


    public void ResetStats()
    {
        foreach (UnitBehavior unit in m_units)
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
        if (m_troopStats == null || m_units.Count == 0)
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
        for (int i = 0; i < m_units.Count; i++)
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

        m_destinationPoint.transform.position = endPosition;
        for(int i = 0; i < m_units.Count; i++)
        {
            m_units[i].MoveFromTo(m_destinationPoint.transform.position + m_formationPosition[i]);
        }
    }

    //TO DO : Cambiare la coroutine se troppo pesante con un sistema che controlli se questo agent si sta muovendo
    // Altra opzione: sistema di waypoints, cosi le truppe farebbero path piu brevi e sono si dividerebbero.
    // Altra opzione: dare direttamente il punto.

    #endregion


    public override void Click()
    {
        base.Click();
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
    public void TroopTakeDamage(UnitBehavior inUnit)
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
        CreateUnits(m_troopStats.UnitType, m_troopStats.UnitQuantity);
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

}