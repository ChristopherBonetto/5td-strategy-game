using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public abstract class TroopMovement : GroupMovement
    {
        [Tooltip("The speed of the agents")]
        public SharedFloat speed = 10;
        [Tooltip("The angular speed of the agents")]
        public SharedFloat angularSpeed = 120;


        public SharedTroopClass TroopRef = null;

        protected Unit Captain = null;
        protected List<Unit> units = null;
        protected Transform[] transforms;

        public override void OnStart()
        {
            Troop troop = TroopRef.Value;

            Captain = troop.Captain;

            units = troop.m_units;

            transforms = new Transform[units.Count];

            for (int i = 0; i < units.Count; ++i)
            {
                transforms[i] = units[i].transform;
                units[i].UnitAgent.speed = speed.Value;
                units[i].UnitAgent.angularSpeed = angularSpeed.Value;
                units[i].UnitAgent.isStopped = false;
            }
        }

        protected override bool SetDestination(int index, Vector3 target)
        {
            if (units[index].UnitAgent.destination == target)
            {
                return true;
            }
            return units[index].UnitAgent.SetDestination(target);
        }

        protected override Vector3 Velocity(int index)
        {
            return units[index].UnitAgent.velocity;
        }

        public override void OnEnd()
        {
            // Disable the nav mesh
            for (int i = 0; i < units.Count; ++i)
            {
                if (units[i] != null)
                {
                    units[i].UnitAgent.isStopped = true;
                }
            }
        }

        // Reset the public variables
        public override void OnReset()
        {
            Captain = null;
        }
    }
}
