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
        [SerializeField] protected bool _setPositionAndRotation;
        [ShowIf("_setPositionAndRotation")]
        [SerializeField] protected Vector3 _position;
        [ShowIf("_setPositionAndRotation")]
        [SerializeField] protected Quaternion _rotation;

        [Header ("PARENT OPTIONS")]
        [Tooltip("If true, the parent-relative position, scale and rotation are modified such that the object keeps the same world space position, rotation and scale as before.")]
        [SerializeField] protected bool _worldPositionStays = true;

        public event Action<GameObject> OnInstanced;
        public event Action<GameObject> OnDestroy;

        public abstract IProvider<GameObject> Provider { get; }
        // public abstract IProvider<GameObject> OutProvider { get; } 
        // public abstract IInProvider<GameObject> InProvider { get; } 
        public Transform Parent { get => Provider.Parent; set => Provider.Parent = value; }
        public GameObject Prefab { get => _prefab; set => _prefab = value; }
        public bool UsePositionAndRotation { get => _setPositionAndRotation; set => _setPositionAndRotation = value; }
        public Vector3 Position { get => _position; set => _position = value; }
        public Quaternion Rotation { get => _rotation; set => _rotation = value; }
        public bool WorldPositionStays { get => _worldPositionStays; set => _worldPositionStays = value; }

        protected void CallOnInstancedEvent(GameObject comp) => OnInstanced?.Invoke(comp);
        protected void CallOnDestroyEvent(GameObject comp) => OnDestroy?.Invoke(comp);

        public void SetPrefab(GameObject value) => (Provider as IInProvider<GameObject>).Prefab = value;
        public void SetPrefab() => (Provider as IInProvider<GameObject>).Prefab = _prefab;
        public void SetUsePositionAndRotation(bool value) => Provider.UsePositionAndRotation = value;
        public void SetPosition(Vector3 value) => Provider.Position = value;
        public void SetRotation(Quaternion value) => Provider.Rotation = value;
        public void SetWorldPositionStays(bool value) => Provider.WorldPositionStays = value;

        public virtual void Setup() {
            SetPrefab(_prefab);
            Provider.UsePositionAndRotation = _setPositionAndRotation;
            Provider.Position = _position;
            Provider.Rotation = _rotation;
            Provider.WorldPositionStays = _worldPositionStays;
        }

        public void RegisterEvents() {
            Provider.OnInstanced += CallOnInstancedEvent;
            Provider.OnDestroy += CallOnDestroyEvent;
        }

        public void UnregisterEvents() {
            Provider.OnInstanced -= CallOnInstancedEvent;
            Provider.OnDestroy -= CallOnDestroyEvent;
        }

        [Button]
        public GameObject Instantiate() => Provider.Instantiate();

        public GameObject Instantiate(Transform parent, bool worldPositionStays = true) => Provider.Instantiate(parent, worldPositionStays);

        public GameObject Instantiate(Vector3 position, Quaternion rotation, Transform parent = null) => Provider.Instantiate(position, rotation, parent);

        public T InstantiateAs<T>() where T : Component => Instantiate().GetComponent<T>();
        public T InstantiateAs<T>(Vector3 position, Quaternion rotation, Transform parent = null) where T : Component => Instantiate(position, rotation, parent).GetComponent<T>();
        public T InstantiateAs<T>(Transform parent, bool worldPositionStays = true) where T : Component => Instantiate(parent, worldPositionStays).GetComponent<T>();

        public void Destroy(GameObject instance) => Provider.Destroy(instance);

        public void Destroy<T>(T instance) where T : Component => Provider.Destroy(instance.gameObject);
    }
}