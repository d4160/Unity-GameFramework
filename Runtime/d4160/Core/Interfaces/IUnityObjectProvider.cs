using UnityEngine;

namespace d4160.Core
{
    public interface IUnityObjectProvider<out T> : IObjectProvider<T> where T : Object
    {
        T Spawn(Vector3 position, Quaternion rotation);

        T Spawn(Vector3 position, Quaternion rotation, Transform parent, bool worldStays);
    }

    public interface IObjectProvider<out T>
    {
        T Spawn();
    }

    public interface IObjectDisposer<in T>
    {
        void Despawn(T clone, float t = 0.0f);

        void DespawnAll();
    }
}

