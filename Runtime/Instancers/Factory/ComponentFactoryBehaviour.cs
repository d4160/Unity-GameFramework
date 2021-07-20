using System.Collections;
using System.Collections.Generic;
using d4160.MonoBehaviourData;
using NaughtyAttributes;
using UltEvents;
using UnityEngine;

namespace d4160.Instancers
{
    public class ComponentFactoryBehaviour : MonoBehaviourUnityData<ComponentFactorySO>
    {
        [SerializeField] private Transform _instancesParent;

        [Header("EVENTS")]
        [SerializeField] private UltEvent<Component> _onInstanced;
        [SerializeField] private UltEvent<Component> _onDestroy;

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

        void Start() {
            Setup();
        }

        public void Setup(){
            if (_data) {
                _data.Parent = _instancesParent;
                _data.Setup();
            }
        }

        [Button]
        public Component Instantiate() {
            if (_data) return _data.Instantiate(); return null;
        }

        public T InstantiateAs<T>() where T : Component
        {
            if (_data) return _data.Instantiate() as T; return null;
        }

        public void Destroy(Component instance) {
            if (_data) _data.Destroy(instance);
        }

        public void Destroy<T>(T instance) where T : Component
        {
            if (_data) _data.Destroy(instance);
        }
    }
}