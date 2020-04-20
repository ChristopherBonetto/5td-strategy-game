using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HF.Refactoring;
using HF;

//-----------------------------------------------------------------------------
// This class manage the tutorial pop-ups throw events. This class will be not 
// a singleton because it doesn't need to be.
// We track the right tutorial to perform storing an enum value that works as
// an ID. If the event triggered is equal to this ID than the tutorial is 
// completed and go to the next one.
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
    private List<HFTutorialPopUp> m_popups = new List<HFTutorialPopUp>();

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
    }

    private void Start()
    {
        LoadTutorialPopups();

        if (m_popups.Count > 0)
        {
            m_popups[0].gameObject.SetActive(true);
        }
    }

    private void LoadTutorialPopups()
    {
        HFUIHUD hud = HFUIManager.Instance.Getwindow<HFUIHUD>(HFUIWindowID.HUD);

        foreach (HFTutorialPopUp popUp in hud.Popups)
        {
            m_popups.Add(popUp);
        }
    }

    private void OnTutorialQuestCompleted(TutorialID id)
    {
        if (m_popups.Count == 0)
        {
            // Triggered the win or wait the wave ends.
            return;
        }

        TutorialID tempID = m_popups[0].ID;

        if (id == tempID)
        {
            m_popups[0].gameObject.SetActive(false);    // maybe start an animation
            m_popups.RemoveAt(0);

            if (m_popups.Count == 0)
            {
                // Triggered the win or wait the wave ends.
                Debug.Log($"<color={m_debugColor}><b>[{this.GetType().Name}]</b></color> : End tutorial!");
                gameObject.SetActive(false);
                return;
            }

            m_popups[0].gameObject.SetActive(true);
        }
    }

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
}
