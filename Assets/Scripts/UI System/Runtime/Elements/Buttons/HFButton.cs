using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public abstract class HFButton : MonoBehaviour
    {
        [Tooltip("Choose the current window ID where the button is")]
        public HFUIWindowID MyWindowID;
        protected bool m_isMatchingWindowID;

        protected bool m_isListeningInput;

        protected virtual void OnEnable()
        {
            HFUIManager.Instance.IsMatchingWindowID += IsMatchingWiindowID;
        }

        protected virtual void OnDisable()
        {
            HFUIManager.Instance.IsMatchingWindowID -= IsMatchingWiindowID;
        }

        /// <summary>
        /// Filter what element listen input.
        /// </summary>
        /// <param name="id"></param>
        protected virtual void IsMatchingWiindowID(HFUIWindowID id)
        {
            m_isMatchingWindowID = MyWindowID == id;
            m_isListeningInput = m_isMatchingWindowID;
        }
    }
}
