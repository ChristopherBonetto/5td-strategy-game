using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Gets the value from the field specified. Returns success if the field was retrieved.")]
    [TaskCategory("Reflection")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class AssignUnitTarget : Action
    {
        [Tooltip("The GameObject to get the field on")]
        public SharedBuilding troopRef;

        public SharedGameObject focusObjRef;

        public override TaskStatus OnUpdate()
        {
            if (troopRef == null)
            {
                Debug.LogWarning("Unable to get field - field value is null");
                return TaskStatus.Failure;
            }

            if (focusObjRef == null)
            {
                return TaskStatus.Failure;
            }

            EntityBehavior tempEntity = focusObjRef.Value.GetComponent<EntityBehavior>();

            if (tempEntity == null)
            {
                return TaskStatus.Failure;
            }

            troopRef.Value.AssignFocusEntity(tempEntity);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            troopRef = null;
            focusObjRef = null;
        }
    }
}
