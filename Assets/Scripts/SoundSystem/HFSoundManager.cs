using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Linq;

public class HFSoundManager : Singleton<HFSoundManager>
{
    new public static HFSoundManager Instance
    {
        get
        {
            if (applicationIsQuitting)
                return null;

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (HFSoundManager)FindObjectOfType(typeof(HFSoundManager));


                    if (_instance == null)
                    {
                        GameObject outGO = Instantiate(Resources.Load<GameObject>("Managers/SoundManager"));
                        _instance = outGO.GetComponent<HFSoundManager>();

                        DontDestroyOnLoad(_instance);
                    }
                    else
                        DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }
    }

    public Dictionary<string, List<HFCustomEvent> > EventsDictionary = new Dictionary<string, List<HFCustomEvent> >();

    public Dictionary<string, Bus> BusesDictionary = new Dictionary<string, Bus>();

    [FMODUnity.EventRef]
    public string StartGameAndWarRoomMusicPath;
    private HFCustomEvent StartGameAndWarRoomMusicEvent;

    #region Behaviour Cycle
    private void OnEnable()
    {
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, GameStateChanged);
    }
    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, GameStateChanged);
    }
    #endregion

    #region Events Dictionary

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

    #endregion

    #endregion

    public void GameStateChanged(GameStates inState)
    {
        switch (inState)
        {
            case GameStates.None:
                break;

            case GameStates.LoadStartingInfo:
                break;

            case GameStates.StartGame:
                TakeStartGameAndWarRoomSound(true);
                break;

            case GameStates.WarRoom:
                TakeStartGameAndWarRoomSound(true);
                break;

            case GameStates.InitializeLevel:
                TakeStartGameAndWarRoomSound(false);
                break;

            case GameStates.PlayingLevel:
                break;

            case GameStates.EndLevel:
                break;

            case GameStates.Pause:
                break;
        }
    }

    private void TakeStartGameAndWarRoomSound(bool wantToPlay)
    {
        if (StartGameAndWarRoomMusicEvent == null)
        {
            StartGameAndWarRoomMusicEvent = GetFreeEventFromDictionaryKey(StartGameAndWarRoomMusicPath);
        }

        if (StartGameAndWarRoomMusicEvent != null)
        {
            if (!StartGameAndWarRoomMusicEvent.isPlaying() && wantToPlay)
            {
                StartGameAndWarRoomMusicEvent.Play();
            }
            else if(StartGameAndWarRoomMusicEvent.isPlaying() && !wantToPlay)
            {
                StartGameAndWarRoomMusicEvent.Stop();
            }
        }
        else
        {
            Debug.LogError("Can't find : " + StartGameAndWarRoomMusicEvent.ToString());
        }
    }
}