using UnityEngine;
using System;
using System.Reflection;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Gets the value from the field specified. Returns success if the field was retrieved.")]
    [TaskCategory("Reflection")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class CheckMyTarget : Action
    {
        [Tooltip("The GameObject to get the field on")]
        public SharedGameObject targetGameObject;

        private EntityBehavior entityRef;

        public override TaskStatus OnUpdate()
        {
            if (targetGameObject == null || !targetGameObject.Value.active)
            {
                Debug.LogWarning("Unable to get field - field value is null");
                return TaskStatus.Failure;
            }

            entityRef = targetGameObject.Value.GetComponent<EntityBehavior>();
            if (entityRef == null)
            {
                Debug.LogWarning("Unable to get field - type is null");
                return TaskStatus.Failure;
            }

            if (entityRef.IsBusy)
            {
                return TaskStatus.Failure;
            }

            return TaskStatus.Running;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            entityRef = null;
        }
    }
}
