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
        public EntityBehavior targetObject;
        [Tooltip("Enable to see busying troops")]
        public SharedBool canSeeBusyTroop;
        [Tooltip("Enable object to detect player's building/tower")]
        public SharedBool canSeeBuilding;
        [Tooltip("The LayerMask of the objects that we are searching for")]
        public LayerMask objectLayerMask;
        [Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
        public LayerMask obstaclesLayerMask;
        [Tooltip("The distance that the agent can see")]
        public SharedFloat viewDistance = 1000;
        [Tooltip("The object that is within sight")]
        private EntityBehavior returnedObject;

        private Collider[] overlapColliders = new Collider[1];

        private int numberOfCollisions;

        private EntityBehavior entityRef;

        public override void OnAwake()
        {
            base.OnAwake();

            entityRef = gameObject.GetComponent<EntityBehavior>();
        }

        // Returns success if an object was found otherwise failure
        public override TaskStatus OnUpdate()
        {
            if (entityRef.IsLifting) return TaskStatus.Failure;

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
                if (Physics.Linecast(transform.position + Vector3.up, go.transform.position + Vector3.up, obstaclesLayerMask))
                {
                    return TaskStatus.Failure;
                }
                else
                {
                    UnityEngine.Debug.Log(transform.name + " see  " + go.name);
                    UnityEngine.Debug.DrawLine(transform.position + Vector3.up, go.transform.position + Vector3.up, Color.red);

                    returnedObject = go.GetComponentInParent<EntityBehavior>();

                    if (returnedObject == null) return TaskStatus.Failure;

                    if (returnedObject is BuildingBehaviour && canSeeBuilding.Value)
                    {
                        if (entityRef.AssignFocusEntity(returnedObject))
                        {
                            return TaskStatus.Success;
                        }
                    }
                    else if (returnedObject is Troop)
                    {
                        if (entityRef.AssignFocusEntity(returnedObject))
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
            returnedObject = null;
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