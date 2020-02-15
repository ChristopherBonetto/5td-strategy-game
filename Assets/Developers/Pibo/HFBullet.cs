using UnityEngine;

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
				//m_target.gameObject.GetComponent<HFUnit>().ReceiveAttack();
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
}
