using UnityEngine;
using System;
using System.Reflection;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Gets the value from the field specified. Returns success if the field was retrieved.")]
    [TaskCategory("Reflection")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class CheckMyEntityTarget : Action
    {
        [Tooltip("The GameObject to get the field on")]
        public SharedTroop troopRef;

        public SharedEntity focus;

        public override TaskStatus OnUpdate()
        {
            if(troopRef.Value != null && focus != null)
            {
                if (focus.Value.gameObject.activeSelf && !focus.Value.IsBusy)
                {
                    Debug.Log("ok");
                    return TaskStatus.Success;
                }
            }

                    Debug.Log("no");
            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
        }
    }
}
