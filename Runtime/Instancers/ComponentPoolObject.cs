using UnityEngine;

namespace d4160.Instancers
{
    public class ComponentPoolObject<T> : MonoBehaviour, IPoolObject<T> where T : Component
    {
        public IObjectPool<T> Pool { get; set; }

        private T _instance;

        void Awake() {
            _instance = this.GetComponent<T>();

            if(!_instance) {
                Debug.LogWarning($"We need a component of type {typeof(T)} in this GameObject", gameObject);
            }
        }

        public void Destroy()
        {
            if(_instance)
                Pool?.Destroy(_instance);
        }
    }
}
