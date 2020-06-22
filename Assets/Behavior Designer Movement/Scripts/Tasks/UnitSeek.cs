using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Seek the target specified using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class UnitSeek : NavMeshMovement
    {
        [Tooltip("The GameObject that the agent is seeking")]
        public SharedUnit unitRef;
        [Tooltip("If target is null then use the target position")]
        public SharedVector3 targetPosition;

        private float m_arriveDistStartValue;

        public override void OnStart()
        {
            base.OnStart();

            m_arriveDistStartValue = arriveDistance.Value;

            SetDestination(Target());
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            if (HasArrived())
            {
                arriveDistance = m_arriveDistStartValue;

                if(unitRef.Value.FocusBuilding != null)
                {
                    unitRef.Value.transform.LookAt(unitRef.Value.FocusBuilding.transform.position);
                }
                else if (unitRef.Value.FocusUnit != null)
                {
                    unitRef.Value.transform.LookAt(unitRef.Value.FocusUnit.transform.position);
                }

                return TaskStatus.Success;
            }

            SetDestination(Target());

            return TaskStatus.Running;
        }

        // Return targetPosition if target is null
        private Vector3 Target()
        {
            if (unitRef.Value.FocusUnit != null)
            {
                return unitRef.Value.FocusUnit.transform.position;
            }
            else if (unitRef.Value.FocusBuilding != null)
            {
                arriveDistance = 0.1f;
                return unitRef.Value.UnitFormationPos;
            }
            return targetPosition.Value;
        }

        public override void OnReset()
        {
            base.OnReset();
            arriveDistance = m_arriveDistStartValue;
            unitRef = null;
            targetPosition = Vector3.zero;
        }
    }
}
