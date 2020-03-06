using UnityEngine;

namespace HF
{
	public class HFAIController : HFController
	{
		protected override void TrySelect()
		{ }

		protected override void TryInteract()
		{ }

		public override HFUnit SpawnUnit(HFBaseStats stats, Vector3 location)
		{
			HFUnit newUnit = base.SpawnUnit(stats, location);

			Vector3 destination = new Vector3(20f, 0f, 0f);
			newUnit.SetStartCommand(new HFMoveCommand(destination));

			return newUnit;
		}

        public override void Respawn()
        { }
    }
}
