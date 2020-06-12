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

    #region Initialize Custom event

    //Initializa a new Custom Event from description.
    public HFCustomEvent(EventDescription inDescription)
    {
        this.m_eventDescription = inDescription;

        if (TakeAllInfoFromDescription())
        {
            m_eventDescription.createInstance(out EventInstance instance);
            m_eventIstance = instance;
            HFSoundManager.Instance.AddNewElementToDictionary(EventPath, this);
        }
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

    #endregion

    #region Control custom event

    public bool isPlaying()
    {
        m_eventIstance.getPlaybackState(out PLAYBACK_STATE playingState);
        return m_eventIstance.isValid() && (playingState != PLAYBACK_STATE.STOPPED);
    }
    
    public bool CheckForErrors()
    {
        return !m_eventDescription.isValid() || !m_eventDescription.hasHandle();
    }

    public void Play()
    {
        if(!isPlaying())
        m_eventIstance.start();
    }

    public void PlayOneShootAttached(GameObject inObj)
    {
        if (!isPlaying())
        {
            RuntimeManager.PlayOneShotAttached(EventPath, inObj);
        }
    }

    

    public void Stop()
    {
        m_eventIstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    #region Parameters

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

    #endregion

    #endregion

}
