using System;
using UnityEngine;

namespace d4160.Instancers {
    public class GameObjectFactory : IObjectFactory<GameObject> {
        public event Action<GameObject> OnInstanced;
        public event Action<GameObject> OnDestroy;

        protected GameObject _prefab;

        public GameObject Prefab { get => _prefab; set => _prefab = value; }

        public virtual void Destroy (GameObject instance) {
            if (instance) {
                if (Application.isPlaying) {
                    OnDestroy?.Invoke (instance);
                    GameObject.Destroy (instance);

                } else {
                    GameObject.DestroyImmediate (instance);
                }
            }
        }

        public virtual GameObject Instantiate () {
            if (_prefab) {
                var newGo = GameObject.Instantiate (_prefab);
                OnInstanced?.Invoke (newGo);
                return newGo;
            }

            return null;
        }

        public void Instantiate (GameObject[] instancesToFill, Action<int, GameObject> onEachInstanced = null) {
            if (_prefab) {
                for (var i = 0; i < instancesToFill.Length; i++) {
                    instancesToFill[i] = Instantiate ();
                    onEachInstanced?.Invoke (i, instancesToFill[i]);
                }
            }
        }
    }
}