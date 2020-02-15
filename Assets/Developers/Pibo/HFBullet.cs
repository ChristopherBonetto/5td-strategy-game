using UnityEngine;

namespace HF
{
	public struct HFBulletParameters
	{
		public float UnitDamage;
		public float BuildingDamage;

		public HFBulletParameters(float inUnitDamage, float inBuildingDamage)
		{
			UnitDamage = inUnitDamage;
			BuildingDamage = inBuildingDamage;
		}
	}

	public class HFBullet : MonoBehaviour
	{
		private bool m_isExploding = false;

		[SerializeField]
		[Tooltip("the speed of the bullet")]
		private float m_speed = 10f;

		private Collider m_target;

		[SerializeField]
		private Renderer m_mesh = null;

		[SerializeField]
		private ParticleSystem m_explosionParticle = null;

		private Transform m_transform;

		private HFBulletParameters m_parameters;

		private void Awake()
		{
			m_transform = transform;

			Destroy(gameObject, 2f);
		}

		void Update()
		{
			if (!m_isExploding)
			{
				m_transform.position += m_transform.forward * m_speed * Time.deltaTime;

				if (m_target)
				{
					m_transform.LookAt(m_target.bounds.center);
				}
				else
				{
					Destroy(gameObject);
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other == m_target)
			{
				if (m_target.enabled)
				{
					HFUnit targetUnit = m_target.gameObject.GetComponent<HFUnit>();
					float actualDamage = (targetUnit.UnitType == HFUnitType.Unit ? m_parameters.UnitDamage);
					targetUnit.TakeDamage(new DamageInfo(actualDamage));
				}
				if (m_explosionParticle)
				{
					m_explosionParticle.Play();
				}
				m_isExploding = true;

				m_mesh.enabled = false;

				Destroy(gameObject, 1f);
			}
		}

		public void SetTarget(Collider inTarget)
		{
			m_target = inTarget;
		}

		public void SetParameters(HFBulletParameters inParameters)
		{
			m_parameters = inParameters;
		}
	}
}
