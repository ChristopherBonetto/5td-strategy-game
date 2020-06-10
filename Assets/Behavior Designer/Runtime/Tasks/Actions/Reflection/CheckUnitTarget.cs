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
        public SharedUnit unitRef;

        public override TaskStatus OnUpdate()
        {
            if (unitRef.Value.FocusUnit == null && unitRef.Value.FocusBuilding == null)
            {
                Debug.Log("try to catch another");
                return TaskStatus.Failure;
            }


            if (unitRef.Value.FocusUnit != null && !unitRef.Value.FocusUnit.gameObject.activeSelf || 
                unitRef.Value.FocusBuilding != null && !unitRef.Value.FocusBuilding.gameObject.activeSelf)
            {
                unitRef.Value.AssignFocusToUnit((Unit)null);
                return TaskStatus.Failure;
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            unitRef = null;
        }
    }
}
