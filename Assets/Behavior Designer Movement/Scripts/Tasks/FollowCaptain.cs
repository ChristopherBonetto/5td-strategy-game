using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Follow the leader using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}LeaderFollowIcon.png")]
    public class FollowCaptain : NavMeshTroopMovement
    {

        // The agents will always be following the leader so always return running
        public override TaskStatus OnUpdate()
        {
            for (int i = 0; i < navMeshAgents.Count; ++i)
            {
                if (navMeshAgents[i].gameObject.activeSelf)
                {
                    navMeshAgents[i].enabled = true;
                    navMeshAgents[i].isStopped = false;
                    navMeshAgents[i].ResetPath();
                    SetDestination(i, troopRef.Value.transform.position + troopRef.Value.m_formationPosition[i]);
                    //SetDestination(i, troopRef.Value.Agent.destination + troopRef.Value.m_formationPosition[i]);
                }
            }
            return TaskStatus.Running;
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();
        }

    }
}
