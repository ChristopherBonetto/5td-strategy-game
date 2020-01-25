using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitActions : Entity, IDamageable
{
    public Actions m_CurrentUnitAction;

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

    protected int m_unitCurrentHp = 10;




    private void Awake()
    {
        NavMeshAgent tempAgent = gameObject.GetComponent<NavMeshAgent>();

        if(tempAgent != null)
        {
            UnitAgent = tempAgent;
        }
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        if(UnitAgent != null)
        {
            UnitAgent.speed = UnitAgent.speed + EntityStatisticsSO.MovementSpeed;
        }
        RefreshFullHp();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        ManageState();
    }

    protected virtual void ManageState()
    {
        switch (m_CurrentUnitAction)
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
                    m_CurrentUnitAction = Actions.Attack;
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

    public virtual void ChangeUnitState(Actions NewAction)
    {
        m_CurrentUnitAction = NewAction;
    }

    public virtual void RefreshFullHp()
    {
        m_unitCurrentHp = EntityStatisticsSO.HealthMax;
    }

    #endregion


    #region Attack

    public virtual void Attack()
    {
        ChangeUnitState(Actions.Attack);

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
            if (Vector3.Distance(transform.position, FocusObject.transform.position) <= UnitAgent.stoppingDistance + FocusObject.transform.localScale.x + EntityStatisticsSO.ViewRadius)
            {
                UnitAgent.ResetPath();

                if (UnitAgent.velocity.sqrMagnitude == 0)
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
                if (UnitAgent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    ChangeUnitState(Actions.Move);
                    UnitAgent.SetDestination(FocusObject.transform.position);
                }
            }
        }
        else if (FocusObject == null)
        {

            if (UnitAgent.velocity.sqrMagnitude == 0 && !UnitAgent.pathPending && !UnitAgent.hasPath && m_CurrentUnitAction != Actions.Idle)
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

    public virtual void StopAgent()
    {
        m_unitAgent.velocity = Vector3.zero;
    }

    #endregion


    #region Take Damage

    public virtual bool TakeDamage(int Damage)
    {
        Damage = Mathf.Clamp(Damage, 0, EntityStatisticsSO.HealthMax + EntityStatisticsSO.Defence);
        Debug.Log(transform.name + "damaged");
        if (m_unitCurrentHp <= Damage)
        {
            m_unitCurrentHp -= Damage;
            Death();
            return true;
        }
        else
        {
            m_unitCurrentHp -= Damage;
            return false;
        }
    }

    public virtual void Death()
    {
        Destroy(this.gameObject);
    }

    #endregion
}
