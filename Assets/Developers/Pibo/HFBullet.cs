using UnityEngine;
using System.Collections;
using Types;

namespace HF
{
	public struct HFBulletParameters
	{
		public PlayerType Instigator;
		public float UnitDamage;
		public float BuildingDamage;
		public float Speed;

		public HFBulletParameters(PlayerType inInstigator, float inUnitDamage = 0f, float inBuildingDamage = 0f, float inSpeed = 10f)
		{
			Instigator = inInstigator;
			UnitDamage = inUnitDamage;
			BuildingDamage = inBuildingDamage;
			Speed = inSpeed;
		}
	}

	public class HFBullet : MonoBehaviour
	{
		[SerializeField]
		private Renderer m_mesh = null;

        [SerializeField]
        private ParticleSystem m_bulletParticle = null;

        [SerializeField]
		private ParticleSystem m_explosionHit = null;

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
           
            if(m_bulletParticle!=null)
            {
                m_bulletParticle.Play();
            }

            if(m_explosionHit!=null)
            {
                m_explosionHit.Stop();
            }
		}

		private void OnDisable()
		{
			m_rigidbody.velocity = Vector3.zero;
		}

		private void OnTriggerEnter(Collider other)
		{
            if (other.gameObject.layer != LayerMask.NameToLayer(m_layer))

                StartCoroutine(DisableBullet());
		}

		/// <summary>
		/// Put the layer of the gameobject that spawn it.
		/// </summary>
		/// <param name="layer"></param>
		private void SetAllyLayer(string layer) 
		{
			m_layer = layer;
		}

		public void SetParameters(HFBulletParameters inParameters)
		{
			m_parameters = inParameters;
			if(m_parameters.Instigator == PlayerType.AI)
			{
				SetAllyLayer(LayerMask.LayerToName(GameController.Instance.m_aiLayer));
			}
			else if(m_parameters.Instigator == PlayerType.Player)
			{
				SetAllyLayer(LayerMask.LayerToName(GameController.Instance.m_playerLayer));
			}
		}

        IEnumerator DisableBullet()
        {
            if (m_bulletParticle != null)
            {
                m_bulletParticle.Stop();
            }
            m_rigidbody.velocity = Vector3.zero;
            if (m_explosionHit != null)
            {

                m_explosionHit.Play();
            }
            yield return new WaitForSeconds(.5f);
            gameObject.SetActive(false);
        }
    }
}
