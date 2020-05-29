using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public class HFShowWindowB : HFButton
    {
        [Tooltip("Choose the window ID to show")]
        public HFUIWindowID WindowIDToShow;

        [Tooltip("Toggling to true, the previos window remain active")]
        public bool ShowAddittive = false;

        public void ShowPanel()
        {
            if (m_isMatchingWindowID)
            {
                HFGameManager.Instance.ChangeGMState(GameStates.Pause);
                HFUIManager.Instance.ShowAndAddToHistory(WindowIDToShow, ShowAddittive);    
            }
        }
    }
}
