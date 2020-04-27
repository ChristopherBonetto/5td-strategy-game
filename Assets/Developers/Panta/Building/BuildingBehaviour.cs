using System.Collections;
using System.Collections.Generic;
using Types;
using UnityEngine;

public class BuildingBehaviour : EntityBehavior, ITakeUpgrade
{
    private BuildingsStatsSO m_unitStats;
    public override EntityStatsSO EntityStats
    {
        get
        {
            return m_unitStats;
        }
        set
        {
            m_unitStats = (BuildingsStatsSO)value;
        }
    }

    #region Detection
    [Header("Detection Field")]

    [SerializeField]
    private LayerMask m_ostileMask;

    private IDetect m_detectionComponent;
    private float m_delayBetweenCheckOstileInRangeElapsed = 0f;
    #endregion


    //-------------------------------------------------------------------------
    // Handle carry infos. The carry action is performed by the troops.
    // The building have to respond to this action changing its scale and postion,
    // also need some check to drop the turret in a new location.
    //-------------------------------------------------------------------------
    #region Carry
    [Header("Carry Field")]

    [SerializeField]
    private Vector3 m_carryUpScale = Vector3.one * 0.5f;
    private Vector3 m_initialScale = Vector3.one;

    [SerializeField]
    private float m_offSetOnY = 1.5f;
    #endregion



    #region MonoBehaviour
    public override void Start()
    {
        base.Start();
        m_detectionComponent = new DetectBehaviors();
    }

    private void Update()
    {
        CheckOstileInRange();
        Attack();
    }
    #endregion



    #region Stats management
    public override void AssignStats(EntityStatsSO inStats)
    {
        base.AssignStats(inStats);
        CreateBuilding(m_unitStats.BuildingType);
    }

    public void CreateBuilding(BuildingType inType)
    {
        GameObject tempUnit = ObjectPooler.SharedInstance.GetBuildingObject(inType);
        tempUnit.SetActive(true);
        tempUnit.transform.SetParent(transform);
    }
    #endregion



    //Esegue il select della truppa
    public override void Select()
    {
        base.Select();

        // Change material or highligth the one it has.
    }


    //-------------------------------------------------------------------------
    // Handle all function used to deal damage or search targets.
    //-------------------------------------------------------------------------
    #region Offensive actions
    public void CheckOstileInRange()
    {
        if (m_delayBetweenCheckOstileInRangeElapsed >= m_unitStats.AttackSpeed)
        {
            // if i'm not attacking anyone, then search in range.
            if (FocusEntity == null)
            {
                FocusEntity = m_detectionComponent.DetectArea(transform, m_unitStats.AttackRange, m_ostileMask);
            }

            m_delayBetweenCheckOstileInRangeElapsed = 0;
        }
        else
        {
            m_delayBetweenCheckOstileInRangeElapsed += Time.deltaTime;
        }
    }

    //TO DO: NEW COMMAND
    public override void Attack()
    {
        if (FocusEntity == null) return;

        if (m_unitStats.CanAttack)
        {
            if (m_attackType.CanAttack(m_unitStats.AttackSpeed))
            {
                // Calculate also the distance to perform, then apply the damage.

                if (m_attackType.SingleAttack(FocusEntity, m_unitStats.Damage))
                {
                    //TO DO: Chiedi alla truppa un altro bersaglio del suo oggetto focus.
                    FocusEntity = null;
                }
            }
        }
    }
    #endregion


    //-------------------------------------------------------------------------
    // Editor visual to help to show infos.
    //-------------------------------------------------------------------------
    #region Editor
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && m_unitStats != null)
        {
            UnityEditor.Handles.color = new Color(1, 0, 0, 0.1f);
            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, m_unitStats.AttackRange);
        }
    }
#endif
    #endregion
}
