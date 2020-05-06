namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Perform the actual interruption. This will immediately stop the specified tasks from running and will return success or failure depending on the value of interrupt success.")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class InitializeTroop : Action
    {
        public SharedTroop troopRef;

        public override TaskStatus OnUpdate()
        {
            
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            //// Reset the properties back to their original values.
            troopRef = null;
        }
    }
}
