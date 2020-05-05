using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HF.Unit
{
    public enum BuildingState
    {
        Positioned,
        Carried,
        NoBehaviourRequire,
    }

    public class BuildingBehaviour : EntityBehavior, ICarriable
    {
        // The view will be go inside the entity behaviour.
        private BuildingView m_view;


        // Handle building state and upate behaviour.
        private BuildingState m_currentState = BuildingState.Positioned;
        private Action m_update;


        // Handle cast on stats when assigned.
        private BuildingsStatsSO m_buildingStats;
        public override EntityStatsSO EntityStats
        {
            get { return m_buildingStats; }
            set { m_buildingStats = (BuildingsStatsSO)value; }
        }


        [Header("Offensive")]

        [SerializeField, Tooltip("Declare the layer that building detect to perform attacks.")]
        private LayerMask m_ostileLayer;

        // Create a detection area to detect ostile.
        private DetectionEntityBehaviour<EntityBehavior> m_detectionArea;
        private const float m_delayBetweenDetections = 1f;
        private float m_timeElapsedbetweenDetections = 0;

        /*
        * Keep tracking the target ostile.
        * When the ostile die, invoke the event that set this value to null.
        */
        private Entity m_focusEntity;
        private ITimer m_simpleTimer;
        private float m_attackRateElapsed;


        [Header("Positioning")]

        [SerializeField, Tooltip("Declare the layers where building can be dropped.")]
        private LayerMask m_droppableLayer;


        private void OnEnable()
        {
            // Subsribe to event --> OnUnitDeath.

            Initialize();
        }

        private void OnDisable()
        {
            // unsubscribe from event --> OnUnitDeath.
        }

        public void Start()
        {
            m_simpleTimer = new SimpleTimer();
        }

        private void Update()
        {
            m_update?.Invoke();
        }

        private void Initialize()
        {
            if (m_buildingStats != null)
            {
                // If it's the castle structure, do not perform any behaviour.
                if (m_buildingStats.BuildingType == Types.BuildingType.CASTLE)
                {
                    SetBuildingState(BuildingState.NoBehaviourRequire);
                    return;
                }

                m_detectionArea = new DetectionEntityBehaviour<EntityBehavior>(5);
                m_timeElapsedbetweenDetections = 0;
                m_focusEntity = null;
            }
        }

        public override void AssignStats(EntityStatsSO inStats)
        {
            base.AssignStats(inStats);

            GameObject tempUnit = ObjectPooler.Instance.GetBuildingObject(m_buildingStats.BuildingType);
            m_view = tempUnit.GetComponent<BuildingView>();
            m_view.gameObject.SetActive(true);
            m_view.transform.position = transform.position - (Vector3.up * m_view.NavMeshObstacle.size.y / 2);
            m_view.transform.SetParent(transform);
        }

        /// <summary>
        /// Handle the logic when change state.
        /// </summary>
        private void SetBuildingState(BuildingState inState)
        {
            m_currentState = inState;

            switch (m_currentState)
            {
                case BuildingState.Positioned:
                    m_update += Attack;
                    //m_update += DetectArea;
                    break;

                case BuildingState.Carried:
                    m_update = null;
                    break;

                default:
                    m_update = null;
                    break;
            }
        }

        /// <summary>
        /// Handle logic and view when carried.
        /// </summary>
        /// <param name="inCarryPosition"></param>
        public void Carry(Vector3 inCarryPosition, out bool success)
        {
            if (m_buildingStats.BuildingType == Types.BuildingType.CASTLE)
            {
                // trigger an UI event when the castle is clicked 
                // that notify the player that can't perform this action with castle.

                success = false;
                return;
            }

            SetBuildingState(BuildingState.Carried);
            transform.position = inCarryPosition + Vector3.up * (m_view.NavMeshObstacle.size.y * 0.5f);

            m_view.CarryBuilding();

            success = true;
        }

        /// <summary>
        /// handle logic and view when dropped.
        /// </summary>
        /// <param name="inDropPosition"></param>
        public void Drop(Vector3 inDropPosition, out bool success)
        {
            Vector3 fixedPosition = inDropPosition + Vector3.up * (m_view.NavMeshObstacle.size.y * 0.5f);
            bool emptyPosition = Physics.CheckBox(fixedPosition * 1.1f, m_view.NavMeshObstacle.size, Quaternion.identity);

            if (emptyPosition)
            {
                RaycastHit hit;
                if (Physics.Raycast(fixedPosition, Vector3.down, out hit, 20f, m_droppableLayer))
                {
                    SetBuildingState(BuildingState.Positioned);
                    transform.up = hit.normal;
                    transform.position = fixedPosition;

                    m_view.DropBuilding();

                    success = true;
                }
            }

            success = false;
        }

        //private void DetectArea()
        //{
        //    if (m_timeElapsedbetweenDetections > m_delayBetweenDetections)
        //    {
        //        // Search ostile if it doens't have one.
        //        if (m_focusEntity == null)
        //        {
        //            // Since the detection area calculate itself the closest ostile, we don't check that.
        //            m_focusEntity = m_detectionArea.DetectFaction(transform, m_buildingStats.AttackRange, m_ostileLayer, gameObject.layer);

        //            // Reset timer.
        //            m_timeElapsedbetweenDetections = 0;
        //        }
        //    }
        //    else
        //    {
        //        m_timeElapsedbetweenDetections += Time.deltaTime;
        //    }
        //}

        public void Attack()
        {
            if (m_focusEntity != null)
            {
                m_view.TrackOstile(m_focusEntity);

                if (m_simpleTimer.Timer(ref m_attackRateElapsed, ref m_buildingStats.AttackSpeed))
                {
                    // instantiate the projectile,
                    /* pool the bullet,
                     * set forward direction,
                     * apply addforce,
                     */

                    // calculate the distance, flight time, cuurent flight time.
                    float distance = Vector3.Distance(m_view.SpawnPoint.position, m_focusEntity.transform.position);
                    //float flightTime = distance / bullet speed.
                    float flightTime = 2; // Temp
                    float currentFlightTime = 0;

                    StartCoroutine(m_simpleTimer.ActionTimer(currentFlightTime, flightTime, DealDamage));

                    // after shoot check if the target is still in range.
                    if (Vector3.Distance(transform.position, m_focusEntity.transform.position) > m_buildingStats.AttackRange)
                    {
                        m_focusEntity = null;
                    }
                }
            }
        }

        private void DealDamage()
        {
            //m_focusEntity.TakeDamage(m_buildingStats.Damage);
        }

        #region 
        /// <summary>
        /// Handle ostile death.
        /// </summary>
        public void OnEnityDead(EntityBehavior entity)
        {
            if (entity == m_focusEntity)
            {
                m_focusEntity = null;
            }
        }
        #endregion
    }
}
