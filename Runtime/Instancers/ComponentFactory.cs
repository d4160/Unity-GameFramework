using UnityEngine;


namespace d4160.Instancers
{
    public class ComponentFactory<T> : IObjectFactory<T> where T : Component
    {
        private T _prefab;

        public T Prefab { get => _prefab; set => _prefab = value; }

        public ComponentFactory(T prefab)
        {
            _prefab = prefab;
        }

        public virtual T Instantiate()
        {
            if (_prefab) {
                return GameObject.Instantiate(_prefab);
            }

            return null;
        }

        public virtual void Destroy(T instance)
        {
            if(instance) {
                if (Application.isPlaying)
                {
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
