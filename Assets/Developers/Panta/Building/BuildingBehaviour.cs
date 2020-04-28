using System.Collections;
using System.Collections.Generic;
using Types;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This class' purpose is to controll the building's view and model.
/// </summary>
[RequireComponent(typeof(NavMeshObstacle))]
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

    private Collider m_Collider;
    public Collider Collider => m_Collider;

    #region Detection variables
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
    #region Carry variables
    [Header("Carry Field")]

    [SerializeField]
    private Vector3 m_carryScale = Vector3.one * 0.5f;
    private Vector3 m_initialScale = Vector3.one;

    private NavMeshObstacle m_navMeshObstacle;
    #endregion


    #region MonoBehaviour
    private void Awake()
    {
        m_navMeshObstacle = GetComponent<NavMeshObstacle>();
        m_Collider = GetComponent<Collider>();
    }

    public override void Start()
    {
        base.Start();

        m_initialScale = this.transform.localScale;
        m_detectionComponent = new DetectBehaviors();
    }

    private void Update()
    {
        CheckOstileInRange();
        Attack();
    }
    #endregion

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

    //Esegue il select della truppa
    public override void Select()
    {
        base.Select();

        // Change material or highligth the one it has.
    }

    #region Carry actions
    /// <summary>
    /// This method is performed by the troop when perform successfully carry a tower.
    /// </summary>
    public void Carry(TroopBehavior troop, Vector3 inCarryPosition)
    {
        // Can attack = false
        // change its scale,
        // move it on top of the units.
        // Set this transform as parent of the unit.
        // Deactivate navmesh obstacle.

        m_unitStats.CanAttack = false;
        m_navMeshObstacle.enabled = false;
        this.transform.SetParent(troop.transform);
        StartCoroutine(UpdatePosition(inCarryPosition));
    }

    /// <summary>
    /// This method is performed by the troop when perform successfully drop the tower.
    /// </summary>
    public void Drop(TroopBehavior troop, Vector3 inDropPosition)
    {
        // Can attack = true
        // restore its original scale,
        // drop onto terrain.
        // Set this transform parent = null
        // Activate navmesh obstalce.

        m_unitStats.CanAttack = true;
        m_navMeshObstacle.enabled = true;
        this.transform.SetParent(null);
        StartCoroutine(UpdatePosition(inDropPosition));
    }

    IEnumerator UpdatePosition(Vector3 inTargetPosition)
    {
        float totalTimeToElaps = 0.5f;
        float elapsedTime = 0f;
        Vector3 startingPos = transform.position;

        while (elapsedTime < totalTimeToElaps)
        {
            transform.position = Vector3.Slerp(startingPos, inTargetPosition, (elapsedTime / totalTimeToElaps));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    #endregion

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
