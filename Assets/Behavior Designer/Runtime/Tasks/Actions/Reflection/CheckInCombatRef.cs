using UnityEngine;
using System;
using System.Reflection;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Gets the value from the field specified. Returns success if the field was retrieved.")]
    [TaskCategory("Reflection")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class CheckInCombatRef : Action
    {
        [Tooltip("The GameObject to get the field on")]
        private Troop troopRef;

        public override void OnAwake()
        {
            base.OnAwake();
            troopRef = gameObject.GetComponentInParent<Troop>();
        }
        public override TaskStatus OnUpdate()
        {
            if (troopRef == null || !troopRef.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("Unable to get field - field value is null");
                return TaskStatus.Failure;
            }

            if(troopRef.FocusEntity == null && troopRef.TargetCastle == null)
            {
                return TaskStatus.Failure;
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            troopRef = null;
        }
    }
}
