
using System;
using UnityEngine;

namespace d4160.Instancers {
    public interface IProvider<T> : IInProvider<T>, IOutProvider<T>
    {
        Transform Parent { get; set; }
        bool WorldPositionStays { get; set; }
        bool UsePositionAndRotation { get; set; }
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
    }

    public interface IInProvider<in T>
    {
        T Prefab { set; }

        void Destroy(T instance);
    }

    public interface IOutProvider<out T>
    {
        event Action<T> OnInstanced;
        event Action<T> OnDestroy;

        T Prefab { get; }

        T Instantiate();

        T Instantiate(Vector3 position, Quaternion rotation, Transform parent);

        T Instantiate(Transform parent, bool worldPositionStays);
    }
}