using System;
using UnityEngine;


namespace d4160.Instancers
{
    public class ComponentFactory<T> : IObjectFactory<T> where T : Component
    {
        public event Action<T> OnInstanced;
        public event Action<T> OnDestroy;

        protected T _prefab;

        public T Prefab { get => _prefab; set => _prefab = value; }

        public ComponentFactory()
        {
        }

        public ComponentFactory(T prefab)
        {
            _prefab = prefab;
        }

        public virtual T Instantiate()
        {
            if (_prefab) {
                T instance = GameObject.Instantiate(_prefab);
                OnInstanced?.Invoke(instance);
                return instance;
            }

            return null;
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
    }
}
