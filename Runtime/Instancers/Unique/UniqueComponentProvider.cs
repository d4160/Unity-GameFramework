using System;
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

        public T Instantiate()
        {
            OnInstanced?.Invoke(Prefab);
            return Prefab;
        }
    }
}