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
        [SerializeField] private float m_delayFollowCommand;
        private float m_timer = 0;

        // The agents will always be following the leader so always return running
        public override TaskStatus OnUpdate()
        {
            for (int i = 0; i < navMeshAgents.Count; ++i)
            {
                SetDestination(i, troopRef.Value.transform.position + troopRef.Value.m_formationPosition[i]);
                if (Timer(m_delayFollowCommand))
                {
                    
                }
            }
            return TaskStatus.Running;
        }

        private bool Timer(float destinationTime)
        {
            m_timer += Time.deltaTime;

            if (m_timer >= destinationTime)
            {
                m_timer = 0f;

                return true;
            }
            else
            {
                return false;
            }
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();
        }

    }
}
