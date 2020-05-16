using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HF.Unit
{
    [RequireComponent(typeof(NavMeshObstacle))]
    public class BuildingView : MonoBehaviour
    {
        [SerializeField]
        private Transform m_SpawnPoint;
        public Transform SpawnPoint => m_SpawnPoint;

        [SerializeField]
        private HFPoolID m_BulletID;
        public HFPoolID BulletID => m_BulletID;

        [SerializeField]
        private float m_BulletSpeed;
        public float BulletSpeed => m_BulletSpeed;

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

        public void CarryBuilding()
        {
            NavMeshObstacle.enabled = false;
        }

        public void DropBuilding()
        {
            NavMeshObstacle.enabled = true;
        }

        public void SpawnBullet()
        {
            // Get bullet from pool.
            GameObject go = HFPoolManager.Instance.GetPooledObject(m_BulletID.ID);
            // Set spawn position and rotation.
            go.transform.position = m_SpawnPoint.position;
            go.transform.rotation = m_SpawnPoint.rotation;
            go.GetComponent<HFBullet>().SetParameters(new HFBulletParameters(null, 0, 0, m_BulletSpeed));
            // Add force to it.
        }

        //public void TrackOstile(Entity ostile)
        //{
        //    m_weapon.LookAt(ostile.transform);
        //}
    }
}
