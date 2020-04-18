using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public abstract class HFButton : MonoBehaviour
    {
        [Tooltip("Choose the current window ID where the button is")]
        public HFUIWindowID MyWindowID;
        protected bool m_isListeningInput;

        protected virtual void OnEnable()
        {
            HFUIManager.Instance.IsListeningInput += IsListeningInput;
        }

        protected virtual void OnDisable()
        {
            HFUIManager.Instance.IsListeningInput -= IsListeningInput;
        }

        /// <summary>
        /// Filter what element listen input.
        /// </summary>
        /// <param name="id"></param>
        protected void IsListeningInput(HFUIWindowID id)
        {
            m_isListeningInput = MyWindowID == id;
        }
    }
}
