using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HF.Refactoring;
using HF;
using UnityEngine.Events;

/*
* -----------------------------------------------------------------------------
* This class manage the tutorial pop-ups throw events. This class will be not 
* a singleton because it doesn't need to be.
* We track the right tutorial to perform storing a Queue of "TutorialPopUp",
* If the ID from the first of the queue correspond to the one triggered by an 
* event, then go to next one if there are any.
* -----------------------------------------------------------------------------
*/

public enum TutorialID
{
    Move_Camera,
    Rotate_Camera,
    Select_Unit,
    Move_Unit,
    Carry_Turret,
    Reposition_Turret,
    Specialize_Unit,
    Pause_Mode,
    Call_wave,
    Select_Castle,
    Create_Ally,
    Specialize_Turret,
    Upgrade_Unit,
    None,
}

/// <summary>
/// Handle the flow of the tutorial.
/// </summary>
public class HFTutorialHandler : MonoBehaviour
{
    [Tooltip("Link to this event all function to perform at the scene start")]
    public GameEventData SceneInitialization;


    [System.Serializable]
    public class TutorialStep
    {
        [Tooltip("Tutorial step id")]
        public TutorialID ID;

        [TextArea, Tooltip("Write the tutorial message to show in the box pop-ip in UI")]
        public string Message = "";

        public GameEventData Events;
    }


    [Tooltip("Collection that store all tutorial steps in sequence")]
    public TutorialStep[] TutorialSteps;

    private TutorialStep m_currentTutorialStep;
    private Queue<TutorialStep> m_tutorialSteps = new Queue<TutorialStep>();
    private HFTutorialPopUp m_popUp;

    // Debug helper
    private const string m_debugColor = "#DA70D6";

    public TutorialID TutorialID { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private void OnEnable()
    {
        HFEventManager.SubscribeTo<TutorialID>(HFEventID.OnTutorialQuestCompleted, OnTutorialQuestCompleted);
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<TutorialID>(HFEventID.OnTutorialQuestCompleted, OnTutorialQuestCompleted);

    }

    private void Start()
    {
        m_popUp = HFUIManager.Instance.Getwindow<HFUIHUD>(HFUIWindowID.HUD).Popup;

        LoadTutorialSteps();

        Dequeue();

        SceneInitialization.RaiseEvent(EventRaisedType.OnGlobalInitialization);
    }

    /// <summary>
    /// Laod every popup from the HUD window
    /// <see cref="HFUIHUD"/>
    /// </summary>
    private void LoadTutorialSteps()
    {
        foreach (TutorialStep step in TutorialSteps)
        {
            m_tutorialSteps.Enqueue(step);
        }
    }

    private void Dequeue()
    {
        if (m_tutorialSteps.Count > 0)
        {
            m_currentTutorialStep = m_tutorialSteps.Dequeue();

            // Reset pop up
            m_popUp.gameObject.SetActive(false);
            m_popUp.SetMessage(m_currentTutorialStep.Message);
            m_popUp.gameObject.SetActive(true);

            // Invoke step initialization.
            if (m_currentTutorialStep.Events != null)
                m_currentTutorialStep.Events.RaiseEvent(EventRaisedType.OnStepInitialization);
        }
        else
        {
            m_popUp.gameObject.SetActive(false);
        }
    }

    /*
    * Event that respond to the triggers,
    * If the parameter ID correspond to the pop-up
    * then go to the next one.
    */
    private void OnTutorialQuestCompleted(TutorialID id)
    {
        // If the IDs match
        if (m_currentTutorialStep.ID == id)
        {
            if (m_currentTutorialStep.Events != null)
                m_currentTutorialStep.Events.RaiseEvent(EventRaisedType.OnStepCompleted);

            Dequeue(); 
        }
    }
}
