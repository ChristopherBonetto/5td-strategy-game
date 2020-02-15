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

		private HFUnit m_target;

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

			if (m_target && !m_target.IsKilled)
			{
				m_transform.LookAt(m_target.TargetCollider.bounds.center);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			Debug.Log(collision.gameObject.name);
			if (collision.collider == m_target.TargetCollider)
			{
				Debug.Log(m_target.gameObject.name);
				if (m_target && m_target.enabled)
				{
					DamageInfo damageInfo = new DamageInfo(
						m_parameters.Instigator,
						m_parameters.UnitDamage,
						m_parameters.BuildingDamage
						);
					m_target.TakeDamage(damageInfo);
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
