using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedTroopClass : SharedVariable<Troop>
    {
        public static implicit operator SharedTroopClass(Troop value) { return new SharedTroopClass { mValue = value }; }
    }
}
