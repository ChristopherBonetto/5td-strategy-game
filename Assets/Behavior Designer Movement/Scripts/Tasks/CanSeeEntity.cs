using FMOD;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Check to see if the any objects are within sight of the agent.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
    public class CanSeeEntity : Conditional
    {
        [Tooltip("The object that we are searching for")]
        public SharedEntity targetObject;
        [Tooltip("The LayerMask of the objects that we are searching for")]
        public LayerMask objectLayerMask;
        [Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
        public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        [Tooltip("The distance that the agent can see")]
        public SharedFloat viewDistance = 1000;
        [Tooltip("The object that is within sight")]
        public SharedEntity returnedObject;

        private Collider[] overlapColliders = new Collider[5];

        private int numberOfCollisions;

        // Returns success if an object was found otherwise failure
        public override TaskStatus OnUpdate()
        {
            GameObject go = null;
            numberOfCollisions = Physics.OverlapSphereNonAlloc(transform.position, viewDistance.Value, overlapColliders, objectLayerMask);
            for (int i = 0; i < numberOfCollisions; i++)
            {
                if (overlapColliders[i])
                {
                   go = overlapColliders[i].gameObject;
                }
            }
            UnityEngine.Debug.Log(numberOfCollisions);

            if (go != null)
            {
                returnedObject.Value = go.GetComponent<EntityBehavior>();

                //MAYBE can see if the entity is busy or not.
                return TaskStatus.Success;
            }

            // An object is not within sight so return failure
            return TaskStatus.Failure;
        }

        // Reset the public variables
        public override void OnReset()
        {
            viewDistance = 1000;
        }

        // Draw the line of sight representation within the scene window
        public override void OnDrawGizmos()
        {
            MovementUtility.DrawLineOfSight(Owner.transform, Vector3.zero, 360, 0, viewDistance.Value, false);
        }

        public override void OnBehaviorComplete()
        {
            MovementUtility.ClearCache();
        }
    }
}