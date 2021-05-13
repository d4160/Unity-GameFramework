using UnityEngine;

namespace d4160.Grid
{
    public abstract class GridMonoBehaviour<T> : GridBehaviour where T : MonoBehaviour
    {
        protected GridMono<T> _grid;

        protected override void InstanceGrid(bool drawText = false, Color textColor = default, int textFontSize = 5, Transform textParent = null)
        {
            _grid = new GridMono<T>(_width, _height, _cellSize, transform.position, drawText, textColor, textFontSize, textParent);
        }

        protected override void DrawGizmos()
        {
            if (_grid == null) InstanceGrid();

            _grid.CellSize = _cellSize;
            _grid.OriginPosition = transform.position;
            _grid.DrawGizmos(_gizmosSettings.gridColor, _gizmosSettings.gridDuration);
        }
    }
}