using TMPro;
using UnityEngine;

namespace d4160.Collections
{
    public class Grid2D<T> : RuntimeSet2D<T>
    {
        public Vector2 CellSize { get; set; }
        public Vector3 OriginPosition { get; set; }

        protected TextMeshPro[,] _debugTextArray = null;

        public Grid2D()
        {

        }

        public Grid2D(int width, int height, Vector2 cellSize, Vector3 originPosition = default, bool drawDebugText = true, Color? textColor = null, int textFontSize = 5, Transform textParent = null) : base(width, height)
        {
            if (Logger) Logger.LogInfo($"Grid2D Ctor();");

            CellSize = cellSize;
            OriginPosition = originPosition;
        }
    }
}
