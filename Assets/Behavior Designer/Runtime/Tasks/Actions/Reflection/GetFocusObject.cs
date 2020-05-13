using UnityEngine;
using System;
using System.Reflection;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Gets the value from the field specified. Returns success if the field was retrieved.")]
    [TaskCategory("Reflection")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class GetFocusObject : Action
    {
        [Tooltip("The GameObject to get the field on")]
        public SharedGameObject targetGameObject;

        public override TaskStatus OnUpdate()
        {
            if (targetGameObject.Value == null)
            {
                return TaskStatus.Failure;
            }


            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
        }
    }
}
