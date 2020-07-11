using DG.Tweening;
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
        private Unit unitRef;
        [Tooltip("If target is null then use the target position")]
        public SharedVector3 targetPosition;

        public override void OnAwake()
        {
            base.OnAwake();
            unitRef = gameObject.GetComponent<Unit>();
        }

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
                if(unitRef.FocusBuilding != null)
                {
                    unitRef.transform.LookAt(unitRef.FocusBuilding.transform.position);
                }
                else if (unitRef.FocusUnit != null)
                {
                    unitRef.transform.LookAt(unitRef.FocusUnit.transform.position);
                }

                return TaskStatus.Success;
            }

            SetDestination(Target());

            return TaskStatus.Running;
        }

        // Return targetPosition if target is null
        private Vector3 Target()
        {
            if (unitRef.FocusUnit != null)
            {
                return unitRef.FocusUnit.transform.position;
            }
            else if (unitRef.FocusBuilding != null)
            {
                return unitRef.UnitFormationPos;
            }
            return targetPosition.Value;
        }

        public override void OnReset()
        {
            base.OnReset();
            //unitRef = null;
            targetPosition = Vector3.zero;
        }

        protected override bool HasArrived()
        {
            // The path hasn't been computed yet if the path is pending.
            float remainingDistance;
            if (navMeshAgent.pathPending)
            {
                remainingDistance = float.PositiveInfinity;
            }
            else
            {
                remainingDistance = navMeshAgent.remainingDistance;
            }

            Transform tempFocus = null;

            if(unitRef.FocusUnit != null)
            {
                tempFocus = unitRef.FocusUnit.transform;
            }
            else if(unitRef.FocusBuilding != null)
            {
                tempFocus = unitRef.FocusBuilding.transform;
            }

            if (tempFocus != null)
            {
                float dist = Vector3.Distance(navMeshAgent.transform.position, tempFocus.position);

                if (remainingDistance == 0)
                {
                    remainingDistance = float.PositiveInfinity;
                }

                if (dist <= arriveDistance.Value)
                {
                    return true;
                }
            }
            return remainingDistance <= arriveDistance.Value;
        }
    }
}
