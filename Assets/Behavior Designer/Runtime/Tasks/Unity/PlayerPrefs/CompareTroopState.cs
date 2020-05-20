using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs
{
    [TaskCategory("Unity/PlayerPrefs")]
    public class CompareTroopState : Action
    {
        [RequiredField]
        [Tooltip("The wanted state")]
        public TroopStates wantedState;

        [Tooltip("Troop ref to compare his state")]
        [RequiredField]
        public SharedTroop troopRef;

        public override TaskStatus OnUpdate()
        {
            if(wantedState == troopRef.Value.CurrentTroopState)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }

        public override void OnReset()
        {
            troopRef = null;
        }
    }
}
