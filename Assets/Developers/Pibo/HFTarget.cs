using UnityEngine;

namespace HF
{
	public class HFTarget : MonoBehaviour, IHFTargetable
	{
		public Vector3 Position => transform.position.SnapLocation();

		public bool TryInteraction(HFUnit otherUnit)
		{
			return false;
		}

		public IHFCommand GetDefaultInteraction()
		{
			return new HFMoveCommand(Position);
		}
	}
}
