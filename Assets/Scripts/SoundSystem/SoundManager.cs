using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Linq;

public class SoundManager : Singleton<SoundManager>
{
    public Dictionary<string, CustomEvent> EventsDictionary = new Dictionary<string, CustomEvent>();

    public Dictionary<string, Bus> BusesDictionary = new Dictionary<string, Bus>();

    [UnityEngine.SerializeField, EventRef] private string m_mainTrackPath;
    private CustomEvent m_mainTrackEvent;

    #region  Unity Event

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
            m_mainTrackEvent = GetEventFromDictionaryPath(m_mainTrackPath);
            m_mainTrackEvent.Play();
        }
    }

    #endregion

    #region CustomEvents

    #region Add event

    public void AddEventToDictionary(string inPath, CustomEvent inEvent)
    {
        EventsDictionary.Add(inPath, inEvent);
    }

    #endregion

    #region Get events

    public CustomEvent GetEventFromDictionaryIndex(int inValue)
    {
        return EventsDictionary.ElementAt(inValue).Value;
    }

    public CustomEvent GetEventFromDictionaryPath(string inPath)
    {
        if (EventsDictionary.ContainsKey(inPath))
        {
            return EventsDictionary[inPath];
        }
        return null;
    }

    #endregion

    #region Release events

    public void ReleaseEventsFromDicionary()
    {
        foreach(string key in EventsDictionary.Keys)
        {
            EventsDictionary[key].EventDescription.releaseAllInstances();
            EventsDictionary[key].EventDescription.unloadSampleData();
        }
        EventsDictionary.Clear();
    }

    #endregion

    #endregion

    #region Bus

    #region Get bus

    public Bus? GetBusFromName(string inName)
    {
        if (BusesDictionary.ContainsKey(inName))
        {
            return BusesDictionary[inName];
        }

        return null;
    }

    #endregion

    #region Set Bus

    public void SetBusValue(string inName, float inValue)
    {
        if (GetBusFromName(inName).HasValue)
        {
            inValue = Mathf.Clamp(inValue, 0, 1);
            BusesDictionary[inName].setVolume(inValue);
        }
    }

    public void SetSoundsBusVolume(float inValue)
    {
        SetBusValue("Sound", inValue);
    }

    public void SetMusicBusVolume(float inValue)
    {
        SetBusValue("Music", inValue);
    }

    public void SetAllAmbienceBusVolume(float inValue)
    {
        SetBusValue("Sound/Background", inValue);
    }

    public void SetFoleyBusVolume(float inValue)
    {
        SetBusValue("Sound/Foley", inValue);
    }

    public void SetSfxBusVolume(float inValue)
    {
        SetBusValue("Sound/SFX", inValue);
    }

    public void SetAmbienceBusVolume(float inValue)
    {
        SetBusValue("Sound/Background/Ambience", inValue);
    }

    public void SetUnderwaterBusVolume(float inValue)
    {
        SetBusValue("Sound/Background/Underwater", inValue);
    }

    #endregion

    #endregion
}
