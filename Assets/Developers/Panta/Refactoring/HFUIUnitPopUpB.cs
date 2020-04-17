using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace HF.Refactoring
{
    public class HFUIUnitPopUpB : HFButton
    {
        [SerializeField]
        private Image m_background;
        [SerializeField]
        private Image m_icon;
        [SerializeField]
        private Text m_cost;

        public delegate void CallBack();
        private CallBack m_callBack;

        protected override void OnEnable()
        {
            // Restore default values.
            base.OnEnable();
            SetCost(0, Color.white);
            SetBackgroundColor(Color.white);
        }

        protected override void OnDisable()
        {
            RemoveAllListener();
        }

        /// <summary>
        /// Invoke all delegates assigned.
        /// It's assigned in inspector in the OnClick.
        /// </summary>
        public void InvokDelegates()
        {
            if (m_isListeningInput)
            {
                m_callBack?.Invoke();
            }
        }

        /// <summary>
        /// Set the icon.
        /// </summary>
        public void SetIcon(Sprite icon)
        {
            m_icon.sprite = icon;
        }

        /// <summary>
        /// Set the background icon color.
        /// </summary>
        /// <param name="color"></param>
        public void SetBackgroundColor(Color color)
        {
            m_background.color = color;
        }

        /// <summary>
        /// Set the cost to show.
        /// </summary>
        public void SetCost(int cost, Color color)
        {
            m_cost.text = cost.ToString();
            m_cost.color = color;
        }

        /// <summary>
        /// Add an action to onClick.
        /// </summary>
        public void AddListener(CallBack callback)
        {
            m_callBack += callback;
        }

        /// <summary>
        /// Remove all actions to onClick.
        /// </summary>
        public void RemoveAllListener()
        {
            m_callBack = null;
        }
    }
}
