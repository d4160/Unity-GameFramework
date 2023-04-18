using System;
using System.Collections.Generic;
using System.Linq;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace d4160.Events
{
    public interface IEventListener
    {
        void OnInvoked();
    }

    public interface IEventListenerCollection
    {
        void OnInvoked(int i);
    }

    public abstract class EventSOBase<T> : EventSOBase
    {
        private readonly List<IEventListener<T>> _listeners = new();

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

        public void RemoveListener(IEventListenerCollection<T> listenerCollection)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                if (_listeners[i] is IEventListenerWithCollection<T> wl)
                {
                    if (wl.ListenerCollection == listenerCollection)
                    {
                        _listeners.RemoveAt(i);
                    }
                }
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

        public readonly struct EventListener : IEventListener<T>
        {
            private readonly UnityAction<T> callback;

            public EventListener(UnityAction<T> callback)
            {
                this.callback = callback;
            }

            public void OnInvoked(T param)
            {
                callback?.Invoke(param);
            }
        }
    }

    public abstract class EventSOBase : ScriptableObject
    {

    }

    public static class EventExtensions
    {
        public static void AddListener<T>(this IList<EventSOBase<T>> source, IEventListenerCollection<T> eventListenerColl)
        {
            EventListenerCollection<T>[] listeners = new EventListenerCollection<T>[source.Count];
            for (int i = 0; i < source.Count; i++)
            {
                listeners[i] = new EventListenerCollection<T>(i, eventListenerColl);
                source[i].AddListener(listeners[i]);
            }
        }

        public static void RemoveListener<T>(this IList<EventSOBase<T>> source, IEventListenerCollection<T> eventListenerColl)
        {
            for (int i = 0; i < source.Count; i++)
            {
                source[i].RemoveListener(eventListenerColl);
            }
        }
    }

    public struct EventListenerCollection<T> : IEventListenerWithCollection<T>
    {
        public int i;
        public IEventListenerCollection<T> listenerColl;

        public IEventListenerCollection<T> ListenerCollection => listenerColl;

        public EventListenerCollection(int i, IEventListenerCollection<T> listenerColl)
        {
            this.i = i;
            this.listenerColl = listenerColl;
        }

        public void OnInvoked(T param)
        {
            listenerColl.OnInvoked(i, param);
        }
    }

    public interface IEventListener<T> 
    {
        void OnInvoked(T param);
    }

    public interface IEventListenerWithCollection<T> : IEventListener<T>
    {
        IEventListenerCollection<T> ListenerCollection { get; }
    }

    public interface IEventListenerCollection<T>
    {
        void OnInvoked(int i, T param);
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
                _listeners[i]?.OnInvoked(param1, param2);
            }
        }

        public readonly struct EventListener : IEventListener<T1, T2>
        {
            private readonly UnityAction<T1, T2> callback;

            public EventListener(UnityAction<T1, T2> callback)
            {
                this.callback = callback;
            }

            public void OnInvoked(T1 param1, T2 param2)
            {
                callback?.Invoke(param1, param2);
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

        public readonly struct EventListener : IEventListener<T1, T2,T3>
        {
            private readonly UnityAction<T1, T2, T3> callback;

            public EventListener(UnityAction<T1, T2, T3> callback)
            {
                this.callback = callback;
            }

            public void OnInvoked(T1 param1, T2 param2, T3 param3)
            {
                callback?.Invoke(param1, param2, param3);
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

        public readonly struct EventListener : IEventListener<T1, T2, T3, T4>
        {
            private readonly UnityAction<T1, T2, T3, T4> callback;

            public EventListener(UnityAction<T1, T2, T3, T4> callback)
            {
                this.callback = callback;
            }

            public void OnInvoked(T1 param1, T2 param2, T3 param3, T4 param4)
            {
                callback?.Invoke(param1, param2, param3, param4);
            }
        }
    }
}