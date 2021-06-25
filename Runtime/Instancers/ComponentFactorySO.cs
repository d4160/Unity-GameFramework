using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Instancers
{
    [CreateAssetMenu(menuName = "d4160/Instancers/ComponentFactory")]
    public class ComponentFactorySO : ScriptableObject
    {
        [SerializeField] private Component _prefab;

        public event Action<Component> OnInstanced;
        public event Action<Component> OnDestroy;
        
        private readonly ComponentFactory<Component> _factory = new ComponentFactory<Component>();

        private void CallOnInstancedEvent(Component comp) => OnInstanced?.Invoke(comp);
        private void CallOnDestroyEvent(Component comp) => OnDestroy?.Invoke(comp);

        public void Setup() {
            _factory.Prefab = _prefab;
        }

        public void RegisterEvents() {
            _factory.OnInstanced += CallOnInstancedEvent;
            _factory.OnDestroy += CallOnDestroyEvent;
        }

        public void UnregisterEvents() {
            _factory.OnInstanced -= CallOnInstancedEvent;
            _factory.OnDestroy -= CallOnDestroyEvent;
        }

        [Button]
        public Component Instantiate()
        {
            return _factory.Instantiate();
        }

        public T InstantiateAs<T>() where T : Component
        {
            return _factory.Instantiate() as T;
        }

        public void Destroy(Component instance)
        {
            _factory.Destroy(instance);
        }

        public void Destroy<T>(T instance) where T : Component
        {
            _factory.Destroy(instance);
        }
    }
}