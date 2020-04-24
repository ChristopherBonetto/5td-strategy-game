using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HF.Refactoring;
using HF;

//-----------------------------------------------------------------------------
// This class manage the tutorial pop-ups throw events. This class will be not 
// a singleton because it doesn't need to be.
// We track the right tutorial to perform storing a Queue of "TutorialPopUp",
// If the ID from the first of the queue correspond to the one triggered by an 
// event, then go to next one if there are any.
//-----------------------------------------------------------------------------

public enum TutorialID
{
    Move_Camera,
    Rotate_Camera,
    Select_Unit,
    Move_Unit,
    Carry_Turret,
    Reposition_Turret,
    Upgrade_Unit,
}

public class HFTutorialManager : MonoBehaviour
{
    // Queue of tutorials to show 
    private Queue<HFTutorialPopUp> m_popups = new Queue<HFTutorialPopUp>();

    // Debug helper
    private const string m_debugColor = "#DA70D6";


    private void OnEnable()
    {
        HFEventManager.SubscribeTo<TutorialID>(HFEventID.OnTutorialQuestCompleted, OnTutorialQuestCompleted);
        HFEventManager.SubscribeTo<HFUnit, int>(HFEventID.OnUnitSelected, OnUnitSelected);
        HFEventManager.SubscribeTo<HFUnit, int>(HFEventID.OnUnitUpgraded, OnUnitUpgraded);
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<TutorialID>(HFEventID.OnTutorialQuestCompleted, OnTutorialQuestCompleted);
        HFEventManager.UnsubscribeFrom<HFUnit, int>(HFEventID.OnUnitSelected, OnUnitSelected);
        HFEventManager.UnsubscribeFrom<HFUnit, int>(HFEventID.OnUnitUpgraded, OnUnitUpgraded);

        HFUIHUD hud = HFUIManager.Instance.Getwindow<HFUIHUD>(HFUIWindowID.HUD);
        foreach (HFTutorialPopUp popUp in hud.Popups)
        {
            popUp.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        LoadTutorialPopups();

        if (m_popups.Count > 0)
        {
            m_popups.Peek().gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Laod every popup from the HUD window
    /// <see cref="HFUIHUD"/>
    /// </summary>
    private void LoadTutorialPopups()
    {
        HFUIHUD hud = HFUIManager.Instance.Getwindow<HFUIHUD>(HFUIWindowID.HUD);

        foreach (HFTutorialPopUp popUp in hud.Popups)
        {
            m_popups.Enqueue(popUp);
        }
    }


    // Event that respond to the triggers,
    // If the parameter ID correspond to the pop-up
    // then go to the next one.
    private void OnTutorialQuestCompleted(TutorialID id)
    {
        // No pop-up available.
        if (m_popups.Count == 0)
        {
            // Triggered the win or wait the wave ends.
            return;
        }


        // Peek the ID from the queue 
        TutorialID tempID = m_popups.Peek().ID;


        // If the IDs match
        if (id == tempID)
        {
            // Remove from the queue
            m_popups.Dequeue().gameObject.SetActive(false); 


            // Check if the collection is empty
            if (m_popups.Count == 0)
            {
                Debug.Log($"<color={m_debugColor}><b>[{this.GetType().Name}]</b></color> : End tutorial!");

                // TTurn off this gameObejct, so it doesn't listen to event anymore.
                gameObject.SetActive(false);
                return;
            }


            // Turn on the next one
            m_popups.Peek().gameObject.SetActive(true);
        }
    }

    #region Events

    private void OnUnitSelected(HFUnit unit, int team)
    {
        if (team == HFGameParameters.PlayerTeam)
        {
            HFEventManager.TriggerEvent<TutorialID>(HFEventID.OnTutorialQuestCompleted, TutorialID.Select_Unit);
        }
    }

    private void OnUnitUpgraded(HFUnit unit, int team)
    {
        if (team == HFGameParameters.PlayerTeam)
        {
            HFEventManager.TriggerEvent<TutorialID>(HFEventID.OnTutorialQuestCompleted, TutorialID.Upgrade_Unit);
        }
    }

    #endregion
}
