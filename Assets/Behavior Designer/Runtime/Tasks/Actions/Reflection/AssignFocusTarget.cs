using UnityEngine;
using System;
using System.Reflection;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Gets the value from the field specified. Returns success if the field was retrieved.")]
    [TaskCategory("Reflection")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class AssignFocusTarget : Action
    {
        [Tooltip("The GameObject to get the field on")]
        public SharedTroop troopRef;

        public SharedEntity focusEntityRef;

        public override TaskStatus OnUpdate()
        {
            if (troopRef == null)
            {
                Debug.LogWarning("Unable to get field - field value is null");
                return TaskStatus.Failure;
            }

            if(focusEntityRef == null)
            {
                return TaskStatus.Failure;
            }

            troopRef.Value.AssignFocusEntity(focusEntityRef.Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            troopRef = null;
            focusEntityRef = null;
        }
    }
}
