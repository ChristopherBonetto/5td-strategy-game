using UnityEngine;

namespace HF
{
	public class HFSpawnPoint : MonoBehaviour
	{
		[Tooltip("Target must implement IHFTargetable interface")]
		public BuildingBehaviour TargetCastle = null;

		public Vector3 SpawnPosition => transform.position.SnapLocation();
		public Quaternion SpawnRotation => transform.rotation;
	}
}
