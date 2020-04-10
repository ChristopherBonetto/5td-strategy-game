using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class HFUnitView : MonoBehaviour
    {
        [SerializeField]
        private HFPoolID m_UnitPopUpID;
        public HFPoolID UnitPopUpID => m_UnitPopUpID;

        private HFUnit m_unitComponent;
        private GameObject m_pooledGameObject;


        private void Awake()
        {
            m_unitComponent = GetComponent<HFUnit>();
        }

        private void OnEnable()
        {
            HFEventManager.SubscribeTo<HFUnit, int>(HFEventID.OnUnitSelected, OnUnitSelected);
        }

        private void OnDisable()
        {
            HFEventManager.UnsubscribeFrom<HFUnit, int>(HFEventID.OnUnitSelected, OnUnitSelected);
        }


        //-----------------------------------------------------------
        // Interact with this component through events. If the ally
        // unit is selected (OnUnitSelect), then sowh upgrade or 
        // specialization. If the unit is already specialized, show
        // updgrade, if not show the specialization. Cam be selected
        // only one unit at time.
        // When the unit is selected --> Pool the icons that allow
        // upgrade or specialization --> chack if the unit is already
        // specialized --> assign the unit to the UI element.
        //-----------------------------------------------------------

        public void OnUnitSelected(HFUnit unit, int team)
        {
            // handle deselect unit.
            // if the unit is null and is
            // triggered by player's unit.
            if (unit == null && team == 0)
            {
                if (m_pooledGameObject)
                {
                    m_pooledGameObject.SetActive(false);
                }
                return;
            }

            // if it's the player's unit and 
            // it's the same unit as the one clicked.
            if (team == 0 && unit == m_unitComponent)
            {
                // Pool the popUp, get component of the popUp.
                m_pooledGameObject = HFPoolManager.Instance.GetPooledObject(m_UnitPopUpID.ID);
                HFUnitUIPopUpUpgrade popUp = m_pooledGameObject.GetComponent<HFUnitUIPopUpUpgrade>();

                // Filter the option: specialization or upgrade.
                if (!unit.IsSpecialized && unit.CanBeSpecialize)
                {
                    popUp.ShowSpecializations(unit);
                }
                else
                {
                    popUp.ShowUpgrade(unit);
                }

                popUp.gameObject.SetActive(true);
            }
        }
    }
}
