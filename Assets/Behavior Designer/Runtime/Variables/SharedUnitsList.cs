using UnityEngine;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedUnitList : SharedVariable<List<Unit>>
    {
        public static implicit operator SharedUnitList(List<Unit> value) { return new SharedUnitList { mValue = value }; }
    }
}
