using System;
using System.Collections.Generic;
using UnityEngine;
public class EventManager : MonoSingleton<EventManager>
{
    private Dictionary<int, Delegate> m_EventHandlers = new Dictionary<int, Delegate>();


    public void RegisterEvent<TEventArgs>(EventHandler<TEventArgs> handler) where TEventArgs : EventArgs
    {
        int eventId = typeof(TEventArgs).GetHashCode();
        if (m_EventHandlers.ContainsKey(eventId))
        {
            m_EventHandlers[eventId] = (EventHandler<TEventArgs>)m_EventHandlers[eventId] + handler;
        }
        else
        {
            m_EventHandlers.Add(eventId, handler);
        }
    }


    public void UnRegisterEvent<TEventArgs>(EventHandler<TEventArgs> handler) where TEventArgs : EventArgs
    {
        int eventId = typeof(TEventArgs).GetHashCode();
        if (m_EventHandlers.ContainsKey(eventId))
        {
            m_EventHandlers[eventId] = (EventHandler<TEventArgs>)m_EventHandlers[eventId] - handler;
            if (m_EventHandlers[eventId] == null)
            {
                m_EventHandlers.Remove(eventId);
            }
        }
    }


    public void TriggerEvent<TEventArgs>(object sender, TEventArgs args) where TEventArgs : EventArgs
    {
        int eventId = typeof(TEventArgs).GetHashCode();
        if (m_EventHandlers.ContainsKey(eventId))
        {
            // 在触发事件前，安全地进行类型转换
            EventHandler<TEventArgs> handler = m_EventHandlers[eventId] as EventHandler<TEventArgs>;
            handler?.Invoke(sender, args);
        }
    }

    public void Clear()
    {
        m_EventHandlers.Clear();
    }
}