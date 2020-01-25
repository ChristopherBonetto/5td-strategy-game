using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBehaviour : Entity, IDamageable
{
    public Actions m_CurrentEntityAction;

    protected NavMeshAgent m_entityAgent;
    public NavMeshAgent EntityAgent
    {
        get
        {
            if(m_entityAgent != null)
            {
                return m_entityAgent;
            }
            else
            {
                return null;
            }
            
        }
        set
        {
            m_entityAgent = value;
        }
    }


    public bool m_CanAttack = true;

    protected IDamageable CanTakeDamage;

    public GameObject m_FocusObject = null;
    public GameObject FocusObject
    {
        get
        {
            return m_FocusObject;
        }
        set
        {
            m_FocusObject = value;
        }
    }

    protected int m_entityCurrentHp = 10;




    private void Awake()
    {
        NavMeshAgent tempAgent = gameObject.GetComponent<NavMeshAgent>();

        if(tempAgent != null)
        {
            EntityAgent = tempAgent;
        }
        
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        if(EntityAgent != null)
        {
            EntityAgent.speed = EntityAgent.speed + EntityStatisticsSO.MovementSpeed;
        }
        
        
        
        RefreshFullHp();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        ManageState();
        //if (CheckFocussedObjectDistance() && m_CanAttack)
        //{
        //    Attack();
        //}

        //if (!m_CanAttack)
        //{
        //    m_CanAttack = Timer(EntityStatisticsSO.TimeToAttack);
        //}

        //if (FocusObject != null)
        //{
        //    gameObject.transform.LookAt(new Vector3(FocusObject.transform.position.x, gameObject.transform.position.y, FocusObject.transform.position.z));
        //}
    }

    protected virtual void ManageState()
    {
        switch (m_CurrentEntityAction)
        {
            case Actions.Idle:
                break;

            case Actions.Attack:
                if (CheckFocussedObjectDistance() && m_CanAttack)
                {
                    Attack();
                }

                if (!m_CanAttack)
                {
                    m_CanAttack = Timer(EntityStatisticsSO.TimeToAttack);
                }
                break;

            case Actions.Collect:
                break;

            case Actions.Move:
                if (CheckFocussedObjectDistance())
                {
                    m_CurrentEntityAction = Actions.Attack;
                }
                break;

            default:
                break;
        }

        if (FocusObject != null)
        {
            gameObject.transform.LookAt(new Vector3(FocusObject.transform.position.x, gameObject.transform.position.y, FocusObject.transform.position.z));
        }
    }


    #region Utility

    public virtual void ChangeEntityState(Actions NewAction)
    {
        m_CurrentEntityAction = NewAction;
    }

    public virtual void RefreshFullHp()
    {
        m_entityCurrentHp = EntityStatisticsSO.HealthMax;
    }

    #endregion


    #region Attack

    public virtual void Attack()
    {
        ChangeEntityState(Actions.Attack);

        CanTakeDamage = FocusObject.GetComponent<IDamageable>() as IDamageable;

        if (CanTakeDamage != null)
        {
            CanTakeDamage.TakeDamage(EntityStatisticsSO.Attack);
            Debug.Log("attacked");
            m_CanAttack = false;
        }
    }

    #endregion


    #region Movement

    protected virtual bool CheckFocussedObjectDistance()
    {
        if (FocusObject != null)
        {
            if (Vector3.Distance(transform.position, FocusObject.transform.position) <= EntityAgent.stoppingDistance + FocusObject.transform.localScale.x + EntityStatisticsSO.ViewRadius)
            {
                EntityAgent.ResetPath();

                if (EntityAgent.velocity.sqrMagnitude == 0)
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
                if (EntityAgent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    ChangeEntityState(Actions.Move);
                    EntityAgent.SetDestination(FocusObject.transform.position);
                }
            }
        }
        else if (FocusObject == null)
        {

            if (EntityAgent.velocity.sqrMagnitude == 0 && !EntityAgent.pathPending && !EntityAgent.hasPath && m_CurrentEntityAction != Actions.Idle)
            {
                ChangeEntityState(Actions.Idle);
            }
            else
            {

            }
            return false;
        }
        return false;
    }

    public virtual void StopAgent()
    {
        m_entityAgent.velocity = Vector3.zero;
    }

    #endregion


    #region Take Damage

    public virtual bool TakeDamage(int Damage)
    {
        Damage = Mathf.Clamp(Damage, 0, EntityStatisticsSO.HealthMax + EntityStatisticsSO.Defence);
        Debug.Log(transform.name + "damaged");
        if (m_entityCurrentHp <= Damage)
        {
            m_entityCurrentHp -= Damage;
            Death();
            return true;
        }
        else
        {
            m_entityCurrentHp -= Damage;
            return false;
        }
    }

    public virtual void Death()
    {
        Destroy(this.gameObject);
    }

    #endregion
}
