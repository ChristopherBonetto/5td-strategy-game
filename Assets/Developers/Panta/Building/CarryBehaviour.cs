using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Unit
{
    /// <summary>
    /// Component that allows entity to perform carry and drop action.
    /// </summary>
    public class CarryBehaviour : MonoBehaviour
    {
        private NewTroopBehavior m_troopBehaviourComponent;
        private bool m_isCarring = false;

        // Here all variable used to detect building.
        [SerializeField]
        private float m_detectionRange = 2f;
        private IDetectGeneric<BuildingBehaviour> m_detectionArea;

        [SerializeField, Tooltip("Declare where the building can be dropped")]
        private LayerMask m_droppableMask;
        [SerializeField, Tooltip("Declare the building layer")]
        private LayerMask m_interactionMask;
        private BuildingBehaviour m_building = null;


        private void Awake()
        {
            m_troopBehaviourComponent = GetComponent<NewTroopBehavior>();
        }

        private void Start()
        {
            m_detectionArea = new DetectionAreaGeneric<BuildingBehaviour>(1);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (m_isCarring)
                    TryDrop();
                else if (!m_isCarring)
                    TryCarry();
            }
        }

        public void Init()
        {
            m_isCarring = false;
        }

        public void TryCarry()
        {
            if (!m_isCarring)
            {
                m_building = m_detectionArea.Detect(transform, m_detectionRange, m_interactionMask);

                if (m_building != null)
                {
                    // pick the carry position from the troops behaviour.

                    m_building.Carry(m_troopBehaviourComponent, m_troopBehaviourComponent.CarryPoint.position);

                    // play the animation for each unit.
                    //foreach (UnitBehavior unit in m_troopBehaviourComponent.m_units)
                    //{
                    //    unit.Anim.SetBool("IsCarrying", true);
                    //}

                    // unit can't attack while carrying, so set that.
                    // Ale questa è solo una info dello scriptable se l'entità è in grado di attaccare. Non è proprio una bool.
                    //foreach (NewUnitBehavior unit in m_troopBehaviourComponent.m_units)
                    //{
                    //    unit.EntityStats.CanAttack = false;
                    //}

                    m_isCarring = true;
                }
                else
                {
                    Debug.Log($"There is no building nearby");
                }
            }
        }

        public void TryDrop()
        {
            if (m_isCarring)
            {
                // in order to drop the turret, we first che the drop position
                Vector3 dropPosition = transform.position + transform.forward * 2f;
                bool fullPosition = Physics.CheckBox(dropPosition, m_building.Collider.bounds.extents, Quaternion.identity);

                if (fullPosition)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(dropPosition, Vector3.down, out hit, 10f, m_droppableMask))
                    {
                        // Play the animation foreach unit.

                        // units now can attack,
                        //foreach (NewUnitBehavior unit in m_troopBehaviourComponent.m_units)
                        //{
                        //    unit.EntityStats.CanAttack = true;
                        //}

                        m_building.Drop(m_troopBehaviourComponent, hit.point + Vector3.up * m_building.Collider.bounds.extents.y);
                        m_building.transform.up = hit.normal;
                        m_building = null;

                        m_isCarring = false;
                    }
                    else
                    {
                        Debug.Log($"You can't drop turret here");
                    }
                }
                else
                {
                    Debug.Log($"You can't drop turret here");
                }
            }
        }
    }
}
