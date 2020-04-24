using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HF.Refactoring;

public class HFCallNextWaveB : HFButton
{
    private TutorialID m_tutorialID = TutorialID.None;
    private bool m_isMatchingTutorialID;

    protected override void OnEnable()
    {
        base.OnEnable();

        // Hardcode the reference about level tutorial
        if (HFScenesManager.Instance.CurrentLevelSelected != null && HFScenesManager.Instance.CurrentLevelSelected.LevelScene.name == "Level_00")
        {
            // If it's in the tutorial level, we don't want to call nezt wave until the tutorial pop up say that.
            m_tutorialID = TutorialID.Call_wave;
            HFEventManager.SubscribeTo<TutorialID>(HFEventID.OnTutorialQuestOn, OnTutorialQuestOn);
        }
        else
        {
            // If it's not the tutorial level, reset value. Now the next wave can be called at any moments.
            ResetListeningInputCondition();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        // Unsubscribe from the event only if the player is in the tutorial level. (same as above)
        if (HFScenesManager.Instance.CurrentLevelSelected != null && HFScenesManager.Instance.CurrentLevelSelected.LevelScene.name == "Level_00")
        {
            HFEventManager.UnsubscribeFrom<TutorialID>(HFEventID.OnTutorialQuestOn, OnTutorialQuestOn);
        }
    }

    public void CallNextWave()
    {
        if (m_isListeningInput)
        {
            HFEventManager.TriggerEvent(HFEventID.OnWaveBeginned);
        }
    }

    private void ResetListeningInputCondition()
    {
        m_tutorialID = TutorialID.None;
        m_isMatchingTutorialID = true;
    }

    private void OnTutorialQuestOn(TutorialID id)
    {
        m_isMatchingTutorialID = m_tutorialID == id || m_tutorialID == TutorialID.None;
        m_isListeningInput = m_isMatchingWindowID && m_isMatchingTutorialID;
    }

    protected override void IsMatchingWiindowID(HFUIWindowID id)
    {
        m_isMatchingWindowID = MyWindowID == id;
        m_isListeningInput =  m_isMatchingWindowID && m_isMatchingTutorialID;
    }
}
