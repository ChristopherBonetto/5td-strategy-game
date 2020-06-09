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
        public SharedTroop troopRef;

        public override TaskStatus OnUpdate()
        {
            if (troopRef == null || !troopRef.Value.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("Unable to get field - field value is null");
                return TaskStatus.Failure;
            }

            if (troopRef.Value.m_currentBattle == null)
            {
                Debug.Log(gameObject.transform.name + " captain no in combat");
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
