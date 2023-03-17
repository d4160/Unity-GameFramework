using System.Collections.Generic;
using UnityEngine;
using d4160.Instancers;

namespace d4160.Collections
{
    public class DictionaryRuntimeSet<T, W>
    {
        private readonly Dictionary<T, W> _items = new();

        public Dictionary<T, W> Items => _items;

        public W this[T key]
        {
            get => _items.ContainsKey(key) ? _items[key] : default;
            set
            {
                if (_items.ContainsKey(key))
                {
                    _items[key] = value;
                }
            }
        }

        public int Count => _items.Count;
        public W Random => _items.Random();

        public W2 GetAs<W2>(T key) where W2 : class => GetAs<W2>(this[key]);
        public W2 RandomAs<W2>() where W2 : class => GetAs<W2>(Random);
        public bool ContainsKey(T key) => _items.ContainsKey(key);
        public bool ContainsValue(W value) => _items.ContainsValue(value);

        public void Add(T key, W value)
        {
            if (!_items.ContainsKey(key))
            {
                _items.Add(key, value);
            }
        }

        public void Remove(T key)
        {
            if (_items.ContainsKey(key))
            {
                Destroy(_items[key]);
                _items.Remove(key);
            }
        }

        public void RemoveWithoutDestroy(T key)
        {
            if (_items.ContainsKey(key))
            {
                _items.Remove(key);
            }
        }

        public void Clear()
        {
            foreach (var item in _items)
            {
                if (item.Value != null)
                    Destroy(item.Value);
            }

            _items.Clear();
        }

        private void Destroy(W instance)
        {
            switch (instance)
            {
                case IDestroyable d:
                    d.Destroy();
                    break;
                case Component c:
                    if (c == null) return;
                    var des = c.GetComponent<IDestroyable>();
                    if (des != null) { des.Destroy(); } else { if (Application.isPlaying) GameObject.Destroy(c.gameObject); else GameObject.DestroyImmediate(c.gameObject); };
                    break;
                case GameObject go:
                    if (go)
                    {
                        if (go.TryGetComponent(out des)) { des.Destroy(); } else { if (Application.isPlaying) GameObject.Destroy(go); else GameObject.DestroyImmediate(go); };
                    }
                    break;
            }
        }

        private W2 GetAs<W2>(W instance) where W2 : class
        {
            switch (instance)
            {
                case Component c:
                    var newC = c as W2;
                    return newC ?? c.GetComponent<W2>();
                case GameObject go:
                    return go.GetComponent<W2>();
                default:
                    return instance as W2;
            }
        }
    }
}