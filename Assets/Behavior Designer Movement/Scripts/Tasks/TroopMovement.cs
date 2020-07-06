using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Seek the target specified using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class TroopMovement : NavMeshMovement
    {
        [Tooltip("If target is null then use the target position")]
        public SharedVector3 targetPosition;

        private Troop troopRef;

        public override void OnAwake()
        {
            base.OnAwake();
            troopRef = GetComponent<Troop>();
        }

        public override void OnStart()
        {
            base.OnStart();

            if (!SetDestination(Target()))
            {
                Debug.LogError("ERROR IN ALLY MOVEMENT : Can't find point");
                navMeshAgent.isStopped = true;
            }
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            if (HasArrived())
            {
                return TaskStatus.Success;
            }

            if (!SetDestination(Target()))
            {
                Debug.LogError("ERROR IN ALLY MOVEMENT : Can't find point");
                navMeshAgent.isStopped = true;
                return TaskStatus.Failure;
            }

            return TaskStatus.Running;
        }

        // Return targetPosition if target is null
        private Vector3 Target()
        {
            if (troopRef.FocusEntity != null)
            {
                return troopRef.FocusEntity.transform.position;
            }
            return targetPosition.Value;
        }

        public override void OnReset()
        {
            base.OnReset();
            targetPosition = Vector3.zero;
        }

        protected override bool SetDestination(Vector3 destination)
        {
            if (base.SetDestination(destination))
            {
                if(navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    return false;
                }

                navMeshAgent.speed = speed.Value;

                for (int i = 0; i < troopRef.UnitList.Count; i++)
                {
                    NavMeshAgent unit = troopRef.UnitList[i].UnitAgent;

                    if (unit.isActiveAndEnabled && unit.isOnNavMesh)
                    {
                        if (Vector3.Distance(unit.transform.position, navMeshAgent.transform.position) > 3f)
                        {
                            unit.speed = speed.Value + 2;
                            Vector3 unitDestin = navMeshAgent.transform.position + troopRef.m_formationPosition[i];
                            unit.SetDestination(unitDestin);
                        }
                        else
                        {
                            unit.speed = speed.Value;
                            Vector3 unitDestin = navMeshAgent.pathEndPosition + troopRef.m_formationPosition[i];
                            unit.SetDestination(unitDestin);
                        }
                    }
                }
                return true;
            }
            return false;
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

            if(troopRef.FocusEntity != null)
            {
                float dist = Vector3.Distance(navMeshAgent.transform.position, troopRef.FocusEntity.transform.position);

                if (remainingDistance == 0)
                {
                    remainingDistance = float.PositiveInfinity;
                }

                if(dist <= arriveDistance.Value)
                {
                    return true;
                }
            }
            else
            {
                float dist = Vector3.Distance(navMeshAgent.transform.position, navMeshAgent.pathEndPosition);

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
