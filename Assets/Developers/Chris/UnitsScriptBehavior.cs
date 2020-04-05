using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;



public class UnitsScriptBehavior : MonoBehaviour, IDamageable
{
    [SerializeField] private Units m_startUnitsInfo;

    [SerializeField] private int m_checkAreaRadius;

    public Actions m_CurrentUnitAction { get; protected set; }
    
    private UnitStatistics m_UnitStatisticsSO;
    public UnitStatistics UnitStatisticsSO
    {
        get
        {
            return m_UnitStatisticsSO;
        }
        set
        {
            m_UnitStatisticsSO = value;
        }
    }
    
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

    private float m_Timer = 0f;
    protected bool m_CanAttack = true;

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
            if (gameObject.layer != PlayerInfoBehavior.Instance.m_playerLayer)
            {
                FocusObject = PlayerInfoBehavior.Instance.CastlePosition;
            }
        }
    }

    public virtual void Initialize()
    {
        UnitStatisticsSO = PlayerInfoBehavior.Instance.PlayerInfoSO.PlayerUnitsDictionary[m_startUnitsInfo].UnitStatsCopy;

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

    public virtual bool Timer(float destinationTime)
    {
        m_Timer += Time.deltaTime;

        if (m_Timer >= destinationTime)
        {
            m_Timer = 0f;
            return true;
        }
        else
        {
            return false;
        }
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


    #region Check units near me
    void CheckNearUnit()
    {
        

        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_checkAreaRadius); // must be integrate layer 
        //for (int i = 0; i < hitColliders.Length; i++)
        //{
        //    if (hitColliders[i].GetComponent<EnemySoldier>())
        //    {
        //        Debug.Log(hitColliders[i].name);
        //        hitColliders[i].GetComponent<EnemySoldier>().PlayerDetected(true);
        //        hitColliders[i].GetComponent<EnemySoldier>().MoveOnTargetDestination(MG_PlayerPosition);
        //        hitColliders[i].GetComponent<EnemySoldier>().ChangeSoldierState(SoldierStates.Allert);

        //    }
        //}
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_checkAreaRadius);
    }
    #endregion
}
