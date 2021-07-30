using System;
using UnityEngine;

namespace d4160.Grid
{
    public class GridNumeric<T> : Grid<T> where T: struct
    {
        private T _min;
        private T _max;

        public GridNumeric(int width, int height, Vector2 cellSize, Vector3 originPosition = default, bool drawDebugText = true, Color? textColor = null, int textFontSize = 5, Transform textParent = null) : base(width, height, cellSize, originPosition, drawDebugText, textColor, textFontSize, textParent)
        {
        }

        public void SetMinMax(T min, T max) { _min = min; _max = max; }

        protected override T ProcessValue(int x, int y, T value) {
            switch (value) {
                case int v:
                    return (T)(object) Mathf.Clamp(v, (int)(object)_min, (int)(object)_max);
                case float v:
                    return (T)(object) Mathf.Clamp(v, (float)(object)_min, (float)(object)_max);
            }

            return value;
        }
    }
}