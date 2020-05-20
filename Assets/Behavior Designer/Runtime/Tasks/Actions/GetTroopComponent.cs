using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
    [TaskCategory("Unity/GameObject")]
    [TaskDescription("Returns the component of Type type if the game object has one attached, null if it doesn't. Returns Success.")]
    public class GetTroopComponent : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;

        public SharedTroop storeValue;

        public override TaskStatus OnUpdate()
        {
            storeValue.Value = targetGameObject.Value.GetComponent<Troop>();

            if(storeValue.Value != null)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue.Value = null;
        }
    }
}
