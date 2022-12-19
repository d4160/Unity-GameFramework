using d4160.MonoBehaviours;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UltEvents;
using UnityEngine;

namespace d4160.Events {
    public class VoidEventBehaviour : MonoBehaviourData<VoidEventSO>, IEventListener 
    {
        [SerializeField] private UltEvent _onEvent;

        void OnEnable() { if (_data) _data.AddListener(this); }

        void OnDisable() { if (_data) _data.RemoveListener(this); }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Invoke() { if (_data) _data.Invoke(); }

        void IEventListener.OnInvoked() => _onEvent?.Invoke();
    }
}