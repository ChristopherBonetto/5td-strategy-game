using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.LWRP;

namespace HF.Refactoring
{
    public class HFPauseModeB : HFButton, IHFTutorial
    {
        public TutorialID TutorialID { get; set; } = TutorialID.Pause_Mode;
        public GameEventData Events;
        public GameEventData Initialization;
        private HFUIHUD m_hudRef;

        private void Awake()
        {
            Initialization.AddListener(this);
            Events.AddListener(this);
            m_hudRef = HFUIManager.Instance.Getwindow<HFUIHUD>(HFUIWindowID.HUD);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        private void OnDestroy()
        {
            Initialization.RemoveListener(this);
            Events.RemoveListener(this);
        }

        public void OnGlobalInitialization()
        {
            gameObject.SetActive(false);
        }

        public void OnPauseMode()
        {
            if (m_isListeningInput)
            {
                m_hudRef.IsPaused = !m_hudRef.IsPaused;
                HFEventManager.TriggerEvent<bool>(HFEventID.OnPauseMode, m_hudRef.IsPaused);
                HFEventManager.TriggerEvent(HFEventID.OnTutorialQuestCompleted, TutorialID);
            }
        }

        public void OnStepCompleted()
        {
            
        }

        public void OnStepInitialization()
        {
            gameObject.SetActive(true);
        }

        public void Reset()
        {
            gameObject.SetActive(true);
        }
    }
}
