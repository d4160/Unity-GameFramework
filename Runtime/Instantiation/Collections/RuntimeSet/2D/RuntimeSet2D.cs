using d4160.Grid;
using d4160.Instancers;
using d4160.Logging;
using UnityEngine;

namespace d4160.Collections 
{
    public class RuntimeSet2D<T>
    {
        protected T[,] _items;

        public Int2EventSO OnItemChangedEventSO { get; set; }
        public ObjectProviderSOBase<T> InstancerSO { get; set; }
        public int ColumnCount { get; set; }
        public int RowCount { get; set; }
        public Transform Parent { get; set; }
        public LoggerSO Logger { get; set; }

        public T[,] Items => _items;

        public T this[int x, int y]
        {
            get => IsXYValid(x, y) ? _items[x, y] : default;
            set {
                if (IsXYValid(x, y)) { 
                    _items[x, y] = value;
                    if (value is IRuntimeObject2D ro2d) { ro2d.X = x; ro2d.Y = y; }
                    if (OnItemChangedEventSO) OnItemChangedEventSO.Invoke(x, y); 
                }
            }
        }

        public int Count => _items.Length;
        public T Random => _items[UnityEngine.Random.Range(0, _items.GetLength(0)), UnityEngine.Random.Range(0, _items.GetLength(1))];

        public bool Contains(T item)
        {
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

        public bool IsXYValid(int x, int y) 
        {
            return x >= 0 && y >= 0 && x < ColumnCount && y < RowCount;
        }

        public RuntimeSet2D()
        {
        }

        public RuntimeSet2D(int columnCount, int rowCount)
        {
            if(Logger) Logger.LogInfo($"RuntimeSet2D Ctor();");

            ColumnCount = columnCount;
            RowCount = rowCount;

            Setup();
        }

        public void Setup() 
        {
            _items = new T[ColumnCount, RowCount];
        }

        public void Swap(int fromX, int fromY, int toX, int toY) 
        {
            T from = this[fromX, fromY];
            T to = this[toX, toY];
            this[fromX, fromY] = to;
            this[toX, toY] = from;
        }

        public bool GetNextXY(int fromX, int fromY, int addX, int addY, out int x, out int y)
        {
            fromX += addX;
            fromY += addY;

            x = fromX;
            y = fromY;

            return IsXYValid(fromX, fromY);
        }

        public bool GetNextXY(T obj, int addX, int addY, out int x, out int y)
        {
            GetXY(obj, out x, out y);
            return GetNextXY(x, y, addX, addY, out x, out y);
        }

        public void GetXY(T obj, out int x, out int y)
        {
            bool found = false;
            x = -1;
            y = -1;

            if (obj == null)
            {
                return;
            }

            for (var i = 0; i < _items.GetLength(0); i++)
            {
                for (var j = 0; j < _items.GetLength(1); j++)
                {
                    if (obj.Equals(_items[i, j]))
                    {
                        x = i;
                        y = j;
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
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