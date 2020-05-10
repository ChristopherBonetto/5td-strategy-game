using UnityEngine;
using System;
using System.Reflection;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Gets the value from the field specified. Returns success if the field was retrieved.")]
    [TaskCategory("Reflection")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class CheckUnitTarget : Action
    {
        [Tooltip("The GameObject to get the field on")]
        public SharedGameObject targetGameObject;

        private Unit unitRef;

        public override TaskStatus OnUpdate()
        {
            if (targetGameObject == null || !targetGameObject.Value.activeInHierarchy)
            {
                Debug.LogWarning("Unable to get field - field value is null");
                return TaskStatus.Failure;
            }

            unitRef = targetGameObject.Value.GetComponent<Unit>();
            if (unitRef == null)
            {
                Debug.LogWarning("Unable to get field - type is null");
                return TaskStatus.Failure;
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            unitRef = null;
        }
    }
}
