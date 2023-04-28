using InspectInLine;
using UnityEngine;

namespace d4160.Collections
{
    public abstract class DualLibrarySOBase<TLib1, T1> : ScriptableObject where TLib1 : LibrarySOBase<T1>
    {
        [SerializeField, InspectInline(canEditRemoteTarget = true)] protected TLib1 _library1;
        [SerializeField, InspectInline(canEditRemoteTarget = true)] protected TLib1 _library2;

        public TLib1 Library1 => _library1;
        public TLib1 Library2 => _library2;

        public int GetCount(int libIndex = 0) => libIndex == 0 ? _library1.Count : _library2.Count;
        public T1 GetRandom(int libIndex = 0) => libIndex == 0 ? _library1.Random : _library2.Random;

        public T2 GetAs<T2>(int i, int libIndex = 0) where T2 : class => GetAs<T2>(libIndex == 0  ? _library1[i] : _library2[i]);
        public T2 RandomAs<T2>(int libIndex = 0) where T2 : class => GetAs<T2>(GetRandom(libIndex));

        private T2 GetAs<T2>(T1 instance) where T2 : class
        {
            return instance switch
            {
                Component c => c is T2 newC ? newC : c.GetComponent<T2>(),
                GameObject go => go.GetComponent<T2>(),
                _ => instance as T2
            };
        }
    }

    public abstract class UnityDualLibrarySOBase<TLib1, T1> : ScriptableObject where TLib1 : UnityLibrarySOBase<T1>
    {
        [SerializeField, InspectInline(canEditRemoteTarget = true)] protected TLib1 _library1;
        [SerializeField, InspectInline(canEditRemoteTarget = true)] protected TLib1 _library2;

        public TLib1 Library1 => _library1;
        public TLib1 Library2 => _library2;

        public int GetCount(int libIndex = 0) => libIndex == 0 ? _library1.Count : _library2.Count;
        public T1 GetRandom(int libIndex = 0) => libIndex == 0 ? _library1.Random : _library2.Random;

        public TLib1 GetLibrary(int libIndex) => libIndex == 0 ? _library1 : _library2;

        public T2 GetAs<T2>(int i, int libIndex = 0) where T2 : class => GetAs<T2>(libIndex == 0 ? _library1[i] : _library2[i]);
        public T2 RandomAs<T2>(int libIndex = 0) where T2 : class => GetAs<T2>(GetRandom(libIndex));

        private T2 GetAs<T2>(T1 instance) where T2 : class
        {
            return instance switch
            {
                Component c => c is T2 newC ? newC : c.GetComponent<T2>(),
                GameObject go => go.GetComponent<T2>(),
                _ => instance as T2
            };
        }
    }
}