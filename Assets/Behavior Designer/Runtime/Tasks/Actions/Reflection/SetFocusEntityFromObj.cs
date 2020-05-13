using UnityEngine;
using System;
using System.Reflection;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Sets the field to the value specified. Returns success if the field was set.")]
    [TaskCategory("Reflection")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class SetFocusEntityFromObj : Action
    {
        [Tooltip("The GameObject to set the field on")]
        public SharedGameObject targetGameObject;
        public SharedTroop troopRef;

        public override TaskStatus OnUpdate()
        {
            if (troopRef == null)
            {
                Debug.LogWarning("Unable to get field - field value is null");
                return TaskStatus.Failure;
            }

            if (targetGameObject == null)
            {
                Debug.LogWarning("Unable to set field - type is null");
                return TaskStatus.Failure;
            }

            EntityBehavior focusEntity = targetGameObject.Value.GetComponent<EntityBehavior>();
            if (focusEntity == null)
            {
                Debug.LogWarning("Unable to set the field with component ");
                return TaskStatus.Failure;
            }

            troopRef.Value.FocusEntity = focusEntity;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            troopRef = null;
        }
    }
}
