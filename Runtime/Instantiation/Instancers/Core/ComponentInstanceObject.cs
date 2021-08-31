using UnityEngine;

namespace d4160.Instancers
{
    public class ComponentInstanceObject<T> : MonoBehaviour, IInstanceObject<T> where T : Component
    {
        public IProvider<T> Provider { get; set; }

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
                Provider?.Destroy(_instance);
        }
    }
}
