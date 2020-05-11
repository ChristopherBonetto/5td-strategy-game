using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public class HFPauseModeB : HFButton, IHFTutorial
    {
        bool isPaused = false;

        public TutorialID TutorialID { get; set; } = TutorialID.Pause_Mode;
        public GameEventData Events;
        public GameEventData Initialization;

        private void Awake()
        {
            Initialization.AddListener(this);
            Events.AddListener(this);
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
                isPaused = !isPaused;
                HFEventManager.TriggerEvent(HFEventID.OnPauseMode, isPaused);
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
    }
}
