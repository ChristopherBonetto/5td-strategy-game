using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public abstract class NavMeshTroopMovement : GroupMovement
    {
        public SharedTroop troopRef;

        [Tooltip("The angular speed of the agents")]
        public SharedFloat angularSpeed = 120;

        // A cache of the NavMeshAgents
        protected NavMeshAgent[] navMeshAgents;
        protected Transform[] transforms;

        public override void OnStart()
        {
            navMeshAgents = new NavMeshAgent[troopRef.Value.UnitList.Count];
            transforms = new Transform[troopRef.Value.UnitList.Count];
            troopRef.Value.Agent.enabled = true;

            for (int i = 0; i < troopRef.Value.UnitList.Count; ++i)
            {
                transforms[i] = troopRef.Value.UnitList[i].transform;
                navMeshAgents[i] = troopRef.Value.UnitList[i].GetComponent<NavMeshAgent>();
                navMeshAgents[i].enabled = true;
                navMeshAgents[i].speed = troopRef.Value.GetStats().UnitSpeed;
                navMeshAgents[i].angularSpeed = angularSpeed.Value;
                navMeshAgents[i].isStopped = false;
            }
        }

        protected override bool SetDestination(int index, Vector3 target)
        {
            if (!navMeshAgents[index].gameObject.activeInHierarchy)
            {
                return false;
            }
            if (navMeshAgents[index].destination == target)
            {
                return true;
            }

            return navMeshAgents[index].SetDestination(target);
        }

        protected override Vector3 Velocity(int index)
        {
            return navMeshAgents[index].velocity;
        }

        public override void OnEnd()
        {
            // Disable the nav mesh
            for (int i = 0; i < navMeshAgents.Length; ++i)
            {
                if (navMeshAgents[i] != null)
                {
                    if (navMeshAgents[i].gameObject.activeInHierarchy)
                    {
                        navMeshAgents[i].isStopped = true;
                    }
                }
            }
        }

        // Reset the public variables
        public override void OnReset()
        {
            troopRef = null;
        }
    }
}
