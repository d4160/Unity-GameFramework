using System.Collections;
using System.Collections.Generic;
using d4160.MonoBehaviours;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UltEvents;
using UnityEngine;

namespace d4160.Instancers
{
    public class GameObjectFactoryBehaviour : MonoBehaviourUnityData<GameObjectFactorySO>
    {
        [Header("EVENTS")]
        [SerializeField] private UltEvent<GameObject> _onInstanced;
        [SerializeField] private UltEvent<GameObject> _onDestroy;

        void OnEnable() {
            if (_data) {
                _data.RegisterEvents();
                _data.OnInstanced += _onInstanced.Invoke;
                _data.OnDestroy += _onDestroy.Invoke;
            }
        }

        void OnDisable() {
            if (_data) {
                _data.UnregisterEvents();
                _data.OnInstanced -= _onInstanced.Invoke;
                _data.OnDestroy -= _onDestroy.Invoke;
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public GameObject Instantiate() {
            if (_data) return _data.Instantiate(); return null;
        }

        public void Destroy(GameObject instance) {
            if (_data) _data.Destroy(instance);
        }
    }
}