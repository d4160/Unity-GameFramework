using System.Collections;
using System.Collections.Generic;
using d4160.MonoBehaviourData;
using NaughtyAttributes;
using UltEvents;
using UnityEngine;

namespace d4160.Events {
    public abstract class EventBehaviourBase<TSo, TObj> : MonoBehaviourData<TSo>, IEventListener<TObj> where TSo : EventSOBase<TObj> 
    {
        [SerializeField] private UltEvent<TObj> _onEvent;

        void OnEnable() => _data?.AddListener(this);

        void OnDisable () => _data?.RemoveListener(this);

        public void Invoke (TObj obj) => _data?.Invoke (obj);

        void IEventListener<TObj>.OnInvoked(TObj param) => _onEvent?.Invoke(param);
    }

    public abstract class EventBehaviourBase<TSo, TObj1, TObj2> : MonoBehaviourData<TSo> where TSo : EventSOBase<TObj1, TObj2> 
    {
        [SerializeField] private UltEvent<TObj1, TObj2> _onEvent;

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

        [Button]
        public void Invoke (TObj1 obj1, TObj2 obj2) {
            if (_data) {
                _data.Invoke (obj1, obj2);
            }
        }
    }

    public abstract class EventBehaviourBase<TSo, TObj1, TObj2, TObj3> : MonoBehaviourData<TSo> where TSo : EventSOBase<TObj1, TObj2, TObj3> 
    {
        [SerializeField] private UltEvent<TObj1, TObj2, TObj3> _onEvent;

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

        [Button]
        public void Invoke (TObj1 obj1, TObj2 obj2, TObj3 obj3) {
            if (_data) {
                _data.Invoke (obj1, obj2, obj3);
            }
        }
    }

    public abstract class EventBehaviourBase<TSo, TObj1, TObj2, TObj3, TObj4> : MonoBehaviourData<TSo> where TSo : EventSOBase<TObj1, TObj2, TObj3, TObj4> 
    {
        [SerializeField] private UltEvent<TObj1, TObj2, TObj3, TObj4> _onEvent;

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

        [Button]
        public void Invoke (TObj1 obj1, TObj2 obj2, TObj3 obj3, TObj4 obj4) {
            if (_data) {
                _data.Invoke (obj1, obj2, obj3, obj4);
            }
        }
    }
}