using d4160.MonoBehaviourData;
using d4160.Variables;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UltEvents;
using UnityEngine;

namespace d4160.Events 
{
    public abstract class EventBehaviourBase<TSo, TObj> : MonoBehaviourData<TSo>, IEventListener<TObj> where TSo : EventSOBase<TObj> 
    {
        [SerializeField] protected UltEvent<TObj> _onEvent;

        void OnEnable() => _data?.AddListener(this);

        void OnDisable () => _data?.RemoveListener(this);

        public void Invoke (TObj obj) => _data?.Invoke (obj);

        void IEventListener<TObj>.OnInvoked(TObj param) => _onEvent?.Invoke(param);
    }

    public abstract class EmptyEventBehaviourBase<TSo, TObj> : MonoBehaviourData<TSo>, IEventListener<TObj> where TSo : EventSOBase<TObj>
    {
        void OnEnable() => _data?.AddListener(this);

        void OnDisable() => _data?.RemoveListener(this);

        public void Invoke(TObj obj) => _data?.Invoke(obj);

        void IEventListener<TObj>.OnInvoked(TObj param) => OnInvokedInternal(param);

        protected abstract void OnInvokedInternal(TObj param);
    }

    public abstract class EmptyVarCheckEventBehaviourBase<TSo, TObj> : MonoBehaviourData<TSo> where TSo : VariableSOBase<TObj>
    {
        public abstract void Invoke();
    }

    public abstract class EventBehaviourBase<TSo, TObj1, TObj2> : MonoBehaviourData<TSo> where TSo : EventSOBase<TObj1, TObj2> 
    {
        [SerializeField] protected UltEvent<TObj1, TObj2> _onEvent;

        void OnEnable () {
            if (_data) {
                _data.OnEvent += _onEvent.Invoke;
            }
        }

        void OnDisable () {
            if (_data) {
                _data.OnEvent -= _onEvent.Invoke;
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Invoke (TObj1 obj1, TObj2 obj2) {
            if (_data) {
                _data.Invoke (obj1, obj2);
            }
        }
    }

    public abstract class EventBehaviourBase<TSo, TObj1, TObj2, TObj3> : MonoBehaviourData<TSo> where TSo : EventSOBase<TObj1, TObj2, TObj3> 
    {
        [SerializeField] protected UltEvent<TObj1, TObj2, TObj3> _onEvent;

        void OnEnable () {
            if (_data) {
                _data.OnEvent += _onEvent.Invoke;
            }
        }

        void OnDisable () {
            if (_data) {
                _data.OnEvent -= _onEvent.Invoke;
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Invoke (TObj1 obj1, TObj2 obj2, TObj3 obj3) {
            if (_data) {
                _data.Invoke (obj1, obj2, obj3);
            }
        }
    }

    public abstract class EventBehaviourBase<TSo, TObj1, TObj2, TObj3, TObj4> : MonoBehaviourData<TSo> where TSo : EventSOBase<TObj1, TObj2, TObj3, TObj4> 
    {
        [SerializeField] protected UltEvent<TObj1, TObj2, TObj3, TObj4> _onEvent;

        void OnEnable () {
            if (_data) {
                _data.OnEvent += _onEvent.Invoke;
            }
        }

        void OnDisable () {
            if (_data) {
                _data.OnEvent -= _onEvent.Invoke;
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Invoke (TObj1 obj1, TObj2 obj2, TObj3 obj3, TObj4 obj4) {
            if (_data) {
                _data.Invoke (obj1, obj2, obj3, obj4);
            }
        }
    }
}