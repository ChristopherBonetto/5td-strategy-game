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
        private int m_Position = 0;
        public int Position => m_Position;

        [SerializeField]
        private KeyCode ResetKey;



        private HFUIWarRoom m_warRoomRef;

        private void Awake()
        {
            Instance = this;
            m_animatorController = GetComponent<Animator>();

            m_warRoomRef = HFUIManager.Instance.Getwindow<HFUIWarRoom>(HFUIWindowID.WAR_ROOM);
            m_warRoomRef.EnableAllButtons(m_Position == 1);
        }

        private void Update() 
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) 
            {
                int newPos = m_Position + 1;
                SetPositionCount(newPos);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) 
            {
                int newPos = m_Position - 1;
                SetPositionCount(newPos);
            }
            else if (Input.GetKeyDown(ResetKey)||  Input.GetMouseButton(1)) 
            {
                SetPositionCount(0);
            }
        }

        public void SetPositionCount(int index)
        {
            // Turn off all buttons, if the camera focus the level selection,
            // the buttons are turned on at the end of the transition.
            EnableAllButtons(0);

            m_Position = index % CountOfPositions;
            m_Position = (int)Mathf.Repeat(m_Position, CountOfPositions);
            m_animatorController.SetInteger("Position", m_Position);
        }

        /// <summary>
        /// This function is colled in animation event.
        /// Since the animation doesn't support boolean, I use an int 0 or 1
        /// </summary>
        /// <param name="enabled"></param>
        public void EnableAllButtons(int index) 
        {
            // Level selection is on index 1
            m_warRoomRef.EnableAllButtons(index == 1);
        }
    }
}
