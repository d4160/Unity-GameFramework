using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Instancers
{
    [CreateAssetMenu(menuName = "d4160/Instancers/GameObject Factory")]
    public class GameObjectFactorySO : ScriptableObject
    {
        [SerializeField] private GameObject _prefab;

        public event Action<GameObject> OnInstanced;
        public event Action<GameObject> OnDestroy;
        
        private readonly GameObjectFactory _factory = new GameObjectFactory();

        private void CallOnInstancedEvent(GameObject go) => OnInstanced?.Invoke(go);
        private void CallOnDestroyEvent(GameObject go) => OnDestroy?.Invoke(go);

        public void Setup() {
            _factory.Prefab = _prefab;
        }

        public void RegisterEvents() {
            _factory.OnInstanced += CallOnInstancedEvent;
            _factory.OnDestroy += CallOnDestroyEvent;
        }

        public void UnregisterEvents() {
            _factory.OnInstanced -= CallOnInstancedEvent;
            _factory.OnDestroy -= CallOnDestroyEvent;
        }

        [Button]
        public GameObject Instantiate()
        {
            return _factory.Instantiate();
        }

        public T Instantiate<T>() where T : Component
        {
            return _factory.Instantiate().GetComponent<T>();
        }

        public void Destroy(GameObject instance)
        {
            _factory.Destroy(instance);
        }

        public void Destroy<T>(T instance) where T : Component
        {
            _factory.Destroy(instance.gameObject);
        }
    }
}