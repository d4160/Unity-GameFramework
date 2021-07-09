using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Events
{
    public abstract class EventSOBase<T> : ScriptableObject
    {
        public event Action<T> OnEvent;

        [Button]
        public void Invoke (T obj) {
            OnEvent?.Invoke (obj);
        }
    }

    public abstract class EventSOBase<T1, T2> : ScriptableObject
    {
        public event Action<T1, T2> OnEvent;

        [Button]
        public void Invoke (T1 obj1, T2 obj2) {
            OnEvent?.Invoke (obj1, obj2);
        }
    }

    public abstract class EventSOBase<T1, T2, T3> : ScriptableObject
    {
        public event Action<T1, T2, T3> OnEvent;

        [Button]
        public void Invoke (T1 obj1, T2 obj2, T3 obj3) {
            OnEvent?.Invoke (obj1, obj2, obj3);
        }
    }

    public abstract class EventSOBase<T1, T2, T3, T4> : ScriptableObject
    {
        public event Action<T1, T2, T3, T4> OnEvent;

        [Button]
        public void Invoke (T1 obj1, T2 obj2, T3 obj3, T4 obj4) {
            OnEvent?.Invoke (obj1, obj2, obj3, obj4);
        }
    }
}