using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Instancers
{
    public abstract class GameObjectProviderSOBase : ScriptableObject
    {
        [SerializeField] protected GameObject _prefab;
        // [Tooltip("If is not null, set as parent in each instantiation.")]
        // [SerializeField] protected Transform _parent;
        [Tooltip("If true, the parent-relative position, scale and rotation are modified such that the object keeps the same world space position, rotation and scale as before.")]
        [SerializeField] protected bool _worldPositionStays = true;

        public event Action<GameObject> OnInstanced;
        public event Action<GameObject> OnDestroy;

        //public abstract IProvider<GameObject> Provider { get; }
        public abstract IOutProvider<GameObject> OutProvider { get; } 
        public abstract IInProvider<GameObject> InProvider { get; } 
        public Transform Parent { get; set; }
        public GameObject Prefab { get => _prefab; set => _prefab = value; }

        protected void CallOnInstancedEvent(GameObject comp) => OnInstanced?.Invoke(comp);
        protected void CallOnDestroyEvent(GameObject comp) => OnDestroy?.Invoke(comp);

        public void Setup() {
            InProvider.Prefab = _prefab;
        }

        public void RegisterEvents() {
            OutProvider.OnInstanced += CallOnInstancedEvent;
            OutProvider.OnDestroy += CallOnDestroyEvent;
        }

        public void UnregisterEvents() {
            OutProvider.OnInstanced -= CallOnInstancedEvent;
            OutProvider.OnDestroy -= CallOnDestroyEvent;
        }

        [Button]
        public GameObject Instantiate()
        {
            GameObject instance = OutProvider.Instantiate();
            if (Parent) instance.transform.SetParent(Parent, _worldPositionStays);
            return instance;
        }

        public T InstantiateAs<T>() where T : Component
        {
            return Instantiate().GetComponent<T>();
        }

        public void Destroy(GameObject instance)
        {
            InProvider.Destroy(instance);
        }

        public void Destroy<T>(T instance) where T : Component
        {
            InProvider.Destroy(instance.gameObject);
        }
    }
}