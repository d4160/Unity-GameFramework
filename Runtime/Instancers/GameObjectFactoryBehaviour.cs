using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Instancers
{
    public class GameObjectFactoryBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;

        private GameObjectFactory _factory = new GameObjectFactory();

        void Start() {
            _factory.Prefab = _prefab;
        }

        [Button]
        public void Instantiate() {
            _factory.Instantiate();
        }

        public void Destroy(GameObject instance) {
            _factory.Destroy(instance);
        }
    }
}