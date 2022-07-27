using System.Collections;
using System.Collections.Generic;
using d4160.Coroutines;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;

namespace d4160.Events 
{
    [CreateAssetMenu (menuName = "d4160/Events/Void")]
    public class VoidEventSO : ScriptableObject 
    {
        [SerializeField] private float _delay;

        private readonly List<IEventListener> _listeners = new List<IEventListener>();

        private WaitForSeconds _wait;
        private Coroutine _storedRoutine;

        public float Delay 
        {
            get => _delay;
            set
            {
                if (_delay != value)
                {
                    _delay = value;
                    Setup();
                }
            }
        }

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

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Invoke() 
        {
            if (_delay <= 0)
                InvokeInternal();
            else
                _storedRoutine = CoroutineStarter.Instance.StartCoroutine(DelayedInvoke());
        }

        public void Invoke(float delay)
        {
            var old = _delay;
            Delay = delay;
            Invoke();
            Delay = old;
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void CancelInvoke()
        {
            if (_storedRoutine != null)
            {
                CoroutineStarter.Instance.StopCoroutine(_storedRoutine);
            }
        }

        public void Setup()
        {
            _wait = new WaitForSeconds(_delay);
        }

        private void InvokeInternal()
        {
            for (var i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].OnInvoked();
            }
        }

        private IEnumerator DelayedInvoke()
        {
            if (_wait == null)
                Setup();

            yield return _wait;

            InvokeInternal();

            _storedRoutine = null;
        }
    }
}