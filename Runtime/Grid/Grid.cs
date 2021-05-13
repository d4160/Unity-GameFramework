using System;
using CodeMonkey.Utils;
using d4160.UnityUtils;
using TMPro;
using UnityEngine;

namespace d4160.Grid {
    public class Grid<T>
    {

        public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
        public class OnGridValueChangedEventArgs : EventArgs
        {
            public int x;
            public int y;
        }

        protected int _width;
        protected int _height;
        protected Vector2 _cellSize;
        protected Vector3 _originPosition;
        protected T[,] _gridArray;
        protected TextMeshPro[,] _debugTextArray = null;

        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
        /// <summary>
        /// When use in play, has a problem when draw gizmos with duration 
        /// </summary>
        /// <param name="cellSize"></param>
        public Vector2 CellSize { get => _cellSize; set => _cellSize = value; }
        public Vector3 OriginPosition { get => _originPosition; set => _originPosition = value; }
        public T[,] GridArray { get => _gridArray; set => _gridArray = value; }

        public Grid (int width, int height, Vector2 cellSize, Vector3 originPosition = default, bool drawDebugText = true, Color? textColor = null, int textFontSize = 5, Transform textParent = null) {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _originPosition = originPosition;

            _gridArray = new T[width, height];

            // Debug.Log(gridArray.GetLength(0)); // x
            // Debug.Log(gridArray.GetLength(1)); // y

            if (drawDebugText)
                DrawDebugText (textColor ?? Color.white, textFontSize, textParent);
        }

        public bool IsXYValid(Vector3 worldPosition) {
            GetXY(worldPosition, out int x, out int y);
            return IsXYValid(x, y);
        }

        public bool IsXYValid(int x, int y) {
            return x >= 0 && y >= 0 && x < _width && y < _height;
        }

        public Vector3 GetWorldPosition (int x, int y) {
            return (new Vector3 (x, y) * _cellSize).SumVector3 (_originPosition);
        }

        public Vector3 GetWorldPositionInCenter (int x, int y) {
            return GetWorldPosition (x, y).SumVector2 (_cellSize / 2);
        }

        public void GetXY (Vector3 worldPosition, out int x, out int y) {
            Vector3 localPosition = worldPosition - _originPosition;
            x = Mathf.FloorToInt (localPosition.x / _cellSize.x);
            y = Mathf.FloorToInt (localPosition.y / _cellSize.y);
        }

        public void GetXY (T gridObject, out int x, out int y) {
            bool found = false;
            x = -1;
            y = -1;

            if(gridObject == null) {
                return;
            }

            for (var i = 0; i < _gridArray.GetLength(0); i++) {
                for (var j = 0; j < _gridArray.GetLength(1); j++)
                {
                    //Object.ReferenceEquals(gridObject, gridObject);
                    //var some = !System.Collections.Generic.EqualityComparer<T>.Default.Equals(gridObject, gridObject);
                    if (gridObject.Equals(_gridArray[i, j])) {
                        x = i;
                        y = j;
                        found = true;
                        break;
                    }
                }
                if(found) break;
            }
        }

        public void SetGridObject (int x, int y, T value) {
            if (IsXYValid(x, y)) {
                _gridArray[x, y] = ProcessValue(x, y, value);

                if(_gridArray[x, y] is IGridObject gridObj) {
                    gridObj.X = x;
                    gridObj.Y = y;
                }
                OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, y = y });
            }
        }

        /// <summary>
        /// Called before set the value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual T ProcessValue(int x, int y, T value) {
            return value;
        }

        public void SetGridObject (Vector3 worldposition, T value) {
            int x, y;
            GetXY (worldposition, out x, out y);
            SetGridObject (x, y, value);
        }

        public T GetGridObject (int x, int y) {
            if (IsXYValid(x, y)) {
                return _gridArray[x, y];
            } else {
                return default;
            }
        }

        public T GetGridObject (Vector3 worldposition) {
            int x, y;
            GetXY (worldposition, out x, out y);
            return GetGridObject (x, y);
        }

        public void Swap(int fromX, int fromY, int toX, int toY) {
            T from = GetGridObject(fromX, fromY);
            T to = GetGridObject(toX, toY);
            SetGridObject(fromX, fromY, to);
            SetGridObject(toX, toY, from);
        }

        public void DrawGizmos (Color color, float duration = 0) {

            for (var x = 0; x < _gridArray.GetLength (0); x++) {
                for (var y = 0; y < _gridArray.GetLength (1); y++) {
                    Debug.DrawLine (GetWorldPosition (x, y), GetWorldPosition (x, y + 1), color, duration);
                    Debug.DrawLine (GetWorldPosition (x, y), GetWorldPosition (x + 1, y), color, duration);
                }
            }

            Debug.DrawLine (GetWorldPosition (0, _height), GetWorldPosition (_width, _height), color, duration);
            Debug.DrawLine (GetWorldPosition (_width, _height), GetWorldPosition (_width, 0), color, duration);
        }

        public void DrawDebugText (Color? textColor = null, int textFont = 5, Transform textParent = null) {

            _debugTextArray = new TextMeshPro[_width, _height];

            for (var x = 0; x < _gridArray.GetLength (0); x++) {
                for (var y = 0; y < _gridArray.GetLength (1); y++) {
                    _debugTextArray[x, y] = MyUtilsClass.CreateWorldText (_gridArray[x, y]?.ToString (), textParent, GetWorldPositionInCenter (x, y), textFont, textColor ?? Color.white, TextAnchor.MiddleCenter);
                }
            }

            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs e) =>
            {
                _debugTextArray[e.x, e.y].text = _gridArray[e.x, e.y]?.ToString();
            };
        }
    }

    [System.Serializable]
    public struct GridGizmosSettings {
        public bool onDrawGizmosSelected;
        public Color gridColor;
        public float gridDuration; //0 - 0.02

        [Space]
        public bool drawDebugText;
        public Color textColor;
        public int textFontSize; // 5
        public Transform textParent;
    }
}