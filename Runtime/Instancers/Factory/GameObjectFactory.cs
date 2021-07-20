using System;
using UnityEngine;


namespace d4160.Instancers
{
    public class GameObjectFactory : IObjectFactory<GameObject>
    {
        public event Action<GameObject> OnInstanced;
        public event Action<GameObject> OnDestroy;
        
        protected GameObject _prefab;
        protected Transform _parent;
        protected bool _worldPositionStays = true;
        protected bool _setPositionAndRotation;
        protected Vector3 _position = Vector2.zero;
        protected Quaternion _rotation =  Quaternion.identity;

        public GameObject Prefab { get => _prefab; set => _prefab = value; }
        public Transform Parent { get => _parent; set => _parent = value; }
        public bool WorldPositionStays { get => _worldPositionStays; set => _worldPositionStays = value; }
        public bool UsePositionAndRotation { get => _setPositionAndRotation; set => _setPositionAndRotation = value; }
        public Vector3 Position { get => _position; set => _position = value; }
        public Quaternion Rotation { get => _rotation; set => _rotation = value; }

        public virtual void Destroy<T>(T instance) where T : Component {
            Destroy(instance.gameObject);
        }

        public virtual void Destroy(GameObject instance)
        {
            if (instance)
            {
                if (Application.isPlaying)
                {
                    OnDestroy?.Invoke(instance);
                    GameObject.Destroy(instance);
                }
                else
                {
                    GameObject.DestroyImmediate(instance);
                }
            }
        }

        public T InstantiateAs<T>() where T : Component {
            return Instantiate().GetComponent<T>();
        }

        public T InstantiateAs<T>(Vector3 position, Quaternion rotation, Transform parent = null) where T : Component {
            return Instantiate(position, rotation, parent).GetComponent<T>();
        }

        public T InstantiateAs<T>(Transform parent, bool worldPositionStays = true) where T : Component {
            return Instantiate(parent, worldPositionStays).GetComponent<T>();
        }

        public GameObject Instantiate(Vector3 position, Quaternion rotation, Transform parent = null) {
            return Instantiate(position, rotation, true, parent, true);
        }

        public GameObject Instantiate(Transform parent, bool worldPositionStays = true) {
            return Instantiate(default, default, false, parent, worldPositionStays);
        }

        protected virtual GameObject Instantiate(Vector3 position, Quaternion rotation, bool setPositionAndRotation, Transform parent, bool worldPositionStays) {
            if (_prefab)
            {
                GameObject newGo = null;
                if (parent)
                {
                    newGo = setPositionAndRotation ? GameObject.Instantiate(_prefab, position, rotation, parent) : GameObject.Instantiate(_prefab, parent, worldPositionStays);
                }
                else {
                    newGo = setPositionAndRotation ? GameObject.Instantiate(_prefab, position, rotation) : GameObject.Instantiate(_prefab);
                }
                OnInstanced?.Invoke(newGo);
                return newGo;
            }

            Debug.LogWarning($"The Prefab for this Factory: '{typeof(GameObjectFactory)}', is missing.");
            return null;
        }

        public GameObject Instantiate()
        {
            return Instantiate(_position, _rotation, _setPositionAndRotation, _parent, _worldPositionStays);
        }

        
        protected void InvokeOnInstancedEvent(GameObject newGo) {
            OnInstanced?.Invoke(newGo);
        }

        protected void InvokeOnDestroyEvent(GameObject go) {
            OnDestroy?.Invoke(go);
        }
    }
}
