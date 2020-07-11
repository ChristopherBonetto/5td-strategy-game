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
        private Unit unitRef;

        public override void OnAwake()
        {
            base.OnAwake();

            unitRef = GetComponent<Unit>();
        }
        public override TaskStatus OnUpdate()
        {
            if (unitRef.FocusUnit == null && unitRef.FocusBuilding == null)
            {
                //Debug.Log("try to catch another");
                return TaskStatus.Failure;
            }


            if (unitRef.FocusUnit != null && !unitRef.FocusUnit.gameObject.activeSelf || 
                unitRef.FocusBuilding != null && !unitRef.FocusBuilding.gameObject.activeSelf)
            {
                unitRef.AssignFocusToUnit((Unit)null);
                return TaskStatus.Failure;
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            //unitRef = null;
        }
    }
}
