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

		private HFBulletParameters m_parameters = new HFBulletParameters();

		private void Awake()
		{
			m_transform = transform;
			m_rigidbody = GetComponent<Rigidbody>();

			Destroy(gameObject, 1.5f);
		}

		void Start()
		{
			m_rigidbody.AddRelativeForce(0f, 0f, m_parameters.Speed, ForceMode.VelocityChange);
		}

		private void OnTriggerEnter(Collider other)
		{
			// Ignore self
			if (m_parameters.Instigator.Colliders.Contains(other))
			{
				return;
			}

			bool bHit = false;
			m_target = other.gameObject.GetComponentInParent<HFUnit>();
			if (m_target && m_target.enabled)
			{
				bHit = true;
				DamageInfo damageInfo = new DamageInfo(
					m_parameters.Instigator,
					m_parameters.UnitDamage,
					m_parameters.BuildingDamage
					);
				m_target.TakeDamage(damageInfo);
			}

			ParticleSystem explosion = (bHit ? m_explosionHit : m_explosionMissed);
			if (explosion)
			{
				explosion.Play();
			}

			m_mesh.enabled = false;

			Destroy(gameObject, 0.5f);
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
