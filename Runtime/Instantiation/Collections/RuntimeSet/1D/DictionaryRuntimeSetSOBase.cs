using System.Collections.Generic;
using UnityEngine;

namespace d4160.Collections
{
    public abstract class DictionaryRuntimeSetSOBase<T, W> : ScriptableObject
    {
        private readonly DictionaryRuntimeSet<T, W> _runtimeSet = new();

        public Dictionary<T, W> Items => _runtimeSet.Items;

        public W this[T key] => _runtimeSet[key];
        public int Count => _runtimeSet.Count;
        public W Random => _runtimeSet.Random;

        public W2 GetAs<W2>(T key) where W2 : class => _runtimeSet.GetAs<W2>(key);
        public W2 RandomAs<W2>() where W2 : class => _runtimeSet.RandomAs<W2>();
        public bool ContainsKey(T key) => _runtimeSet.ContainsKey(key);
        public bool ContainsValue(W value) => _runtimeSet.ContainsValue(value);
        public void Add(T key, W value) => _runtimeSet.Add(key, value);
        public void Remove(T key) => _runtimeSet.Remove(key);
        public void RemoveWithoutDestroy(T key) => _runtimeSet.RemoveWithoutDestroy(key);
        public void Clear() => _runtimeSet.Clear();
    }
}