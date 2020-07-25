using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Seek the target specified using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class GoToCastle : NavMeshMovement
    {
        [Tooltip("If target is null then use the target position")]
        public SharedVector3 targetPosition;

        [Tooltip("d divided the distance, so it represents the rateo in wich the speed growth")]
        public SharedFloat d = 2f;
        [Tooltip("how faster the speed growth")]
        public SharedFloat Growth = 50f;


        private Troop troopRef;

        public override void OnAwake()
        {
            base.OnAwake();
            troopRef = GetComponent<Troop>();
        }

        public override void OnStart()
        {
            base.OnStart();

            if (Target() != null && SetDestination(Target().Value))
            {
            }
            else
            {
                Debug.LogError("Enemy troop can't go to the castle because path is invalid or castle engange is not setted");
                troopRef.Death();
            }
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            if (HasArrived())
            {
                troopRef.FocusEntity = troopRef.TargetCastle;
                return TaskStatus.Success;
            }

            if (Target() != null && SetDestination(Target().Value)) { }
            else 
            {
                Debug.LogError("Enemy troop can't go to the castle because path is invalid or castle engange is not setted");
                troopRef.Death();
                return TaskStatus.Failure;
            }
            return TaskStatus.Running;
        }

        // Return targetPosition if target is null
        private Vector3? Target()
        {
            if (troopRef.EngagePointForCastle != null && troopRef.TargetCastle != null)
            {
                return troopRef.EngagePointForCastle;
            }
            Debug.Log("Point to castle not assigned");
            return null;
        }

        public override void OnReset()
        {
            base.OnReset();
            targetPosition = Vector3.zero;
        }

        protected override bool SetDestination(Vector3 destination)
        {
            if(troopRef.TargetCastle == null || targetPosition == null)
            {
                return false;
            }

            if (base.SetDestination(destination))
            {
                if(navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial || navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    return false;
                }

                navMeshAgent.speed = speed.Value;

                for (int i = 0; i < troopRef.UnitList.Count; i++)
                {
                    NavMeshAgent unit = troopRef.UnitList[i].UnitAgent;

                    if (unit.isActiveAndEnabled && unit.isOnNavMesh)
                    {
                        unit.ResetPath();

                        // Set destination
                        Vector3 unitDestin = navMeshAgent.transform.position + troopRef.m_formationPosition[i];

                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(unitDestin, out hit, 1.0f, NavMesh.AllAreas))
                        {
                            unit.SetDestination(hit.position);
                        }
                        else
                        {
                            unit.SetDestination(navMeshAgent.transform.position);
                        }

                        // Define the speed
                        float distance = Vector3.Distance(unit.transform.position, unitDestin);
                        unit.speed = speed.Value + (distance / d.Value * (speed.Value * Growth.Value / 100f));
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

            if (troopRef.TargetCastle != null && troopRef.EngagePointForCastle != null)
            {
                float dist = Vector3.Distance(navMeshAgent.transform.position, troopRef.TargetCastle.transform.position);

                if (remainingDistance == 0 && dist <= arriveDistance.Value)
                {
                    return true;
                }
            }


            return remainingDistance <= arriveDistance.Value;
        }
    }
}
