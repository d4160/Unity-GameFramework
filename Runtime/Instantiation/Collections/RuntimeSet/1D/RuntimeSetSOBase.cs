using System.Collections.Generic;
using UnityEngine;
using d4160.Instancers;

namespace d4160.Collections
{
    public abstract class RuntimeSetSOBase<T> : ScriptableObject
    {
        private readonly RuntimeSet<T> _runtimeSet = new RuntimeSet<T>();

        public List<T> Items => _runtimeSet.Items;

        public T this[int i] => _runtimeSet[i];
        public int Count => _runtimeSet.Count;
        public T Random => _runtimeSet.Random;

        public T2 GetAs<T2>(int i) where T2 : class => _runtimeSet.GetAs<T2>(i);
        public T2 RandomAs<T2>() where T2: class =>  _runtimeSet.RandomAs<T2>();
        public bool Contains(T instance) => _runtimeSet.Contains(instance);
        public void Add(T instance) => _runtimeSet.Add(instance);
        public void Insert(int index, T instance)  => _runtimeSet.Insert(index, instance);
        public void Remove(T instance) => _runtimeSet.Remove(instance);
        public void RemoveAt(int index) => _runtimeSet.RemoveAt(index);
        public void Clear() => _runtimeSet.Clear();
    }
}