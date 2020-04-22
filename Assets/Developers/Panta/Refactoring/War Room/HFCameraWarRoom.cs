using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public class HFCameraWarRoom : MonoBehaviour
    {
        [SerializeField]
        private int CountOfPositions;
        private Animator m_animatorController;
        private int m_position = 0;

        private void Awake()
        {
            m_animatorController = GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                m_position++;
                m_position = (int)Mathf.Repeat(m_position++, CountOfPositions);
                m_animatorController.SetInteger("Position", m_position);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                m_position--;
                m_position = (int)Mathf.Repeat(m_position--, CountOfPositions);
                m_animatorController.SetInteger("Position", m_position);
            }
        }
    }
}
