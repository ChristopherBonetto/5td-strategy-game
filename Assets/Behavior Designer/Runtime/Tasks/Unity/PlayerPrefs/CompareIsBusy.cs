using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs
{
    [TaskCategory("Unity/PlayerPrefs")]
    public class CompareIsBusy : Action
    {
        [RequiredField]
        [Tooltip("The wanted state")]
        public bool wantedState;

        [Tooltip("Troop ref to compare his state")]
        [RequiredField]
        public EntityBehavior entityRef;

        public override TaskStatus OnUpdate()
        {
            if (entityRef.IsBusy == wantedState)
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
            entityRef = null;
        }
    }
}