using System.Collections.Generic;
using UnityEngine;
using d4160.Instancers;

namespace d4160.Collections
{
    public class RuntimeSet<T>
    {
        private List<T> _items = new List<T>();

        public List<T> Items => _items;

        public T this[int i] {
            get => _items.IsValidIndex(i) ? _items[i] : default;
            set {
                if (_items.IsValidIndex(i)) {
                    _items[i] = value;
                }
            }
        }

        public int Count => _items.Count;
        public T Random => _items.Random();

        public T2 GetAs<T2>(int i) where T2 : class => GetAs<T2>(this[i]);
        public T2 RandomAs<T2>() where T2: class => GetAs<T2>(Random);
        public bool Contains(T instance) => _items.Contains(instance);

        public void Add(T instance) 
        {
            if (!_items.Contains(instance))
            {
                _items.Add(instance);
            }
        }

        public void Insert(int index, T instance) 
        {
            if (!_items.Contains(instance)) 
            {
                _items.Insert(index, instance);
            }
        }

        public void Remove(T instance) 
        {
            if (_items.Contains(instance)) 
            {
                _items.Remove(instance);
                Destroy(instance);
            }
        }

        public void RemoveAt(int index) 
        {
            if (_items.IsValidIndex(index)) 
            {
                Destroy(_items[index]);
                _items.RemoveAt(index);
            }
        }

        public void Clear()
        {
            for (var i = _items.LastIndex(); i >= 0; i--)
            {
                Destroy(_items[i]);
            }

            _items.Clear();
        }

        private void Destroy(T instance) 
        {
            switch (instance) {
                case IDestroyable d:
                    d.Destroy();
                    break;
                case Component c:
                    var des = c.GetComponent<IDestroyable>();
                    if (des != null) { des.Destroy(); } else { GameObject.Destroy(c.gameObject); };
                    break;
                case GameObject go:
                    des = go.GetComponent<IDestroyable>();
                    if (des != null) { des.Destroy(); } else { GameObject.Destroy(go); };
                    break;
            }
        }

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