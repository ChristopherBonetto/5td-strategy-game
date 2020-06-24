using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Seek the target specified using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class SeekCastle : NavMeshMovement
    {
        [Tooltip("The GameObject that the agent is seeking")]
        public SharedBuilding target;
        [Tooltip("If target is null then use the target position")]
        public SharedVector3 targetPosition;

        private NavMeshObstacle targetObstacle;

        public override void OnStart()
        {
            targetObstacle = target.Value.GetComponent<NavMeshObstacle>();

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

            //SetDestination(Target());

            return TaskStatus.Running;
        }

        // Return targetPosition if target is null
        private Vector3 Target()
        {
            if (target.Value != null)
            {
                Vector3? destination = GameController.Instance.RandomPoint(target.Value.transform.position, targetObstacle.size.x * 1.5f, target.Value.transform.position);
                return destination.Value;
                //return target.Value.transform.position;
            }
            return targetPosition.Value;
        }

        public override void OnReset()
        {
            base.OnReset();
            target = null;
            targetPosition = Vector3.zero;
        }
    }
}
