using System;
using UnityEngine;


namespace d4160.Instancers
{
    public class GameObjectFactory : IObjectFactory<GameObject>
    {
        public event Action<GameObject> OnInstanced;
        public event Action<GameObject> OnDestroy;
        
        protected GameObject _prefab;

        public GameObject Prefab { get => _prefab; set => _prefab = value; }

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

        public virtual GameObject Instantiate()
        {
            if (_prefab)
            {
                var go = GameObject.Instantiate(_prefab);
                OnInstanced?.Invoke(go);
                return go;
            }

            return null;
        }
    }
}
