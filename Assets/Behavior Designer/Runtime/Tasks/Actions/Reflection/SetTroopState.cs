using UnityEngine;
using System;
using System.Reflection;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Gets the value from the field specified. Returns success if the field was retrieved.")]
    [TaskCategory("Reflection")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class SetTroopState : Action
    {
        [Tooltip("The Troop to get the field on")]
        public SharedTroop targetTroop;

        [Tooltip("The GameObject to get the field on")]
        public TroopStates wantedState;

        public override void OnStart()
        {
            targetTroop.Value.SetNewTroopState(wantedState);
        }

        public override TaskStatus OnUpdate()
        {
            if (targetTroop.Value.CurrentTroopState == wantedState)
            {
                return TaskStatus.Success;
            }


            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
            targetTroop = null;
        }
    }
}
