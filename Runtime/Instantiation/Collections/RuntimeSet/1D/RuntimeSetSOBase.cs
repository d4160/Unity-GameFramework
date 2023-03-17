#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Collections
{
    public abstract class RuntimeSetSOBase<T> : ScriptableObject
    {
        private readonly RuntimeSet<T> _runtimeSet = new ();

        public List<T> Items => _runtimeSet.Items;

        public T this[int i] => _runtimeSet[i];
        public int Count => _runtimeSet.Count;
        public T Random => _runtimeSet.Random;

        public T2 GetAs<T2>(int i) where T2 : class => _runtimeSet.GetAs<T2>(i);
        public T2 RandomAs<T2>() where T2: class =>  _runtimeSet.RandomAs<T2>();
        public bool Contains(T instance) => _runtimeSet.Contains(instance);
        public void Add(T instance) => _runtimeSet.Add(instance);
        public void AddRange(IList<T> list) => _runtimeSet.AddRange(list);
        public void Insert(int index, T instance)  => _runtimeSet.Insert(index, instance);
        public void Remove(T instance) => _runtimeSet.Remove(instance);
        public void RemoveAt(int index) => _runtimeSet.RemoveAt(index);
        public void Clear() => _runtimeSet.Clear();

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        [ContextMenu("DebugItems")]
        private void DebugItems()
        {
            string s = string.Empty;
            for (int i = 0; i < Items.Count; i++)
            {
                s += $"{Items[i]}\n";
            }

            Debug.Log($"{this.GetType()} Items: {s}");
        }
    }
}