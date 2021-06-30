
using System;

namespace d4160.Instancers {
    public interface IProvider<T>
    {
        T Prefab { get; set; }

        event Action<T> OnInstanced;
        event Action<T> OnDestroy;
        
        T Instantiate();

        void Destroy(T instance);
    }
}