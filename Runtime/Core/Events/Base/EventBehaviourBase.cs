using d4160.MonoBehaviours;
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

        void OnEnable() { if (_data) _data.AddListener(this); }

        void OnDisable() { if (_data) _data.RemoveListener(this); }

        public void Invoke(TObj param) { if (_data) _data.Invoke(param); }

        void IEventListener<TObj>.OnInvoked(TObj param) => _onEvent?.Invoke(param);
    }

    public abstract class EmptyEventBehaviourBase<TSo, TObj> : MonoBehaviourData<TSo>, IEventListener<TObj> where TSo : EventSOBase<TObj>
    {
        void OnEnable() { if (_data) _data.AddListener(this); }

        void OnDisable() { if (_data) _data.RemoveListener(this); }

        public void Invoke(TObj param) { if (_data) _data.Invoke(param); }

        void IEventListener<TObj>.OnInvoked(TObj param) => OnInvokedInternal(param);

        protected abstract void OnInvokedInternal(TObj param);
    }

    public abstract class EmptyVarCheckEventBehaviourBase<TSo, TObj> : MonoBehaviourData<TSo> where TSo : VariableSOBase<TObj>
    {
        public abstract void Invoke();
    }

    public abstract class EventBehaviourBase<TSo, T1, T2> : MonoBehaviourData<TSo>, IEventListener<T1, T2> where TSo : EventSOBase<T1, T2> 
    {
        [SerializeField] protected UltEvent<T1, T2> _onEvent;

        void OnEnable () { if (_data) _data.AddListener(this); }

        void OnDisable() { if (_data) _data.RemoveListener(this); }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Invoke (T1 param1, T2 param2) 
        {
            if (_data) _data.Invoke (param1, param2);
        }

        void IEventListener<T1, T2>.OnInvoked(T1 param1, T2 param2) => _onEvent?.Invoke(param1, param2);
    }

    public abstract class EventBehaviourBase<TSo, T1, T2, T3> : MonoBehaviourData<TSo>, IEventListener<T1,T2,T3> where TSo : EventSOBase<T1, T2, T3> 
    {
        [SerializeField] protected UltEvent<T1, T2, T3> _onEvent;

        void OnEnable () { if (_data) _data.AddListener(this); }

        void OnDisable () { if (_data) _data.RemoveListener(this); }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Invoke (T1 param1, T2 param2, T3 param3) { if (_data) _data.Invoke(param1, param2, param3); }

        void IEventListener<T1, T2, T3>.OnInvoked(T1 param1, T2 param2, T3 param3) => _onEvent?.Invoke(param1, param2, param3);
    }

    public abstract class EventBehaviourBase<TSo, T1, T2, T3, T4> : MonoBehaviourData<TSo>, IEventListener<T1,T2,T3,T4> where TSo : EventSOBase<T1, T2, T3, T4> 
    {
        [SerializeField] protected UltEvent<T1, T2, T3, T4> _onEvent;

        void OnEnable() { if (_data) _data.AddListener(this); }

        void OnDisable() { if (_data) _data.RemoveListener(this); }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Invoke (T1 param1, T2 param2, T3 param3, T4 param4) 
        {
            if (_data) 
                _data.Invoke (param1, param2, param3, param4);
        }

        void IEventListener<T1, T2, T3, T4>.OnInvoked(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            _onEvent.Invoke(param1, param2, param3, param4);
        }
    }
}