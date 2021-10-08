using d4160.MonoBehaviourData;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UltEvents;
using UnityEngine;

namespace d4160.Events {
    public class VoidEventBehaviour : MonoBehaviourData<VoidEventSO>, IEventListener {
        [SerializeField] private UltEvent _onEvent;

        void OnEnable () => _data?.AddListener(this);

        void OnDisable () => _data?.RemoveListener(this);

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Invoke () => _data?.Invoke();

        void IEventListener.OnInvoked() => _onEvent?.Invoke();
    }
}