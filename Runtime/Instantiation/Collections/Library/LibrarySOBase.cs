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
            switch (instance) {
                case Component c:
                    var newC = c as T2;
                    return newC != null ? newC : c.GetComponent<T2>();
                case GameObject go:
                    return go.GetComponent<T2>();
                default:
                    return instance as T2;
            }
        }
    }
}