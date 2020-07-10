using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public class HFCameraWarRoom : MonoBehaviour
    {
        public static HFCameraWarRoom Instance;

        [SerializeField]
        private int CountOfPositions;
        private Animator m_animatorController;
        private int m_position = 0;

        private void Awake()
        {
            Instance = this;
            m_animatorController = GetComponent<Animator>();
        }

        private void Update() 
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) 
            {
                int newPos = m_position + 1;
                SetPositionCount(newPos);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) 
            {
                int newPos = m_position - 1;
                SetPositionCount(newPos);
            }
        }

        public void SetPositionCount(int index)
        {
            m_position = index % CountOfPositions;
            m_position = (int)Mathf.Repeat(m_position, CountOfPositions);
            m_animatorController.SetInteger("Position", m_position);
        }
    }
}
