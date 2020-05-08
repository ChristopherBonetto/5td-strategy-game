using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventRaisedType
{
    OnGlobalInitialization,
    OnStepInitialization,
    OnStepCompleted,
}

[CreateAssetMenu(menuName = "Human Factor/Game Event Data")]
public class GameEventData : ScriptableObject
{
    private List<IHFTutorial> m_events = new List<IHFTutorial>();

    public void AddListener(IHFTutorial component)
    {
        m_events.Add(component);
    }

    public void RemoveListener(IHFTutorial component)
    {
        m_events.Remove(component);
    }

    public void RaiseEvent(EventRaisedType eventType)
    {
        switch (eventType)
        {
            case EventRaisedType.OnGlobalInitialization:
                for (int i = m_events.Count - 1; i >= 0; i--)
                {
                    m_events[i].OnGlobalInitialization();
                }
                break;

            case EventRaisedType.OnStepCompleted:
                for (int i = m_events.Count - 1; i >= 0; i--)
                {
                    m_events[i].OnStepCompleted();
                }
                break;

            case EventRaisedType.OnStepInitialization:
                for (int i = m_events.Count - 1; i >= 0; i--)
                {
                    m_events[i].OnStepInitialization();
                }
                break;
        }
    }
}
