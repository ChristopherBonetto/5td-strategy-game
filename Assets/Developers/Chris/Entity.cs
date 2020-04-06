using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{

    private EntityStatistics m_UnitStatisticsSO;
    public EntityStatistics UnitStatisticsSO
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

    [SerializeField] private int m_checkAreaRadius;

    private float m_Timer = 0f;
    protected bool m_CanAttack = true;


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
