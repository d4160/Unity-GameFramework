using System.Collections.Generic;
using UnityEngine;
using d4160.Collections;
using d4160.Events;
using Doozy.Runtime.Signals;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.DoozyUI
{
    public class SignalEventsListenerMono : MonoBehaviour, IEventListenerCollection<Signal>
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [OnValueChanged("_OnSignalInspectorChanged"), Expandable]
#endif
        [SerializeField] private SignalEventSO[] _signalEvents;

#if UNITY_EDITOR
        private void _OnSignalInspectorChanged()
        {
            if (_signalEvents.Length != _eventsListeners.Count)
            {
                while (_signalEvents.Length < _eventsListeners.Count)
                {
                    _eventsListeners.RemoveLast();
                }

                while (_signalEvents.Length > _eventsListeners.Count)
                {
                    _eventsListeners.Add(default);
                }
            }
        }
#endif

        [SerializeField] private List<UltEvents.UltEvent<Signal>> _eventsListeners = new();

        private void OnEnable()
        {
            _signalEvents.AddListener(this);
        }

        private void OnDisable()
        {
            _signalEvents.RemoveListener(this);
        }

        public void OnInvoked(int i, Signal param)
        {
            _eventsListeners[i].Invoke(param);
        }
    }
}