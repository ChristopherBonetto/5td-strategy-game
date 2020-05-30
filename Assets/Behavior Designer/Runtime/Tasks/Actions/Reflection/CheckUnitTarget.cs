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
            if (unitRef.Value.m_focusUnit == null && unitRef.Value.m_focusBuilding == null)
            {
                Debug.Log("try to catch another");
                return TaskStatus.Failure;
            }


            if (unitRef.Value.m_focusUnit != null && !unitRef.Value.m_focusUnit.gameObject.activeSelf || 
                unitRef.Value.m_focusBuilding != null && !unitRef.Value.m_focusBuilding.gameObject.activeSelf)
            {
                unitRef.Value.AssignFocusUnit(null);
                unitRef.Value.AssignFocusBuilding(null);
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
