using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityNavMeshAgent
{
    [TaskCategory("Unity/NavMeshAgent")]
    public class GetCaptainIsMoving : Action
    {
        public SharedTroopClass TroopRef;
        private Unit Captain;

        private float CaptainSpeed;

        public override void OnStart()
        {
            Captain = TroopRef.Value.Captain;
        }

        public override TaskStatus OnUpdate()
        {
            if (Captain == null)
            {
                Debug.LogWarning("Captain is null");
                return TaskStatus.Failure;
            }

            //CaptainSpeed = Captain.UnitAgent.velocity.sqrMagnitude;

            if(Captain.UnitAgent.hasPath && Captain.UnitAgent.velocity != Vector3.zero)
            {
                Debug.Log("ok");
                return TaskStatus.Success;
            }
            else
            {
                Debug.Log("no");
                return TaskStatus.Failure;
            }
        }

        public override void OnReset()
        {
            TroopRef = null;
            CaptainSpeed = 0;
        }
    }
}
