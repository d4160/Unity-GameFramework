using UnityEngine;

namespace d4160.Collections
{
    public abstract class LibrarySOBase<T> : ScriptableObject where T : class
    {
        [SerializeField] protected T[] _items;

        public T[] Items => _items;

        public T this[int i] => _items.IsValidIndex(i) ? _items[i] : null;
        public int Count => _items.Length;
        public T Random => _items.Random();

        public virtual T2 GetAs<T2>(int i) where T2 : class => this[i] as T2;
        public virtual T2 RandomAs<T2>() where T2: class => Random as T2;
    }
}