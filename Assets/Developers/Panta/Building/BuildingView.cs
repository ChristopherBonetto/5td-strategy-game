using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HF.Unit
{
    [RequireComponent(typeof(NavMeshObstacle))]
    public class BuildingView : MonoBehaviour
    {
        // The obstacle is set to the view because we can
        // adapt the size when modify prefab.
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
    }
}
