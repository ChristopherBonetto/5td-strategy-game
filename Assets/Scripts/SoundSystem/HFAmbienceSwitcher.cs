using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Ambience
{
    OutdoorNight,
    Underwater,
    Cave
}

public class HFAmbienceSwitcher : Singleton<HFAmbienceSwitcher>
{
    [UnityEngine.SerializeField, EventRef] private string m_ambienceSwitcherPath;
    private HFCustomEvent m_ambienceSwitcherEvent;

    private Ambience m_currentAmbience = Ambience.OutdoorNight;

    private void OnEnable()
    {
        HFEventManager.SubscribeTo<bool>(HFEventID.OnFinishedLoadEvents, TakeEvents);
    }
    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<bool>(HFEventID.OnFinishedLoadEvents, TakeEvents);
    }

    public void TakeEvents(bool inValue)
    {
        if (inValue)
        {
            HFSoundManager tempIstance = HFSoundManager.Instance;

            m_ambienceSwitcherEvent = tempIstance.GetEventFromDictionaryPath(m_ambienceSwitcherPath);
            m_ambienceSwitcherEvent.Play();
        }
    }


    public void ChangeParamToAmbience(string inParamName, float inValue)
    {
        if (m_ambienceSwitcherEvent.CheckParamInList(inParamName))
        {
            m_ambienceSwitcherEvent.ChangeParamFromName(inParamName, inValue);
        }
    }

    public void ChangeAmbience(Ambience inNewAmbience)
    {
        if(inNewAmbience == Ambience.OutdoorNight)
        {
            m_ambienceSwitcherEvent.ChangeParamFromName("ambience_index", 0);
            m_ambienceSwitcherEvent.ChangeParamFromName("underwater", 0);
        }
        else if(inNewAmbience == Ambience.Cave)
        {
            m_ambienceSwitcherEvent.ChangeParamFromName("ambience_index", 1);
            m_ambienceSwitcherEvent.ChangeParamFromName("underwater", 0);
        }
        else if(inNewAmbience == Ambience.Underwater)
        {
            m_ambienceSwitcherEvent.ChangeParamFromName("underwater", 1);
        }
        m_currentAmbience = inNewAmbience;
    }
}
