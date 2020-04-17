using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public class HFCameraWarRoom : MonoBehaviour
    {
        private Animator m_animatorController;
        private int m_position = 0;

        private void Awake()
        {
            m_animatorController = GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                m_position = 0;
                m_animatorController.SetInteger("Position", m_position);
                HFUIManager.Instance.ShowAndClearHistory(HFUIWindowID.WR_LEVEL_SELCTION);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                m_position = 1;
                m_animatorController.SetInteger("Position", m_position);
                HFUIManager.Instance.ShowAndClearHistory(HFUIWindowID.WR_SETTINGS);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                m_position = 2;
                m_animatorController.SetInteger("Position", m_position);
                HFUIManager.Instance.ShowAndClearHistory(HFUIWindowID.WR_CREDITS);
            }
        }
    }
}
