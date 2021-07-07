
using System;

namespace d4160.Instancers {
    public interface IProvider<T> : IInProvider<T>, IOutProvider<T>
    {
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
    }
}