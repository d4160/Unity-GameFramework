using System.Collections;
using System.Collections.Generic;
using d4160.MonoBehaviourData;
using NaughtyAttributes;
using UltEvents;
using UnityEngine;

namespace d4160.Events {
    public class VoidEventBehaviour : MonoBehaviourData<VoidEventSO> {
        [SerializeField] private UltEvent _onEvent;

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
        public void Invoke () {
            if (_data) {
                _data.Invoke ();
            }
        }
    }
}