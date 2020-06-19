using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HF.Refactoring;

public class HFCallNextWaveB : HFButton, IHFTutorial
{
    public GameEventData Events;
    public GameEventData Initialization;
    public TutorialID TutorialID { get; set; } = TutorialID.Call_wave;

    // Tutorial variables
    private bool m_tutorialMatch = true;


    private void Awake()
    {
        Events.AddListener(this);
        Initialization.AddListener(this);
    }

    private void OnDestroy()
    {
        Events.RemoveListener(this);
        Initialization.RemoveListener(this);
    }

    protected override void OnDisable()
    {
        Reset();
        base.OnDisable();
    }

    public void CallNextWave()
    {
        if (m_isListeningInput && m_tutorialMatch)
        {
            HFEventManager.TriggerEvent(HFEventID.OnWaveBeginned);
            HFEventManager.TriggerEvent(HFEventID.OnTutorialQuestCompleted, TutorialID.Call_wave);
        }
    }

    public void OnGlobalInitialization()
    {
        m_tutorialMatch = false;
        this.gameObject.SetActive(false);
    }

    public void OnStepInitialization()
    {
        m_tutorialMatch = true;
        this.gameObject.SetActive(true);
        // Turn on UI feedbacks.
    }

    public void OnStepCompleted()
    {
        
    }

    public void Reset()
    {
        m_tutorialMatch = true;
    }
}
