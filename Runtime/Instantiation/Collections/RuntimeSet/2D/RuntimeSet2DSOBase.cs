using UnityEngine;
using d4160.Instancers;
using d4160.Logging;

namespace d4160.Collections
{
    public abstract class RuntimeSet2DSOBase<TRs2D, T> : ScriptableObject where TRs2D : RuntimeSet2D<T>, new()
    {
        [SerializeField] protected int _columnCount;
        [SerializeField] protected int _rowCount;
        [SerializeField] protected ObjectProviderSOBase<T> _instancerSO;
        [SerializeField] protected Int2EventSO _onItemChangedEventSO;
        [SerializeField] protected LoggerSO _logger;

        protected readonly TRs2D _runtimeSet = new();

        public T[,] Items => _runtimeSet.Items;

        public T this[int x, int y] {
            get => _runtimeSet[x, y];
            set => _runtimeSet[x, y] = value;
        }
        public int Count => _runtimeSet.Count;
        public T Random => _runtimeSet.Random;

        protected virtual void SetProperties() 
        {
            _runtimeSet.InstancerSO = _instancerSO;
            _runtimeSet.ColumnCount = _columnCount;
            _runtimeSet.RowCount = _rowCount;
            _runtimeSet.OnItemChangedEventSO = _onItemChangedEventSO;
            _runtimeSet.Logger = _logger;
        }

        public void Setup() 
        {
            SetProperties();

            _runtimeSet.Setup();
        }

        public T2 GetAs<T2>(int x, int y) where T2 : class, T => this[x, y] as T2;
        public T2 RandomAs<T2>() where T2 : class, T => Random as T2;
        public bool Contains(T instance) => _runtimeSet.Contains(instance);
        public bool IsXYValid(int x, int y) => _runtimeSet.IsXYValid(x, y);
        public void Swap(int fromX, int fromY, int toX, int toY) => _runtimeSet.Swap(fromX, fromY, toX, toY);
        public void GetXY(T obj, out int x, out int y) => _runtimeSet.GetXY(obj, out x, out y);
        public void Fill() => _runtimeSet.Fill();
        public void Clear() => _runtimeSet.Clear();
    }
}