using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public class HFUIPopUpHandler : MonoBehaviour
    {
        [Header("IDs")]
        [SerializeField]
        private HFPoolID m_unitPopUpSpecializationID;
        [SerializeField]
        private HFPoolID m_unitPopUpUpgradeID;

        private HFUnit m_unit;
        private Camera m_cam;
        private List<HFUIUnitPopUpB> m_upgradesPopUps = new List<HFUIUnitPopUpB>();
        private List<HFUIUnitPopUpB> m_specializationPopUps = new List<HFUIUnitPopUpB>();

        private void Start()
        {
            m_cam = Camera.main;
        }

        private void Update()
        {
            if (m_unit != null)
            {
                transform.position = RectTransformUtility.WorldToScreenPoint(m_cam, m_unit.transform.position);
            }
        }

        public void SetUp(HFUnit unit)
        {
             m_unit = unit;
             
            if (m_unit == null)
            {
                gameObject.SetActive(false);
                return;
            }

            // If the unit must be specialized then show specializations
            // Else show the upgrade. If can't be upgraded then show it but it don't perform any action.
        }

        private void EnableButtons(bool enabled, List<HFUIUnitPopUpB> buttons)
        {
            foreach (var button in buttons)
            {
                button.gameObject.SetActive(enabled);
            }
        }
    }
}
