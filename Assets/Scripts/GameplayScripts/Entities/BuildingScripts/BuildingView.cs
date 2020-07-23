using System.Collections;
using System.Collections.Generic;
using Types;
using UnityEngine;
using UnityEngine.AI;

namespace HF.Unit
{
    [RequireComponent(typeof(NavMeshObstacle))]
    public class BuildingView : MonoBehaviour
    {
        [SerializeField]
        private Transform m_objectToRotateOnY;
        [SerializeField]
        private Transform m_objectToRotateOnX;
        [SerializeField]
        private bool m_lockSpawnBulletPoint;

        [SerializeField]
        private Transform m_SpawnPoint;
        public Transform SpawnPoint => m_SpawnPoint;

        public bool AoEDamage = false;

        #region Visual Feedbacks
        public Animator anim;
        public ParticleSystem drop;
        public GameObject RangeFeedback;
        public ParticleSystem muzzleFlash;
        #endregion

        /*
        * The obstacle is set to the view because we can
        * adapt the size when modify prefab.
        */
        private NavMeshObstacle m_NavMeshObstacle;
        public NavMeshObstacle NavMeshObstacle
        {
            get
            {
                if (m_NavMeshObstacle == null)
                    m_NavMeshObstacle = GetComponent<NavMeshObstacle>();
                return m_NavMeshObstacle;
            }
        }

        private void OnEnable()
        {
            RangeFeedback.SetActive(false);

            if(drop!=null)
            {
                drop.Stop();
            }
            if(muzzleFlash!=null)
            {
                muzzleFlash.Stop();
            }
        }

        public void CarryBuilding()
        {
            NavMeshObstacle.enabled = false;
        }

        public void DropBuilding()
        {
            NavMeshObstacle.enabled = true;
           if(drop!=null)
            {
                drop.Play();
            }
        }

        public void SpawnBullet(PlayerType inPlayerType, BuildingsStatsSO inParam)
        {
            if(anim!=null)
            {
                anim.SetTrigger("isShooting");
            }
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }


            // Get bullet from pool.
            GameObject go = HFPoolManager.Instance.GetPooledObject(inParam.Projectile.ID);

            Debug.Log(go);
            // Set spawn position and rotation.
            if (go != null) 
            {
                go.transform.position = m_SpawnPoint.position;
                go.transform.rotation = m_SpawnPoint.rotation;

                HFBullet bullet = go.GetComponent<HFBullet>();
                bullet.SetParameters(new HFBulletParameters(inPlayerType, inParam.Damage, inParam.Damage, inParam.ProjectileSpeed));
                go.SetActive(true);
            }
        }

        public void TrackOstile(Vector3 ostile)
        {
            float angleOnX = 0;
            float angleOnY = 0;


            if (m_objectToRotateOnX != null)
            {
                float angle = Vector3.Angle(ostile - m_objectToRotateOnX.position, m_objectToRotateOnX.up);
                angleOnX = angle;
                m_objectToRotateOnX.localRotation = Quaternion.Euler(angleOnX, angleOnY, 0);
            }

            if (m_objectToRotateOnY != null)
            {
                Vector3 direction = ostile - m_objectToRotateOnY.position;
                angleOnY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                m_objectToRotateOnY.localRotation = Quaternion.Euler(angleOnX, angleOnY, 0);
            }

            if (m_objectToRotateOnY != null && m_objectToRotateOnX != null && m_objectToRotateOnX == m_objectToRotateOnY)
            {
                m_objectToRotateOnY.localRotation = Quaternion.Euler(angleOnX, angleOnY, 0);
            }

            if (!m_lockSpawnBulletPoint)
            {
                m_SpawnPoint.forward = ostile - m_SpawnPoint.position + Vector3.up * 2;
            }
        }
        public void UpdateTowerVisualState(bool state)
        {
            RangeFeedback.SetActive(state);
        }
    }

}
