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

        public override void OnStart()
        {
            base.OnStart();

            SetDestination(Target());
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            if (HasArrived())
            {
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
            return targetPosition.Value;
        }

        public override void OnReset()
        {
            base.OnReset();
            unitRef = null;
            targetPosition = Vector3.zero;
        }
    }
}
