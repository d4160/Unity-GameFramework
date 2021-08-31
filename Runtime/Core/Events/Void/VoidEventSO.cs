using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Events 
{
    [CreateAssetMenu (menuName = "d4160/Events/Void")]
    public class VoidEventSO : ScriptableObject 
    {
        private readonly List<IEventListener> _listeners = new List<IEventListener>();

        public void AddListener(IEventListener listener) 
        {
            if (!_listeners.Contains(listener)) {
                _listeners.Add(listener);
            }
        }

        public void RemoveListener(IEventListener listener) 
        {
            if (_listeners.Contains(listener)) {
                _listeners.Remove(listener);
            }
        }

        public void RemoveAllListeners() {
            _listeners.Clear();
        }

        [Button]
        public void Invoke() 
        {
            for (var i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].OnInvoked();
            }
        }
    }
}