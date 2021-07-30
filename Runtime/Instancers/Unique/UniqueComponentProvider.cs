using System;
using d4160.Collections;
using UnityEngine;

namespace d4160.Instancers
{
    public class UniqueComponentProvider<T> : IProvider<T> where T : Component
    {
        protected T _uniqueInstance;
        public T Prefab
        {
            get
            {
                if (_uniqueInstance == null)
                {
                    _uniqueInstance = GameObject.FindObjectOfType<T>();
                }
                return _uniqueInstance;
            }
            set => _uniqueInstance = value;
        }

        protected Transform _parent;
        protected bool _worldPositionStays;
        protected bool _usePositionAndRotation;
        protected Vector3 _position = Vector2.zero;
        protected Quaternion _rotation =  Quaternion.identity;
        protected bool _useLibrary;
        protected LibrarySOBase<T> _library;

        public Transform Parent { get => _parent; set => _parent = value; }
        public bool WorldPositionStays { get => _worldPositionStays; set => _worldPositionStays = value; }
        public bool UsePositionAndRotation { get => _usePositionAndRotation; set => _usePositionAndRotation = value; }
        public Vector3 Position { get => _position; set => _position = value; }
        public Quaternion Rotation { get => _rotation; set => _rotation = value; }
        public bool useLibrary { get => _useLibrary; set => _useLibrary = value; }
        public LibrarySOBase<T> Library { get => _library; set => _library = value; }
        

        public event Action<T> OnInstanced;
        public event Action<T> OnDestroy;

        public UniqueComponentProvider()
        {
        }

        public UniqueComponentProvider(T uniqueInstance)
        {
            _uniqueInstance = uniqueInstance;
        }

        public void Destroy(T instance)
        {
            if (_uniqueInstance)
            {
                if (Application.isPlaying)
                {
                    OnDestroy?.Invoke(_uniqueInstance);
                    GameObject.Destroy(_uniqueInstance.gameObject);
                }
                else
                {
                    GameObject.DestroyImmediate(_uniqueInstance.gameObject);
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

        protected virtual T Instantiate(Vector3 position, Quaternion rotation, bool usePositionAndRotation, Transform parent, bool worldPositionStays) {
            if (Prefab)
            {
                if (_parent)
                    Prefab.transform.SetParent(_parent, worldPositionStays);
                if(usePositionAndRotation)
                    Prefab.transform.SetPositionAndRotation(_position, _rotation);

                OnInstanced?.Invoke(Prefab);
                return Prefab;
            }

            return null;
        }

        public T Instantiate() => Instantiate(_position, _rotation, _parent);
    }
}