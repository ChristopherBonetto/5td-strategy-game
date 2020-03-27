using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

[System.Serializable]
public class HFCustomEvent
{
    [UnityEngine.SerializeField, EventRef] private string m_eventPath = "";
    public string EventPath
    {
        get
        {
            return m_eventPath;
        }
        private set
        {
            if(value == null || value == "")
            {
                Debug.LogError("NO PATH");
            }

            m_eventPath = value;
        }
    }

    private System.Guid m_ID;
    public System.Guid ID
    {
        get
        {
            return m_ID;
        }
        private set
        {
            if (value == null || value == System.Guid.Empty)
            {
                Debug.LogError("NO ID");
            }

            m_ID = value;
        }
    }

    private List<PARAMETER_DESCRIPTION> m_paramsDesc;

    private EventDescription m_eventDescription;
    public EventDescription EventDescription { get => m_eventDescription; }

    private EventInstance m_eventIstance = new EventInstance();
    public EventInstance EventIstance { get => m_eventIstance; }

    public HFCustomEvent(EventDescription inDescription)
    {
        this.m_eventDescription = inDescription;

        if (TakeAllInfoFromDescription())
        {
            m_eventIstance = RuntimeManager.CreateInstance(m_eventPath);
            HFSoundManager.Instance.AddEventToDictionary(EventPath, this);
        }
    }
    
    public bool CheckForErrors()
    {
        return !m_eventDescription.isValid() || !m_eventDescription.hasHandle();
    }

    public bool TakeAllInfoFromDescription()
    {
        FMOD.RESULT res;

        res = m_eventDescription.getParameterDescriptionCount(out int paramsFinded);

        m_paramsDesc = new List<PARAMETER_DESCRIPTION>();

        //Get all parameters from event
        for (int i = 0; i < paramsFinded; i++)
        {
            res = m_eventDescription.getParameterDescriptionByIndex(i, out PARAMETER_DESCRIPTION param);

            if (res != FMOD.RESULT.OK)
            {
                return false;
            }

            m_paramsDesc.Add(param);
        }
        

        //Get event description and path
        if (res == FMOD.RESULT.OK)
        {
            res = m_eventDescription.getID(out m_ID);
            res = m_eventDescription.getPath(out m_eventPath);
        }

        return m_eventPath != null && m_ID != null;
    }


    

    public void Play()
    {
        m_eventIstance.start();
    }

    public void Stop()
    {
        m_eventIstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }


    public void ChangeParamFromName(string inName, float value)
    {
        if(CheckParamInList(inName))
        {
            m_eventIstance.setParameterByName(inName, value);
        }
        else
        {
            Debug.LogError("Can't change parameter (" + inName + ") value because doesn't finded :");
        }
    }

    public void DebugAllParams()
    {
        if(m_paramsDesc.Count > 0)
        {
            for(int i = 0; i < m_paramsDesc.Count; i++)
            {
                Debug.Log((string)m_paramsDesc[i].name);
            }
        }
    }

    public bool CheckParamInList(string inName)
    {
        if (m_paramsDesc.Count > 0)
        {
            for (int i = 0; i < m_paramsDesc.Count; i++)
            {
                if (inName == (string)m_paramsDesc[i].name)
                {
                    return true;
                }
            }
        }
        return false;
    }
    

    public float GetParamValue(string inName)
    {
        if (CheckParamInList(inName))
        {
            m_eventIstance.getParameterByName(inName, out float value);
            Debug.Log(value);
            return value;
        }
        else
        {
            Debug.LogError("Can't get parameter (" + inName + ") value because doesn't finded :");
            return 0;
        }
    }

    public void SetEventVolume(float inVolume)
    {
        m_eventIstance.setVolume(inVolume);
    }

    #region Deprecated Methods

    //public void AddParameters(string inParams)
    //{
    //    m_params.Add(inParams);
    //}

    //public void SetID(System.Guid inID)
    //{
    //    m_ID = inID;
    //}

    //public void TakeParamsList(List<string> inList)
    //{
    //    m_params = inList;
    //}

    //public void SetPath(string inPath)
    //{
    //    m_eventPath = inPath;
    //}

    #endregion
}
