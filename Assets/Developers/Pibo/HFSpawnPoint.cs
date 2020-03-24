using UnityEngine;

namespace HF
{
	public class HFSpawnPoint : MonoBehaviour
	{
		[SerializeField]
		private HFUnitType m_unitType = 0;

		public HFUnitType UnitType => m_unitType;

		[Tooltip("Target must implement IHFTargetable interface")]
		[SerializeField]
		private GameObject m_target = null;

		public Vector3 TargetPosition => m_target.GetComponent<IHFTargetable>().Position;
		public Vector3 SpawnPosition => transform.position.SnapLocation();
		public Quaternion SpawnRotation => transform.rotation;

#if UNITY_EDITOR
		void OnValidate()
		{
			if (!m_target)
			{
				return;
			}

			if (m_target.GetComponent<IHFTargetable>() == null)
			{
				//(new UnityEditor.EditorWindow()).ShowNotification(new GUIContent(m_target.gameObject.name + " is not a valid target"), 2f);
				Debug.LogWarning(m_target.gameObject.name + " is not a valid target");
				m_target = null;
			}
		}
#endif
	}
}
