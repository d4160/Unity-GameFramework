using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Instancers {
    [CreateAssetMenu (menuName = "d4160/Instancers/GameObject Factory")]
    public class GameObjectFactorySO : ScriptableObject {

        [SerializeField] protected GameObject _prefab;

        public event Action<GameObject> OnInstanced;
        public event Action<GameObject> OnDestroy;

        private readonly GameObjectFactory _factory = new GameObjectFactory ();

        public GameObjectFactory Factory => _factory;

        private void CallOnInstancedEvent (GameObject go) => OnInstanced?.Invoke (go);
        private void CallOnDestroyEvent (GameObject go) => OnDestroy?.Invoke (go);

        [Button]
        public void Setup () {
            _factory.Prefab = _prefab;
        }

        public void RegisterEvents () {
            _factory.OnInstanced += CallOnInstancedEvent;
            _factory.OnDestroy += CallOnDestroyEvent;
        }

        public void UnregisterEvents () {
            _factory.OnInstanced -= CallOnInstancedEvent;
            _factory.OnDestroy -= CallOnDestroyEvent;
        }

        [Button]
        public GameObject Instantiate () {
            return _factory.Instantiate ();
        }

        public void Instantiate (GameObject[] instancesToFill, Action<int, GameObject> onEachInstanced = null) {
            _factory.Instantiate (instancesToFill, onEachInstanced);
        }

        public void Destroy (GameObject instance) {
            _factory.Destroy (instance);
        }
    }
}