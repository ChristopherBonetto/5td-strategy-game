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
       public bool isPaused = false;

        public TutorialID TutorialID { get; set; } = TutorialID.Pause_Mode;
        public GameEventData Events;
        public GameEventData Initialization;

        public Image pauseHudMask;

        
        //public LightweightRenderPipelineAsset standard;
        //public LightweightRenderPipelineAsset pause;


        private void Awake()
        {
            Initialization.AddListener(this);
            Events.AddListener(this);

           
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        private void Update()
        {
            if(isPaused==true)
            {
                if (pauseHudMask != null)
                {
                    pauseHudMask.DOFade(.8f, 0.5f);
                }
            }
            else
            {
                if (pauseHudMask != null)
                {
                    pauseHudMask.DOFade(0, 0.5f);
                }
            }

        }


        private void OnDestroy()
        {
            Initialization.RemoveListener(this);
            Events.RemoveListener(this);
        }

        public void OnGlobalInitialization()
        {
            pauseHudMask.DOFade(0, 0.5f);
            gameObject.SetActive(false);
        }

        public void OnPauseMode()
        {
            if (m_isListeningInput)
            {
                isPaused = !isPaused;
                HFEventManager.TriggerEvent<bool>(HFEventID.OnPauseMode, isPaused);
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
