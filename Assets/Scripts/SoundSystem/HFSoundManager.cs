using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Linq;

public class HFSoundManager : Singleton<HFSoundManager>
{
    public Dictionary<string, List<HFCustomEvent> > EventsDictionary = new Dictionary<string, List<HFCustomEvent> >();

    public Dictionary<string, Bus> BusesDictionary = new Dictionary<string, Bus>();


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            DebugDictionary();
            
        }
    }


    #region Dictionary

    #region Add dictionary event

    public void AddNewElementToDictionary(string inPath, HFCustomEvent inEventsList)
    {
        if (!EventsDictionary.ContainsKey(inPath))
        {
            List<HFCustomEvent> tempList = new List<HFCustomEvent>();
            tempList.Add(inEventsList);
            EventsDictionary.Add(inPath, tempList);
        }
        else
        {
            EventsDictionary[inPath].Add(inEventsList);
        }
        Debug.Log("Added " + EventsDictionary[inPath].Count + " " + inPath);
    }

    #endregion

    #region Get events

    public HFCustomEvent GetFreeEventFromDictionaryKey(string inPath)
    {
        if (EventsDictionary.ContainsKey(inPath))
        {
            EventDescription tempDesc = EventsDictionary[inPath].ElementAt(0).EventDescription;

            foreach(HFCustomEvent instance in EventsDictionary[inPath])
            {
                if (!instance.isPlaying())
                {
                    return instance;
                }
            }
            return new HFCustomEvent(tempDesc); 
        }
        return null;
    }

    #endregion

    public void DebugDictionary()
    {
        foreach(string path in EventsDictionary.Keys)
        {
            Debug.Log(path + " have " + EventsDictionary[path].Count);
        }
    }

    #region Release events

    public void ReleaseEventsFromDicionary()
    {
        foreach (string key in EventsDictionary.Keys)
        {
            foreach(HFCustomEvent instance in EventsDictionary[key])
            {
                instance.EventDescription.releaseAllInstances();
                instance.EventDescription.unloadSampleData();
            }
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
