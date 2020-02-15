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
		private bool m_isExploding = false;

		private Collider m_target;

		[SerializeField]
		private Renderer m_mesh = null;

		[SerializeField]
		private ParticleSystem m_explosionParticle = null;

		private Transform m_transform;

		private HFBulletParameters m_parameters = new HFBulletParameters();

		private void Awake()
		{
			m_transform = transform;

			Destroy(gameObject, 2f);
		}

		void Update()
		{
			if (!m_isExploding)
			{
				MoveToTarget();
			}
		}

		private void MoveToTarget()
		{
			m_transform.position += m_transform.forward * m_parameters.Speed * Time.deltaTime;

			if (m_target)
			{
				m_transform.LookAt(m_target.bounds.center);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other == m_target)
			{
				if (m_target.enabled)
				{
					HFUnit targetUnit = m_target.gameObject.GetComponent<HFUnit>();
					float actualDamage = (targetUnit.UnitType == HFUnitType.Unit ? m_parameters.UnitDamage : m_parameters.BuildingDamage);
				}

				m_isExploding = true;
				if (m_explosionParticle)
				{
					m_explosionParticle.Play();
				}

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
