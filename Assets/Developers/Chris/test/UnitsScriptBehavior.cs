using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;



public class UnitsScriptBehavior : Entity, IDamageable
{
    protected NavMeshAgent m_unitAgent;
    public NavMeshAgent UnitAgent
    {
        get
        {
            return m_unitAgent;
        }
        set
        {
            m_unitAgent = value;
        }
    }

    protected IDamageable CanTakeDamage;

    protected GameObject m_focusObject = null;
    public GameObject FocusObject
    {
        get { return m_focusObject; }
        protected set { m_focusObject = value; }
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
        InitializeStats();
    }


    public virtual void Update()
    {
        CheckAndAttackFocusObj();
        ReloadWeapon();
    }

    public virtual void ChangeBehaviorOnPlayerType()
    {

    }

    public virtual void InitializeStats()
    {
        UnitAgent.speed = UnitAgent.speed + EntityStatisticsSO.MovementSpeed;
        ResetHp();
    }

    public void CheckAndAttackFocusObj()
    {
        if (CheckFocussedObjectDistance() && m_CanAttack)
        {
            Attack();
        }
        if (FocusObject != null)
        {
            gameObject.transform.LookAt(new Vector3(FocusObject.transform.position.x, gameObject.transform.position.y, FocusObject.transform.position.z));
        }
    }

    public void ReloadWeapon()
    {
        if (!m_CanAttack)
        {
            m_CanAttack = Timer(EntityStatisticsSO.AttackSpeed);
        }
    }
    
    public virtual void Attack()
    {
        ChangeAction(Actions.Attack);
        
        CanTakeDamage = FocusObject.GetComponent<IDamageable>() as IDamageable;
        if (CanTakeDamage != null)
        {
            CanTakeDamage.TakeDamage(EntityStatisticsSO.Attack);
            m_CanAttack = false;
        }
    }

    protected virtual bool CheckFocussedObjectDistance()
    {
        if (FocusObject != null)
        {
            if (Vector3.Distance(transform.position, FocusObject.transform.position) <= m_unitAgent.stoppingDistance + FocusObject.transform.localScale.x + EntityStatisticsSO.Range)
            {
                m_unitAgent.ResetPath();

                if (m_unitAgent.velocity.sqrMagnitude == 0)
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
                if(m_unitAgent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    ChangeAction(Actions.Move);
                    m_unitAgent.SetDestination(FocusObject.transform.position);
                }
            }
        }
        else if(FocusObject == null)
        {
            
            if (m_unitAgent.velocity.sqrMagnitude == 0 && !m_unitAgent.pathPending && !m_unitAgent.hasPath && m_CurrentUnitAction != Actions.Idle)
            {
                ChangeAction(Actions.Idle);
            }
            else
            {

            }
            return false;
        }
        return false;
    }

    public virtual void AssignFocusObj(GameObject inObj)
    {
        FocusObject = inObj;
    }

    public virtual void SetDestination(Vector3 destination)
    {
        UnitAgent.SetDestination(destination);
    }

    public virtual void StopAgent()
    {
        m_unitAgent.velocity = Vector3.zero;
    }

    

    public virtual void ResetHp()
    {
        m_UnitCurrentHp = EntityStatisticsSO.HealthMax;
    }
    

    public virtual bool TakeDamage(int Damage)
    {
        Damage = Mathf.Clamp(Damage, 0, m_UnitCurrentHp);
        
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
