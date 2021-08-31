using UnityEngine;
using d4160.Instancers;

namespace d4160.Collections
{
    public abstract class RuntimeSet2DSOBase<T> : ScriptableObject
    {
        [SerializeField] protected int _columnCount;
        [SerializeField] protected int _rowCount;
        [SerializeField] protected ObjectProviderSOBase<T> _instancerSO;
        [SerializeField] protected Int2EventSO _onItemChangedEventSO;

        private readonly RuntimeSet2D<T> _runtimeSet = new RuntimeSet2D<T>();

        public T[,] Items => _runtimeSet.Items;

        public T this[int x, int y] {
            get => _runtimeSet[x, y];
            set => _runtimeSet[x, y] = value;
        }
        public int Count => _runtimeSet.Count;
        public T Random => _runtimeSet.Random;

        public void Setup() {
            _runtimeSet.InstancerSO = _instancerSO;
            _runtimeSet.ColumnCount = _columnCount;
            _runtimeSet.RowCount = _rowCount;
            _runtimeSet.OnItemChangedEventSO = _onItemChangedEventSO;

            _runtimeSet.Setup();
        }

        // TODO
        // public virtual T2 GetAs<T2>(int x, int y) where T2 : class => this[x, y] as T2;
        // public virtual T2 RandomAs<T2>() where T2: class => Random as T2;
        public bool Contains(T instance) => _runtimeSet.Contains(instance);
        public void Fill() => _runtimeSet.Fill();
        public void Clear() => _runtimeSet.Clear();
    }
}