using System;
using d4160.Collections;
using UnityEngine;


namespace d4160.Instancers
{
    public class ComponentFactory<T> : IObjectFactory<T> where T : Component
    {
        public event Action<T> OnInstanced;
        public event Action<T> OnDestroy;

        protected T _prefab;
        protected Transform _parent;
        protected bool _worldPositionStays = true;
        protected bool _setPositionAndRotation;
        protected Vector3 _position = Vector2.zero;
        protected Quaternion _rotation =  Quaternion.identity;
        protected bool _useLibrary;
        protected LibrarySOBase<T> _library;

        public T Prefab { get => _prefab; set => _prefab = value; }
        public Transform Parent { get => _parent; set => _parent = value; }
        public bool WorldPositionStays { get => _worldPositionStays; set => _worldPositionStays = value; }
        public bool UsePositionAndRotation { get => _setPositionAndRotation; set => _setPositionAndRotation = value; }
        public Vector3 Position { get => _position; set => _position = value; }
        public Quaternion Rotation { get => _rotation; set => _rotation = value; }
        public bool useLibrary { get => _useLibrary; set => _useLibrary = value; }
        public LibrarySOBase<T> Library { get => _library; set => _library = value; }

        public ComponentFactory()
        {
        }

        public ComponentFactory(T prefab)
        {
            _prefab = prefab;
        }

        public void Destroy<T2>(T2 instance) where T2 : T {
            Destroy(instance as T);
        } 

        public virtual void Destroy(T instance)
        {
            if(instance) {
                if (Application.isPlaying)
                {
                    OnDestroy?.Invoke(instance);
                    GameObject.Destroy(instance.gameObject);
                }
                else
                {
                    GameObject.DestroyImmediate(instance.gameObject);
                }
            }
        }

        public T2 InstantiateAs<T2>() where T2 : T {
            return Instantiate() as T2;
        }

        public T2 InstantiateAs<T2>(Vector3 position, Quaternion rotation, Transform parent = null) where T2 : T {
            return Instantiate(position, rotation, parent) as T2;
        }

        public T2 InstantiateAs<T2>(Transform parent, bool worldPositionStays = true) where T2 : T {
            return Instantiate(parent, worldPositionStays) as T2;
        }

        public T Instantiate(Vector3 position, Quaternion rotation, Transform parent = null) {
            return Instantiate(position, rotation, true, parent, true);
        }

        public T Instantiate(Transform parent, bool worldPositionStays = true) {
            return Instantiate(default, default, false, parent, worldPositionStays);
        }

        protected virtual T Instantiate(Vector3 position, Quaternion rotation, bool setPositionAndRotation, Transform parent, bool worldPositionStays) {
            T prefab = _useLibrary ? _library.Random : _prefab;

            if (prefab)
            {
                T instance = null;

                if (parent)
                {
                    instance = setPositionAndRotation ? GameObject.Instantiate(prefab, position, rotation, parent) : GameObject.Instantiate(prefab, parent, worldPositionStays);
                }
                else {
                    instance = setPositionAndRotation ? GameObject.Instantiate(prefab, position, rotation) : GameObject.Instantiate(prefab);
                }
                OnInstanced?.Invoke(instance);
                return instance;
            }

            return null;
        }

        public T Instantiate()
        {
            if (UsePositionAndRotation)
                return Instantiate(_position, _rotation, _parent);
            else
                return Instantiate(_parent, _worldPositionStays);
        }

        
        protected void RaiseOnInstancedEvent(T newGo) {
            OnInstanced?.Invoke(newGo);
        }

        protected void RaiseOnDestroyEvent(T go) {
            OnDestroy?.Invoke(go);
        }
    }
}
