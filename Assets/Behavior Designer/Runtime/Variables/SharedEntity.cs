namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedEntity : SharedVariable<EntityBehavior>
    {
        public static implicit operator SharedEntity(EntityBehavior value) { return new SharedEntity { mValue = value }; }
    }
}
