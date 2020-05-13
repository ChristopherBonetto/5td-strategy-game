namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedTroop : SharedVariable<Troop>
    {
        public static implicit operator SharedTroop(Troop value) { return new SharedTroop { mValue = value }; }
    }
}
