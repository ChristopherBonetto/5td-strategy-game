using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Seek the target specified using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class NavMeshTroopMovement02 : NavMeshMovement
    {
        [Tooltip("The GameObject that the agent is seeking")]
        public SharedEntity targetEntity;
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
            if (targetEntity.Value != null)
            {
                return targetEntity.Value.transform.position;
            }
            return targetPosition.Value;
        }

        public override void OnReset()
        {
            base.OnReset();
            targetEntity = null;
            targetPosition = Vector3.zero;
        }

        protected override bool SetDestination(Vector3 destination)
        {
            if (base.SetDestination(destination))
            {
                for(int i = 0; i < troopRef.UnitList.Count; i++)
                {
                    navMeshAgent.speed = speed.Value;
                    troopRef.UnitList[i].UnitAgent.speed = speed.Value;
                    Vector3 unitDestin = navMeshAgent.pathEndPosition + troopRef.m_formationPosition[i];
                    troopRef.UnitList[i].UnitAgent.SetDestination(unitDestin);
                }
                return true;
            }
            return false;
        }
    }
}
