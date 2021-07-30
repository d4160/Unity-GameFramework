
using System;
using d4160.Collections;
using UnityEngine;

namespace d4160.Instancers {
    public interface IProvider<T> : IInProvider<T>, IOutProvider<T> where T : class
    {
        Transform Parent { get; set; }
        bool WorldPositionStays { get; set; }
        bool UsePositionAndRotation { get; set; }
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        bool useLibrary { get; set; }
        LibrarySOBase<T> Library { get; set; }
    }

    public interface IInProvider<in T> where T : class
    {
        T Prefab { set; }

        void Destroy(T instance);
    }

    public interface IOutProvider<out T> where T : class
    {
        event Action<T> OnInstanced;
        event Action<T> OnDestroy;

        T Prefab { get; }

        T Instantiate();

        T Instantiate(Vector3 position, Quaternion rotation, Transform parent);

        T Instantiate(Transform parent, bool worldPositionStays);
    }
}