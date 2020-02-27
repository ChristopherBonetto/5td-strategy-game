using System.Collections.Generic;
using UnityEngine;

namespace HF
{
	public class HFUnitVisuals : MonoBehaviour
	{
		[System.Serializable]
		public struct LevelMeshes
		{
			public List<GameObject> List;
		}

		[SerializeField]
		private Transform m_pivot = null;
		public Transform Pivot => m_pivot;

		[SerializeField]
		private bool m_usesPivot = false;
		public bool UsesPivot => m_usesPivot;

		[SerializeField]
		private Collider m_target = null;
		public Collider Target => m_target;

		[SerializeField]
		private LevelMeshes[] m_visuals = new LevelMeshes[3];
		public LevelMeshes[] Visuals => m_visuals;
	}
}
