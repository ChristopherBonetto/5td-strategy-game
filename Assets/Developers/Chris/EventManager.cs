using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    public delegate void Callback();
    public delegate void Callback<T>(T arg);
    public delegate void Callback<T, K>(T arg1, K arg2);

    private static Dictionary<EventID, Delegate> m_Events = new Dictionary<EventID, Delegate>();

    public static void SubscribeTo(EventID inEvent, Callback inHandler)
    {
        if (!m_Events.ContainsKey(inEvent)) m_Events.Add(inEvent, null);

        if (m_Events[inEvent] != null)
        {
            Delegate[] invocationList = ((Callback)m_Events[inEvent]).GetInvocationList();

            for (int i = 0; i < invocationList.Length; i++)
            {
                if ((Delegate)inHandler == invocationList[i])
                    return;
            }
        }

        m_Events[inEvent] = (Callback)m_Events[inEvent] + inHandler;
    }

    public static void SubscribeTo<T>(EventID inEvent, Callback<T> inHandler)
    {
        if (!m_Events.ContainsKey(inEvent)) m_Events.Add(inEvent, null);

        if (m_Events[inEvent] != null)
        {
            Delegate[] invocationList = ((Callback<T>)m_Events[inEvent]).GetInvocationList();

            for (int i = 0; i < invocationList.Length; i++)
            {
                if ((Delegate)inHandler == invocationList[i])
                    return;
            }
        }

        m_Events[inEvent] = (Callback<T>)m_Events[inEvent] + inHandler;
    }

    public static void SubscribeTo<T,K>(EventID inEvent, Callback<T,K> inHandler)
    {
        if (!m_Events.ContainsKey(inEvent)) m_Events.Add(inEvent, null);

        if (m_Events[inEvent] != null)
        {
            Delegate[] invocationList = ((Callback<T,K>)m_Events[inEvent]).GetInvocationList();

            for (int i = 0; i < invocationList.Length; i++)
            {
                if ((Delegate)inHandler == invocationList[i])
                    return;
            }
        }

        m_Events[inEvent] = (Callback<T,K>)m_Events[inEvent] + inHandler;
    }


    public static void UnsubscribeFrom(EventID inEvent, Callback inHandler)
    {
        if (m_Events.ContainsKey(inEvent))
        {
            m_Events[inEvent] = (Callback)m_Events[inEvent] - inHandler;

            if (m_Events[inEvent] == null) m_Events.Remove(inEvent);
        }
    }

    public static void UnsubscribeFrom<T>(EventID inEvent, Callback<T> inHandler)
    {
        if (m_Events.ContainsKey(inEvent))
        {
            m_Events[inEvent] = (Callback<T>)m_Events[inEvent] - inHandler;

            if (m_Events[inEvent] == null) m_Events.Remove(inEvent);
        }
    }

    public static void UnsubscribeFrom<T,K>(EventID inEvent, Callback<T,K> inHandler)
    {
        if (m_Events.ContainsKey(inEvent))
        {
            m_Events[inEvent] = (Callback<T,K>)m_Events[inEvent] - inHandler;

            if (m_Events[inEvent] == null) m_Events.Remove(inEvent);
        }
    }


    public static void TriggerEvent(EventID inEvent)
    {
        if (m_Events.ContainsKey(inEvent)) (m_Events[inEvent] as Callback)?.Invoke();
    }

    public static void TriggerEvent<T>(EventID inEvent, T arg)
    {
        if (m_Events.ContainsKey(inEvent)) (m_Events[inEvent] as Callback<T>)?.Invoke(arg);
    }

    public static void TriggerEvent<T,K>(EventID inEvent, T arg1, K arg2)
    {
        if (m_Events.ContainsKey(inEvent)) (m_Events[inEvent] as Callback<T,K>)?.Invoke(arg1, arg2);
    }

    public static bool Exists(EventID inEvent)
    {
        return m_Events.ContainsKey(inEvent);
    }
}
