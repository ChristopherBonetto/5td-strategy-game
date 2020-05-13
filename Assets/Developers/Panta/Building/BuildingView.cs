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
        private Transform m_weapon;

        [SerializeField]
        private Transform m_SpawnPoint;
        public Transform SpawnPoint => m_SpawnPoint;

        [SerializeField]
        private HFPoolID m_BulletID;
        public HFPoolID BulletID => m_BulletID;

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

        //public void TrackOstile(Entity ostile)
        //{
        //    m_weapon.LookAt(ostile.transform);
        //}
    }
}
