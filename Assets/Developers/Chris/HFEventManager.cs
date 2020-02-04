using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFEventManager
{
    public delegate void Callback();
    public delegate void Callback<T>(T arg);
    public delegate void Callback<T, K>(T arg1, K arg2);

    private static Dictionary<HFEventID, Delegate> m_Events = new Dictionary<HFEventID, Delegate>();

    public static void SubscribeTo(HFEventID inEvent, Callback inHandler)
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

    public static void SubscribeTo<T>(HFEventID inEvent, Callback<T> inHandler)
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

    public static void SubscribeTo<T,K>(HFEventID inEvent, Callback<T,K> inHandler)
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


    public static void UnsubscribeFrom(HFEventID inEvent, Callback inHandler)
    {
        if (m_Events.ContainsKey(inEvent))
        {
            m_Events[inEvent] = (Callback)m_Events[inEvent] - inHandler;

            if (m_Events[inEvent] == null) m_Events.Remove(inEvent);
        }
    }

    public static void UnsubscribeFrom<T>(HFEventID inEvent, Callback<T> inHandler)
    {
        if (m_Events.ContainsKey(inEvent))
        {
            m_Events[inEvent] = (Callback<T>)m_Events[inEvent] - inHandler;

            if (m_Events[inEvent] == null) m_Events.Remove(inEvent);
        }
    }

    public static void UnsubscribeFrom<T,K>(HFEventID inEvent, Callback<T,K> inHandler)
    {
        if (m_Events.ContainsKey(inEvent))
        {
            m_Events[inEvent] = (Callback<T,K>)m_Events[inEvent] - inHandler;

            if (m_Events[inEvent] == null) m_Events.Remove(inEvent);
        }
    }


    public static void TriggerEvent(HFEventID inEvent)
    {
        if (m_Events.ContainsKey(inEvent)) (m_Events[inEvent] as Callback)?.Invoke();
    }

    public static void TriggerEvent<T>(HFEventID inEvent, T arg)
    {
        if (m_Events.ContainsKey(inEvent)) (m_Events[inEvent] as Callback<T>)?.Invoke(arg);
    }

    public static void TriggerEvent<T,K>(HFEventID inEvent, T arg1, K arg2)
    {
        if (m_Events.ContainsKey(inEvent)) (m_Events[inEvent] as Callback<T,K>)?.Invoke(arg1, arg2);
    }

    public static bool Exists(HFEventID inEvent)
    {
        return m_Events.ContainsKey(inEvent);
    }
}
