namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedBuilding : SharedVariable<BuildingBehaviour>
    {
        public static implicit operator SharedBuilding(BuildingBehaviour value) { return new SharedBuilding { mValue = value }; }
    }
}
