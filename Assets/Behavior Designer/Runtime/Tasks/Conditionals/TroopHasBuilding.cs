using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
    [TaskCategory("Unity/GameObject")]
    [TaskDescription("Returns the component of Type type if the game object has one attached, null if it doesn't. Returns Success.")]
    public class TroopHasBuilding : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedTroop troopRef;

        public bool wantedValue;

        public override TaskStatus OnUpdate()
        {
            if(troopRef.Value == null)
            {
                return TaskStatus.Failure;
            }

            if (troopRef.Value.BuildingHandled == null && wantedValue == false)
            {
                return TaskStatus.Success;
            }
            else if(troopRef.Value.BuildingHandled != null && wantedValue == true)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
            troopRef = null;
        }
    }
}
