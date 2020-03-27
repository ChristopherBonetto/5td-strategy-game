using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public class HFFmodDatabase : Singleton<HFFmodDatabase>
{
    //STUDIO VARIABLES
    private FMOD.Studio.System m_system = new FMOD.Studio.System();

    //BANKS VARIABLES
    [SerializeField, BankRef]
    private string[] m_banksPath = null;
    private List<Bank> m_loadedBanks = new List<Bank>();

    private const string assetsPath = "/FmodBuilds/Desktop/";
    private const string bankExtension = ".bank";

    private int m_pendingEvents = 0;

    //BUSS VARIABLES
    private const string m_busPrefix = "bus:/";
    [SerializeField] private string[] m_busesNames;

    #region Behaviour Cycle

    private void Awake()
    {
        //base.Awake();
        InitializeStudio();
        FindBanks();
    }

    private void Start()
    {
        FindBuses();
    }

    private void Update()
    {
        m_system.update();
    }

    

    private void OnApplicationQuit()
    {
        ReleaseFmodFromDatabase();
    }

    #endregion

    #region Initialize Studio

    //Check and initialize System studio
    public void InitializeStudio()
    {
        FMOD.RESULT res = FMOD.RESULT.OK;
        if (!m_system.isValid())
        {
            res = FMOD.Studio.System.create(out m_system);
            if (res != FMOD.RESULT.OK)
            {
                Debug.LogError("Can't create file system " + res);
                return;
            }

            res = m_system.initialize(256, INITFLAGS.ALLOW_MISSING_PLUGINS | INITFLAGS.SYNCHRONOUS_UPDATE, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
            if (res != FMOD.RESULT.OK)
            {
                Debug.LogError(res);
                return;
            }
        }
    }

    #endregion

    #region Initialize Banks

    //Try to find all the banks from paths.
    private void FindBanks()
    {
        FMOD.RESULT res = FMOD.RESULT.OK;
        int counter = 0;

        foreach (string bankPath in m_banksPath)
        {
            if(bankPath != "")
            {
                Bank tempBank;
                string _finalPath = Application.dataPath + assetsPath + bankPath + bankExtension;
                res = m_system.loadBankFile(_finalPath, LOAD_BANK_FLAGS.NONBLOCKING, out tempBank);

                if (res != FMOD.RESULT.OK)
                {
                    Debug.LogError("can't load this bank " + res);
                }

                counter++;

                //Check if all banks finded are in pending to be elaborate. If YES start coroutine.
                StartCoroutine(LoadPendingBank(tempBank));
            }
        }
    }

    //Check and elaborate all banks in pending
    IEnumerator LoadPendingBank(Bank inBank)
    {
        LOADING_STATE _loadingState = LOADING_STATE.ERROR;

        while (_loadingState != LOADING_STATE.LOADED)
        {
            Debug.Log("Try to load bank");
            FMOD.RESULT res = inBank.getLoadingState(out _loadingState);
            yield return null;
        }
        Debug.Log("Bank loaded");
        m_loadedBanks.Add(inBank);

        if(CheckBanks(m_loadedBanks.Count, null, m_banksPath))
        {
            Debug.Log("LOADED ALL BANKS");
            FindEvents();
        }
        yield return null;
    }

    public bool CheckBanks(int inValue, List<Bank> inBanksList = null, string[] inBankArray = null)
    {
        return inValue == m_banksPath.Length || inValue == inBankArray.Length;
    }

    #endregion

    #region Initialize Events

    //Find all events descriptions from different banks and start a coroutine to elaborate them.
    private void FindEvents()
    {
        FMOD.RESULT result = FMOD.RESULT.ERR_INVALID_HANDLE;

        foreach (Bank bank in m_loadedBanks)
        {
            result = bank.getEventList(out EventDescription[] pendingEventDesc);
            m_pendingEvents += pendingEventDesc.Length;

            if (result != FMOD.RESULT.OK)
            {
                bank.getPath(out string bankPath);
                Debug.LogError("Can't find event descriptions in : " + bankPath + ": " + result.ToString());
                continue;
            }
            if(m_pendingEvents > 0)
            {
                Bus[] buses;
                result = bank.getBusList(out buses);
                Debug.Log("Finded : " + m_pendingEvents + " Events and " + buses.Length + " Buses");
            }
            StartCoroutine(LoadPendingEventDescriptions(pendingEventDesc));
        }
    }
    
    //Take all pending events and try to add them to the final list. 
    private IEnumerator LoadPendingEventDescriptions(EventDescription[] inPendingEvents)
    {
        List<HFCustomEvent> tempList = new List<HFCustomEvent>();

        foreach (EventDescription description in inPendingEvents)
        {
            description.loadSampleData();
            
            LOADING_STATE tempState = LOADING_STATE.ERROR;
            do
            {
                description.getSampleLoadingState(out tempState);
                yield return new UnityEngine.WaitForEndOfFrame();
            }
            while (tempState != LOADING_STATE.LOADED);

            m_pendingEvents--;

            tempList.Add(new HFCustomEvent(description));
            yield return null;
        }
        //If all events finded from banks as been loaded right so READY.
        if(m_pendingEvents == 0)
        {
            yield return new WaitForSeconds(0.5f);

            HFEventManager.TriggerEvent<bool>(HFEventID.OnFinishedLoadEvents, true);
        }
    }

    //public void AddEventToDictionary(string inPath, CustomEvent inEvent)
    //{
    //    EventsDictionary.Add(inPath, inEvent);
    //}

    #endregion

    #region Initialize Bus

    //Find all buses from RuntimeManager checking if their name if equal to the bus prefix + the bus name.
    public void FindBuses()
    {
        HFSoundManager tempRef = HFSoundManager.Instance;

        for (int i = 0; i < m_busesNames.Length; i++)
        {
            Bus tempBus = new Bus();

            tempBus = RuntimeManager.GetBus(m_busPrefix + m_busesNames[i]);

            tempRef.BusesDictionary.Add(m_busesNames[i], tempBus);
        }
    }

    #endregion

    #region Release Fmod events, banks and system

    //Tell to fmod that it will must release all his references to events, bank and system.
    private void ReleaseFmodFromDatabase()
    {
        HFSoundManager.Instance.ReleaseEventsFromDicionary();

        foreach (Bank bank in m_loadedBanks)
        {
            bank.unload();
        }
        m_loadedBanks.Clear();

        if (m_system.isValid())
        {
            m_system.release();
        }
    }

    #endregion
}

