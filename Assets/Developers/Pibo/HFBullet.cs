using UnityEngine;

namespace HF
{
	public struct HFBulletParameters
	{
		public HFUnit Instigator;
		public float UnitDamage;
		public float BuildingDamage;
		public float Speed;

		public HFBulletParameters(HFUnit inInstigator, float inUnitDamage = 0f, float inBuildingDamage = 0f, float inSpeed = 10f)
		{
			Instigator = inInstigator;
			UnitDamage = inUnitDamage;
			BuildingDamage = inBuildingDamage;
			Speed = inSpeed;
		}
	}

	public class HFBullet : MonoBehaviour
	{
		private HFUnit m_target;

		[SerializeField]
		private Renderer m_mesh = null;

		[SerializeField]
		private ParticleSystem m_explosionHit = null;

		[SerializeField]
		private ParticleSystem m_explosionMissed = null;

		private Transform m_transform;

		private Rigidbody m_rigidbody;
		private string m_layer;

		private HFBulletParameters m_parameters = new HFBulletParameters();

		private void Awake()
		{
			m_transform = transform;
			m_rigidbody = GetComponent<Rigidbody>();
		}

		void OnEnable()
		{
			m_rigidbody.AddRelativeForce(0f, 0f, m_parameters.Speed, ForceMode.VelocityChange);
		}

		private void OnDisable()
		{
			m_rigidbody.velocity = Vector3.zero;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.layer != LayerMask.NameToLayer(m_layer))
				gameObject.SetActive(false);
		}

		/// <summary>
		/// Put the layer of the gameobject that spawn it.
		/// </summary>
		/// <param name="layer"></param>
		public void SetAllyLayer(string layer) 
		{
			m_layer = layer;
		}

		public void SetTarget(HFUnit inTarget)
		{
			m_target = inTarget;
		}

		public void SetParameters(HFBulletParameters inParameters)
		{
			m_parameters = inParameters;
		}
	}
}
