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
        [Tooltip("Enable to see busying troops")]
        public SharedBool canSeeBusyTroop;
        [Tooltip("Enable object to detect player's building/tower")]
        public SharedBool canSeeBuilding;
        [Tooltip("The LayerMask of the objects that we are searching for")]
        public LayerMask objectLayerMask;
        [Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
        public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        [Tooltip("The distance that the agent can see")]
        public SharedFloat viewDistance = 1000;
        [Tooltip("The object that is within sight")]
        public SharedEntity returnedObject;

        private Collider[] overlapColliders = new Collider[1];

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

            if (go != null)
            {
                float distance = Vector3.Distance(go.transform.position + Vector3.up * 1.5f, transform.position + Vector3.up * 1.5f) - .1f;

                if (!Physics.Raycast(transform.position + Vector3.up * 1.5f, (go.transform.position - transform.position).normalized + Vector3.up * 1.5f, distance, LayerMask.GetMask("Terrain")))
                {
                    returnedObject.Value = go.GetComponentInParent<EntityBehavior>();

                    if (!returnedObject.Value.IsBusy || canSeeBusyTroop.Value && returnedObject.Value.IsBusy)
                    {
                        if (returnedObject.Value is BuildingBehaviour && canSeeBuilding.Value)
                        {
                            return TaskStatus.Success;
                        }
                        else if (returnedObject.Value is Troop)
                        {
                            return TaskStatus.Success;
                        }
                    }
                }
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