using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    /// <summary>
    /// Handle pause mode bheaviour.
    /// </summary>
    public class HFUnitViewModel : MonoBehaviour
    {
        [SerializeField]
        private HFPoolID m_UnitPopUpID;
        public HFPoolID UnitPopUpID => m_UnitPopUpID;

        private HFUnit m_unitComponent;
        private GameObject m_pooledGameObject;


        private Animator m_animator;
        private HFUnit m_unit;

        private void Awake()
        {
            m_unitComponent = GetComponent<HFUnit>();
            m_animator = GetComponent<Animator>();
            m_unit = GetComponent<HFUnit>();
        }

        private void OnEnable()
        {
            HFEventManager.SubscribeTo<HFUnit, int>(HFEventID.OnUnitSelected, OnUnitSelected);
            HFEventManager.SubscribeTo<bool>(HFEventID.OnPauseMode, OnPauseMode);
        }

        private void OnDisable()
        {
            HFEventManager.UnsubscribeFrom<HFUnit, int>(HFEventID.OnUnitSelected, OnUnitSelected);
            HFEventManager.UnsubscribeFrom<bool>(HFEventID.OnPauseMode, OnPauseMode);
        }



        public void OnPauseMode(bool freeze)
        {
            if (freeze)
                StartCoroutine(Freeze());
            else
                StartCoroutine(UnFreeze());
        }

        IEnumerator Freeze()
        {
            float timeScale = 1;
            float targetTimeScale = 0;

            while(timeScale > targetTimeScale)
            {
                timeScale -= Time.deltaTime;
                timeScale = Mathf.Clamp(timeScale, 0, 1);
                //m_animator.speed = timeScale;
                yield return null;
            }

            if (m_unit.m_navAgent.isOnNavMesh)
                m_unit.m_navAgent.isStopped = true;
            m_unit.Updaiting = false;
        }

        IEnumerator UnFreeze()
        {
            float timeScale = 0;
            float targetTimeScale = 1;

            while (timeScale < targetTimeScale)
            {
                timeScale += Time.deltaTime;
                timeScale = Mathf.Clamp(timeScale, 0, 1);
                //m_animator.speed = timeScale;
                yield return null;
            }

            if (m_unit.m_navAgent.isOnNavMesh)
                m_unit.m_navAgent.isStopped = false;
            m_unit.Updaiting = true;
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
