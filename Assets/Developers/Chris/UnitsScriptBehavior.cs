using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;



public class UnitsScriptBehavior : Entity, IDamageable
{
    [SerializeField] private EntityType m_startUnitsInfo = EntityType.Farmer;

    public Actions m_CurrentUnitAction { get; protected set; }
    
    protected NavMeshAgent m_UnitAgent;
    public NavMeshAgent UnitAgent
    {
        get
        {
            return m_UnitAgent;
        }
        set
        {
            m_UnitAgent = value;
        }
    }

    protected IDamageable CanTakeDamage;

    protected GameObject m_focusObject = null;
    public GameObject FocusObject
    {
        get
        {
            return m_focusObject;
        }
        set
        {
            m_focusObject = value;
        }
    }

    protected int m_UnitCurrentHp = 10;



    public virtual void Awake()
    {
        UnitAgent = gameObject.GetComponent<NavMeshAgent>();

        if(UnitAgent == null)
        {
            UnitAgent = gameObject.AddComponent<NavMeshAgent>();
        }
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        Initialize();
    }


    public virtual void Update()
    {
        if (CheckFocussedObjectDistance() && m_CanAttack)
        {            
            Attack();
        }

        if (!m_CanAttack)
        {
            m_CanAttack = Timer(UnitStatisticsSO.AttackSpeed);
        }
        
        if(FocusObject != null)
        {
            gameObject.transform.LookAt(new Vector3(FocusObject.transform.position.x, gameObject.transform.position.y, FocusObject.transform.position.z));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (gameObject.layer != GameController.Instance.m_playerLayer)
            {
                FocusObject = GameController.Instance.PlayerCastle;
            }
        }
    }

    public virtual void Initialize()
    {
        UnitStatisticsSO = GameController.Instance.GameCollectionCopy.GameEntitiesDictionary[m_startUnitsInfo];

        UnitAgent.speed = UnitAgent.speed + UnitStatisticsSO.MovementSpeed;
        UpdateCurrentHp();
    }

    public virtual void ChangeUnitState(Actions NewAction)
    {
        m_CurrentUnitAction = NewAction;
    }

    
    public virtual void Attack()
    {
        ChangeUnitState(Actions.Attack);
        
        CanTakeDamage = FocusObject.GetComponent<IDamageable>() as IDamageable;
        if (CanTakeDamage != null)
        {
            CanTakeDamage.TakeDamage(UnitStatisticsSO.Attack);
            m_CanAttack = false;
        }
    }

    protected virtual bool CheckFocussedObjectDistance()
    {
        if (FocusObject != null)
        {
            if (Vector3.Distance(transform.position, FocusObject.transform.position) <= m_UnitAgent.stoppingDistance + FocusObject.transform.localScale.x + UnitStatisticsSO.Range)
            {
                m_UnitAgent.ResetPath();

                if (m_UnitAgent.velocity.sqrMagnitude == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if(m_UnitAgent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    ChangeUnitState(Actions.Move);
                    m_UnitAgent.SetDestination(FocusObject.transform.position);
                }
            }
        }
        else if(FocusObject == null)
        {
            
            if (m_UnitAgent.velocity.sqrMagnitude == 0 && !m_UnitAgent.pathPending && !m_UnitAgent.hasPath && m_CurrentUnitAction != Actions.Idle)
            {
                ChangeUnitState(Actions.Idle);
            }
            else
            {

            }
            return false;
        }
        return false;
    }

    public virtual void SetDestination(Vector3 destination)
    {
        UnitAgent.SetDestination(destination);
    }

    public virtual void StopAgent()
    {
        m_UnitAgent.velocity = Vector3.zero;
    }

    

    public virtual void UpdateCurrentHp()
    {
        m_UnitCurrentHp = UnitStatisticsSO.HealthMax;
    }
    

    public virtual bool TakeDamage(int Damage)
    {
        Damage = Mathf.Clamp(Damage, 0, UnitStatisticsSO.HealthMax + UnitStatisticsSO.Defence);
        
        if (m_UnitCurrentHp <= Damage)
        {
            m_UnitCurrentHp -= Damage;
            Death();
            return true;
        }
        else
        {
            m_UnitCurrentHp -= Damage;
            return false;
        }
    }

    public virtual void Death()
    {
        Destroy(this.gameObject);
    }


    
}
