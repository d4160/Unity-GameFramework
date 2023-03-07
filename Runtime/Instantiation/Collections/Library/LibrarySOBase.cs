using UnityEngine;

namespace d4160.Collections
{
    public abstract class LibrarySOBase<T> : ScriptableObject
    {
        [SerializeField] protected T[] _items;

        public T[] Items => _items;

        public T this[int i] => _items.IsValidIndex(i) ? _items[i] : default;
        public int Count => _items.Length;
        public T Random => _items.Random();

        public T2 GetAs<T2>(int i) where T2 : class => GetAs<T2>(this[i]);
        public T2 RandomAs<T2>() where T2: class => GetAs<T2>(Random);

        private T2 GetAs<T2>(T instance) where T2 : class
        {
            return instance switch
            {
                Component c => c is T2 newC ? newC : c.GetComponent<T2>(),
                GameObject go => go.GetComponent<T2>(),
                _ => instance as T2,
            };
        }
    }
}