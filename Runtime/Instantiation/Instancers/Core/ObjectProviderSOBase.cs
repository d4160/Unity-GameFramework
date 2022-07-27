using System;
using d4160.Collections;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;

namespace d4160.Instancers
{
    public abstract class ObjectProviderSOBase<T> : ScriptableObject
    {
        [SerializeField] protected bool _useLibrary;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [ShowIf("_useLibrary"), Expandable]
#endif
        [SerializeField] protected LibrarySOBase<T> _library;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [HideIf("_useLibrary")]
#endif
        [SerializeField] protected T _prefab;
        // [Tooltip("If is not null, set as parent in each instantiation.")]
        // [SerializeField] protected Transform _parent;
        [SerializeField] protected bool _usePositionAndRotation;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [ShowIf("_usePositionAndRotation")]
#endif
        [SerializeField] protected Vector3 _position;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [ShowIf("_usePositionAndRotation")]
#endif
        [SerializeField] protected Quaternion _rotation;

        [Header ("PARENT OPTIONS")]
        [Tooltip("If true, the parent-relative position, scale and rotation are modified such that the object keeps the same world space position, rotation and scale as before.")]
        [SerializeField] protected bool _worldPositionStays = true;

        public event Action<T> OnInstanced;
        public event Action<T> OnDestroy;

        public abstract IProvider<T> Provider { get; }
        public Transform Parent { get => Provider.Parent; set => Provider.Parent = value; }
        public virtual T Prefab { get => _prefab; set => _prefab = value; } // TODO remove virtual
        public bool UsePositionAndRotation { get => _usePositionAndRotation; set => _usePositionAndRotation = value; }
        public Vector3 Position { get => _position; set => _position = value; }
        public Quaternion Rotation { get => _rotation; set => _rotation = value; }
        public bool WorldPositionStays { get => _worldPositionStays; set => _worldPositionStays = value; }
        public bool HasPrefab => (Provider as IOutProvider<T>).Prefab != null;

        protected void RaiseOnInstancedEvent(T comp) => OnInstanced?.Invoke(comp);
        protected void RaiseOnDestroyEvent(T comp) => OnDestroy?.Invoke(comp);

        public void SetPrefab(T value) => (Provider as IInProvider<T>).Prefab = value;
        public void SetPrefab() => (Provider as IInProvider<T>).Prefab = _prefab;
        public void SetUsePositionAndRotation(bool value) => Provider.UsePositionAndRotation = value;
        public void SetPosition(Vector3 value) => Provider.Position = value;
        public void SetRotation(Quaternion value) => Provider.Rotation = value;
        public void SetWorldPositionStays(bool value) => Provider.WorldPositionStays = value;

        public virtual void Setup() {
            SetPrefab();
            Provider.UsePositionAndRotation = _usePositionAndRotation;
            Provider.Position = _position;
            Provider.Rotation = _rotation;
            Provider.WorldPositionStays = _worldPositionStays;
            Provider.useLibrary = _useLibrary;
            Provider.Library = _library;
        }

        public virtual void RegisterEvents() {
            Provider.OnInstanced += RaiseOnInstancedEvent;
            Provider.OnDestroy += RaiseOnDestroyEvent;
        }

        public virtual  void UnregisterEvents() {
            Provider.OnInstanced -= RaiseOnInstancedEvent;
            Provider.OnDestroy -= RaiseOnDestroyEvent;
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public T Instantiate() { if (!HasPrefab) Setup(); return Provider.Instantiate(); }
        public T Instantiate(Transform parent, bool worldPositionStays = true) { if (!HasPrefab) Setup(); return Provider.Instantiate(parent, worldPositionStays); }
        public T Instantiate(Vector3 position, Quaternion rotation, Transform parent = null) { if (!HasPrefab) Setup(); return Provider.Instantiate(position, rotation, parent); }
        public T Instantiate(Vector3 position, Transform parent = null) { if (!HasPrefab) Setup(); return Provider.Instantiate(position, Quaternion.identity, parent); }
        public T InstantiateTo(Transform posRot, Transform parent) { if (!HasPrefab) Setup(); return Provider.Instantiate(posRot.position, posRot.rotation, parent); }
        public T InstantiateTo(Transform posRot) { if (!HasPrefab) Setup(); return Provider.Instantiate(posRot.position, posRot.rotation, null); }

        public virtual T2 InstantiateAs<T2>() where T2 : class => Instantiate() as T2;
        public virtual T2 InstantiateAs<T2>(Vector3 position, Quaternion rotation, Transform parent = null) where T2 : class => Instantiate(position, rotation, parent) as T2;
        public virtual T2 InstantiateAs<T2>(Transform parent, bool worldPositionStays = true) where T2 : class => Instantiate(parent, worldPositionStays) as T2;

        public void Destroy(T instance) => Provider.Destroy(instance);
        public virtual void Destroy<T2>(T2 instance) where T2 : class {}
    }
}