using System;
using System.Collections.Generic;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;

namespace d4160.Events
{
    public interface IEventListener
    {
        void OnInvoked();
    }

    public abstract class EventSOBase<T> : ScriptableObject
    {
        private readonly List<IEventListener<T>> _listeners = new List<IEventListener<T>>();

        public void AddListener(IEventListener<T> listener) 
        {
            if (!_listeners.Contains(listener)) {
                _listeners.Add(listener);
            }
        }

        public void RemoveListener(IEventListener<T> listener) 
        {
            if (_listeners.Contains(listener)) {
                _listeners.Remove(listener);
            }
        }

        public void RemoveAllListeners() {
            _listeners.Clear();
        }

        public void Invoke(T obj) 
        {
            for (var i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].OnInvoked(obj);
            }
        }
    }

    public interface IEventListener<T> 
    {
        void OnInvoked(T param);
    }

    // *** When need two or more events with the same generic type in one class, use structs to hold the implamentation passing the dependencies ***
    // public struct EventListener<T> : IEventListener<T>
    // {
    //     public void OnInvoked(T param)
    //     {
    //     }
    // }

    public abstract class EventSOBase<T1, T2> : ScriptableObject
    {
        public event Action<T1, T2> OnEvent;

        public void Invoke (T1 obj1, T2 obj2) {
            OnEvent?.Invoke (obj1, obj2);
        }
    }

    public abstract class EventSOBase<T1, T2, T3> : ScriptableObject
    {
        public event Action<T1, T2, T3> OnEvent;

        public void Invoke (T1 obj1, T2 obj2, T3 obj3) {
            OnEvent?.Invoke (obj1, obj2, obj3);
        }
    }

    public abstract class EventSOBase<T1, T2, T3, T4> : ScriptableObject
    {
        public event Action<T1, T2, T3, T4> OnEvent;

        public void Invoke (T1 obj1, T2 obj2, T3 obj3, T4 obj4) {
            OnEvent?.Invoke (obj1, obj2, obj3, obj4);
        }
    }
}