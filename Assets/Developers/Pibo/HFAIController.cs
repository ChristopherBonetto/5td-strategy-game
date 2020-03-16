using UnityEngine;

namespace HF
{
	public class HFAIController : HFController
	{
		protected override void TrySelect()
		{ }

		protected override void TryInteract()
		{ }

		public override HFUnit SpawnUnit(HFBaseStats stats, HFSpawnPoint spawnPoint)
		{
			HFUnit newUnit = base.SpawnUnit(stats, spawnPoint);

			newUnit.SetStartCommand(new HFMoveCommand(spawnPoint.TargetPosition));

			return newUnit;
		}

        public override void Respawn()
        { }
    }
}
