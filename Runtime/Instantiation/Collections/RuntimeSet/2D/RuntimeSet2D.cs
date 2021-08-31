using d4160.Grid;
using d4160.Instancers;
using UnityEngine;

namespace d4160.Collections {

    public class RuntimeSet2D<T>
    {
        private T[,] _items;

        public Int2EventSO OnItemChangedEventSO { get; set; }
        public ObjectProviderSOBase<T> InstancerSO { get; set; }
        public int ColumnCount { get; set; }
        public int RowCount { get; set; }
        public Transform Parent { get; set; }

        public T[,] Items => _items;

        public T this[int x, int y]
        {
            get => IsXYValid(x, y) ? _items[x, y] : default;
            set {
                if (IsXYValid(x, y)) { 
                    _items[x, y] = value; 
                    if (value is IRuntimeObject2D ro2d) ro2d.SetXY(x, y);
                    OnItemChangedEventSO?.Invoke(x, y); 
                }
            }
        }

        public int Count => _items.Length;
        public T Random => _items[UnityEngine.Random.Range(0, _items.GetLength(0)), UnityEngine.Random.Range(0, _items.GetLength(1))];

        public bool Contains(T item) {
            for (var x = 0; x < _items.GetLength(0); x++)
            {
                for (var y = 0; y < _items.GetLength(1); y++)
                {
                    if (this[x, y].Equals(item))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsXYValid(int x, int y) {
            return x >= 0 && y >= 0 && x < ColumnCount && y < RowCount;
        }

        public RuntimeSet2D()
        {
        }

        public RuntimeSet2D(int columnCount, int rowCount)
        {
            ColumnCount = columnCount;
            RowCount = rowCount;

            Setup();
        }

        public void Setup() {
            _items = new T[ColumnCount, RowCount];
        }

        public void Swap(int fromX, int fromY, int toX, int toY) 
        {
            T from = this[fromX, fromY];
            T to = this[toX, toY];
            this[fromX, fromY] = to;
            this[toX, toY] = from;
        }

        public void Fill() {
            if (InstancerSO)
            {
                for (var x = 0; x < _items.GetLength(0); x++)
                {
                    for (var y = 0; y < _items.GetLength(1); y++)
                    {
                        this[x, y] = Parent ? InstancerSO.Instantiate(Parent) : InstancerSO.Instantiate();
                    }
                }
            }
        }

        public void Clear() {
            if (InstancerSO)
            {
                for (var x = 0; x < _items.GetLength(0); x++)
                {
                    for (var y = 0; y < _items.GetLength(1); y++)
                    {
                        if (this[x, y] != null)
                            InstancerSO.Destroy(this[x, y]);
                    }
                }
            }
        }
    }
}