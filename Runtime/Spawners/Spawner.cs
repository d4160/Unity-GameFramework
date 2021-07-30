using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using d4160.Instancers;

namespace d4160.Spawners
{
    public class Spawner<T> where T : class
    {
        public ObjectProviderSOBase<T> ObjectProviderSO { get; set; }

        protected T Spawn() => ObjectProviderSO?.Instantiate();
        protected T Spawn(Transform parent, bool worldPositionStays = true) => ObjectProviderSO?.Instantiate(parent, worldPositionStays);
        protected T Spawn(Vector3 position, Quaternion rotation, Transform parent = null) => ObjectProviderSO?.Instantiate(position, rotation, parent);

        public void StartSpawn() {}

        public void StopSpawn() {}

        public void PauseSpawn() {}

        public void ResumeSpawn() {}
    }
}