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

    public interface IEventListener<T1, T2>
    {
        void OnInvoked(T1 param1, T2 param2);
    }

    public interface IEventListener<T1, T2, T3>
    {
        void OnInvoked(T1 param1, T2 param2, T3 param3);
    }

    public interface IEventListener<T1, T2, T3, T4>
    {
        void OnInvoked(T1 param1, T2 param2, T3 param3, T4 param4);
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
        private readonly List<IEventListener<T1, T2>> _listeners = new List<IEventListener<T1, T2>>();

        public void AddListener(IEventListener<T1, T2> listener)
        {
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }

        public void RemoveListener(IEventListener<T1, T2> listener)
        {
            if (_listeners.Contains(listener))
            {
                _listeners.Remove(listener);
            }
        }

        public void RemoveAllListeners()
        {
            _listeners.Clear();
        }

        public void Invoke(T1 param1, T2 param2)
        {
            for (var i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].OnInvoked(param1, param2);
            }
        }
    }

    public abstract class EventSOBase<T1, T2, T3> : ScriptableObject
    {
        private readonly List<IEventListener<T1, T2, T3>> _listeners = new List<IEventListener<T1, T2, T3>>();

        public void AddListener(IEventListener<T1, T2, T3> listener)
        {
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }

        public void RemoveListener(IEventListener<T1, T2, T3> listener)
        {
            if (_listeners.Contains(listener))
            {
                _listeners.Remove(listener);
            }
        }

        public void RemoveAllListeners()
        {
            _listeners.Clear();
        }

        public void Invoke(T1 param1, T2 param2, T3 param3)
        {
            for (var i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].OnInvoked(param1, param2, param3);
            }
        }
    }

    public abstract class EventSOBase<T1, T2, T3, T4> : ScriptableObject
    {
        private readonly List<IEventListener<T1, T2, T3, T4>> _listeners = new List<IEventListener<T1, T2, T3, T4>>();

        public void AddListener(IEventListener<T1, T2, T3, T4> listener)
        {
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }

        public void RemoveListener(IEventListener<T1, T2, T3, T4> listener)
        {
            if (_listeners.Contains(listener))
            {
                _listeners.Remove(listener);
            }
        }

        public void RemoveAllListeners()
        {
            _listeners.Clear();
        }

        public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            for (var i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].OnInvoked(param1, param2, param3, param4);
            }
        }
    }
}